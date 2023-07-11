import { ChangeDetectorRef, Component, DoCheck, EventEmitter, OnInit } from '@angular/core';
import { EChartOption } from 'echarts';
import { DashboardsService } from '../../../shared/services/dashboards.service';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NgxSpinnerService } from "ngx-spinner";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgbModal, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Paginator } from 'src/app/shared/models/paginator.model';
import * as FileSaver from "file-saver";
import * as XLSX from 'xlsx';
import { Utils } from 'src/app/shared/utils';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { SoftlandService } from 'src/app/shared/services/softland.service';
import { ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';


const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

interface AutoCompleteModel {
    value: any;
    display: string;
}

@Component({
    selector: 'app-dashboard-administrador',
    templateUrl: './dashboard-administrador.component.html',
    styleUrls: ['./dashboard-administrador.component.scss'],
    animations: [SharedAnimations]
})
export class DashboardAdministradorComponent implements OnInit, DoCheck {
    chartLineOption1: EChartOption;
    chartLineOption2: EChartOption;
    chartLineOption3: EChartOption;
    salesChartBar: EChartOption;
    salesChartBar2: EChartOption;
    chartPie1: any;
    detalleCab: any = null;
    detalleDet: any = [];
    public otrosCorreo: AutoCompleteModel[] = [];
    showDetail: boolean = false;
    tituloDetalle: string = '';
    contactosCliente: any = [];
    otroCorreo: string = '';
    documentoAEnviar: any = null;
    public enviaPdf: boolean = false;
    public enviaXml: boolean = false;
    contactosSeleccionados: any = [];
    cantidadPagados: number = 0;
    totalPagados: number = 0;
    rangoRegistros: any = [];
    dateDesde: NgbDateStruct;
    dateHasta: NgbDateStruct;
    showDashboard: number = 1;
    dashboardAdmin: any = null;
    detallePago: any = [];
    rangoInicial: number = 0;
    cantidadVencidos: number = 0;
    cantidadPendiente: number = 0;
    totalVencidos: number = 0;
    totalPendiente: number = 0;
    totalPorVencer: number = 0;
    cantidadPorVencer: number = 0;
    tipoDoc: number = 0;
    configuracion: ConfiguracionPortal = new ConfiguracionPortal();
    public ultimosPagos: any[] = [];
    public ultimosRechazados: any[] = [];
    docPagado: any = null;
    tiposDocumentos: any[] = [];
    searchRut: string = '';
    noResultText = '';
    documentos: any[] = [];
    documentosPorPagina: any[] = [];
    documentosFiltro: any[] = [];
    documentosPagados: any[] = [];
    documentosPagadosPorPagina: any[] = [];
    documentosPagadosFiltro: any[] = [];
    folio: number = null;
    configDocumentos: any;
    existModuloInventario: boolean = false;
    public paginadorDocumentos: Paginator = {
        startRow: 0,
        endRow: 10,
        sortBy: 'desc',
        search: '',
        documentos: []
    };

    public spinnerResumen = 'spinnerResumen';
    public spinnerDeudores = 'spinnerDeudores';
    public graphicDeudoresLoad: boolean = false;
    public graphicResumenLoad: boolean = false;
    clienteMostrar: number = -1;
    topDeudores: any[] = [];
    idPago: number = null;
    estadosPagos: any[] = [{ id: 1, nombre: 'TODOS' }, { id: 2, nombre: 'FINALIZADO' }, { id: 3, nombre: 'FINALIZADO SIN COMPROBANTE' }];
    selectedEstadoPagos: number = 1;
    pagoComprobante: any = null;
    deudaVsPago: any[] = [];
    compraDetalleAux = null;
    updateDocumentos = false;
    updateDocumentosEvent = new EventEmitter<any>();
    updateClientes = true;
    updateClientesEvent = new EventEmitter<any>();
    updatePagados = true;
    updatePagadosEvent = new EventEmitter<any>();
    cliente: any = null;

    constructor(private dashboardsService: DashboardsService,
        private clienteService: ClientesService,
        private authService: AuthService,  //private montoPipe: MontoPipe,
        private spinner: NgxSpinnerService, private utils: Utils, private configuracionService: ConfiguracionPagoClientesService,
        private notificationService: NotificationService, private softlandConfigService: SoftlandService,
        private modalService: NgbModal, private softlandService: ConfiguracionSoftlandService, private cdr: ChangeDetectorRef) { }

    ngOnInit() {

        this.getCantidadDocumentos();
        this.getTopDeudores();
        // this.getDeudaVsPagos();
    }

    ngDoCheck() {
        if (this.updateDocumentos) {
            this.updateDocumentosEvent.subscribe(documentos => {

                let c = this.documentosPorPagina.filter(x => x.codaux == this.cliente.codaux);
                if (c.length > 0) {
                    c[0].documentosPorPagina = documentos;
                    this.cdr.markForCheck();

                }
                this.updateDocumentos = false;
            });
        }

        // if (this.updateClientes) {
        //     this.updateClientesEvent.subscribe(clientes => {
        //         this.documentosPorPagina = clientes
        //         this.documentosPorPagina.forEach(element => {
        //             element.paginadorDocumentos = {
        //                 startRow: 0,
        //                 endRow: 10,
        //                 sortBy: 'desc',
        //                 search: ''
        //             }

        //             element.configDocumentosCliente = {
        //                 itemsPerPage: element.paginadorDocumentos.endRow,
        //                 currentPage: 1,
        //                 totalItems: element.documentosFiltro.length,
        //                 id: element.idPaginador
        //             }
        //             element.documentosPorPagina = element.documentosFiltro.slice(element.paginadorDocumentos.startRow, element.paginadorDocumentos.endRow)
        //             element.documentosFiltro = element.documentosFiltro
        //             element.muestraDocumentos = false;
        //         });
        //         this.updateClientes = false;
        //     });
        // }

        if (this.updatePagados) {
            this.updateClientesEvent.subscribe(pagados => {
                this.documentosPorPagina = pagados
                this.updatePagados = false;
            });
        }
    }


    getTopDeudores() {
        this.spinner.show(this.spinnerDeudores);
        this.clienteService.getTopDeudores().subscribe((res: any) => {
            this.topDeudores = res;
            let clientes = [];
            let montos = [];
            this.topDeudores.forEach(element => {
                clientes.push(element.nomaux);
                montos.push(element.saldo);
            });

            let yAxis = {
                type: 'value',
                axisLabel: {
                    formatter: '${value}'
                },
                // min: 0,
                // max:  this.topDeudores.reduce(function(a,b){return Math.max(a.totalDeuda,b.totalDeuda)}),
                // interval: this.topDeudores.reduce(function(a,b){return Math.max(a.totalDeuda,b.totalDeuda)}) / 10,
                // axisLine: {
                //     show: true
                // },
                // splitLine: {
                //     show: true,
                //     interval: 'auto'
                // }

            };

            this.salesChartBar = {
                // legend: {
                //     borderRadius: 0,
                //     orient: 'horizontal',
                //     x: 'right',

                // },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true,
                },
                tooltip: {
                    show: true,
                    backgroundColor: 'rgba(0, 0, 0, .8)',

                },
                xAxis: [{
                    type: 'category',
                    data: clientes,
                    axisTick: {
                        alignWithLabel: false
                    },
                    axisLabel: {
                        fontSize: 10,
                        rotate: -90,
                        overflow: 'truncate',
                        width: 10,
                    },

                    // axisLabel: {
                    //     inside: true,
                    //     color: '#fff',
                    //   },
                    // splitLine: {
                    //     show: true
                    // },
                    // axisLine: {
                    //     show: true
                    // }
                }],
                yAxis: [yAxis //FCA 05-04-2022 SE CAMBIAN CANTIDAD DE INTERVALOS
                ],

                series: [{
                    name: '',
                    data: montos,
                    label: { show: false, color: '#fff' },
                    type: 'bar',
                    barGap: 0,
                    color: '#bcbbdd',
                    smooth: true,
                    barCategoryGap: '40%',
                    animation: false,
                },
                ]
            }


            this.graphicDeudoresLoad = true;
            this.spinner.hide(this.spinnerDeudores);
        }, err => { this.notificationService.error('Ocurrió un error al obtener grafico.', '', true); });
    }

    getDeudaVsPagos() {
        this.spinner.show(this.spinnerResumen);

        this.clienteService.getDeudaVsPagos().subscribe((res: any) => {
            this.deudaVsPago = res
            let meses = [];
            let deudas = [];
            let pagos = [];
            this.deudaVsPago.forEach(element => {
                meses.push(element.fechaTexto);
                deudas.push(element.totalDeuda);
                pagos.push(element.totalPagos);
            });

            let yAxis = {
                type: 'value',
                axisLabel: {
                    formatter: '${value}'
                },
                // min: 0,
                // max:  this.topDeudores.reduce(function(a,b){return Math.max(a.totalDeuda,b.totalDeuda)}),
                // interval: this.topDeudores.reduce(function(a,b){return Math.max(a.totalDeuda,b.totalDeuda)}) / 10,
                // axisLine: {
                //     show: true
                // },
                // splitLine: {
                //     show: true,
                //     interval: 'auto'
                // }

            };

            this.salesChartBar2 = {
                legend: {
                    borderRadius: 0,
                    orient: 'horizontal',
                    x: 'right',

                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true,
                },
                tooltip: {
                    show: true,
                    backgroundColor: 'rgba(0, 0, 0, .8)',

                },
                xAxis: [{
                    type: 'category',
                    data: meses,
                    axisTick: {
                        alignWithLabel: false
                    },
                    axisLabel: {
                        fontSize: 10,
                        rotate: -90,
                        overflow: 'truncate',
                        width: 10,
                    },

                    // axisLabel: {
                    //     inside: true,
                    //     color: '#fff',
                    //   },
                    // splitLine: {
                    //     show: true
                    // },
                    // axisLine: {
                    //     show: true
                    // }
                }],
                yAxis: [yAxis //FCA 05-04-2022 SE CAMBIAN CANTIDAD DE INTERVALOS
                ],

                series: [{
                    name: 'Deuda Vencimiento en el mes',
                    data: deudas,
                    label: { show: false, color: '#fff' },
                    type: 'bar',
                    barGap: 0,
                    color: '#bcbbdd',
                    smooth: true,
                    barCategoryGap: '40%',
                    animation: true,
                },
                {
                    name: 'Pagos Portal',
                    data: pagos,
                    label: { show: false, color: '#fff' },
                    type: 'bar',
                    barGap: 0,
                    color: '#C4DAC3',
                    smooth: true,
                    barCategoryGap: '40%',
                    animation: true,
                }
                ]
            }
            this.graphicResumenLoad = true;
            this.spinner.hide(this.spinnerResumen);
        }, err => { this.notificationService.error('Ocurrió un error al obtener grafico.', '', true); });
    }

    // getExistenCompras() {
    //     this.spinner.show();
    //     this.clienteService.getExistCompras().subscribe((res: any) => {

    //         if (res.hoy == true) {
    //             this.rangoRegistros.push({ id: 1, rango: "Hoy" });
    //         }
    //         if (res.semana == true) {
    //             this.rangoRegistros.push({ id: 2, rango: "Semana" });
    //         }
    //         if (res.mes == true) {
    //             this.rangoRegistros.push({ id: 3, rango: "Mes" });
    //         }
    //         if (res.anio == true) {
    //             this.rangoRegistros.push({ id: 4, rango: "Año" });
    //         }
    //         this.rangoRegistros.push({ id: 5, rango: "Rango de fecha" });
    //         this.rangoInicial = 3;
    //         if (this.rangoInicial != 5) {
    //             this.getCantidadDocumentos();
    //         }


    //     }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener pagos.', '', true); });
    // }

    // getDashboardEcommerce(tipo: number) {

    //     const model = { Rut: '', CodAux: '', Nombre: '', Folio: 0, TipoDoc: '', correos: '', TipoEnvioDoc: '', TipoBusqueda: tipo, fechaDesde: this.dateDesde, fechaHasta: this.dateHasta };
    //     this.clienteService.getDashboardAdministrador(model).subscribe((res: any) => {
    //         this.totalPagados = res[0].montoTotal;
    //         this.cantidadPagados = res[0].cantidadDocumentos;
    //         this.getCantidadDocumentos();
    //     }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener pagos.', '', true); });

    // }

    getCantidadDocumentos() {

        this.spinner.show();
        const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
        if (configuracionCompletaPortal != null) {

            this.configuracion = configuracionCompletaPortal.configuracionPortal;
            this.existModuloInventario = configuracionCompletaPortal.existModuloInventario;

            if (configuracionCompletaPortal.configuracionPagoCliente.cuentasContablesDeuda = ! '' && configuracionCompletaPortal.configuracionPagoCliente.cuentasContablesDeuda != null) {
                this.clienteService.getDocumentosDashboardAdministrador().subscribe((res: any) => {
    
                    this.dashboardAdmin = res;
    
                    this.cantidadVencidos = this.dashboardAdmin.cantidadVencida;
                    this.totalVencidos = this.dashboardAdmin.montoVencido;
                    this.cantidadPendiente = this.dashboardAdmin.cantidadDocPendiente;
                    this.totalPendiente = this.dashboardAdmin.saldoPendiente;
                    this.cantidadPorVencer = this.dashboardAdmin.cantidadxVencer;
                    this.totalPorVencer = this.dashboardAdmin.saldoxvencer;
                    this.totalPagados = this.dashboardAdmin.cantidadDocumentosPagados;
                    this.cantidadPagados = this.dashboardAdmin.montoPagado;
                    this.spinner.hide();
    
                }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos.', '', true); });
            } else {
                this.spinner.hide();
                this.notificationService.info('', 'Portal no ha sido inicializado, favor ir al módulo de administración y realizar las configuraciones correspondientes.', false);
            }
        }else{
            this.spinner.hide();
            this.authService.signoutExpiredToken();
        }
    }

    verDetalle(pago: any) {
        this.clienteService.getDetallePago(pago.idPago).subscribe((res: any) => {

            this.showDetail = true;
            this.detallePago = res;

        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    }


    volver() {
        this.detalleCab = null;
        this.detalleDet = [];
        this.showDetail = false;

    }

    print() {
        if (window) {
            window.print();
        }
    }



    openModalSend(content, doc: any) {
        this.spinner.show();
        this.enviaPdf = false;
        this.enviaXml = false;
        this.contactosCliente = [];
        this.documentoAEnviar = doc;
        this.otroCorreo = ''; //FCA 05-07-2022
        var user = this.authService.getuser();
        if (user != null) {
            // let vm = JSON.parse(user);
            const rut: string[] = this.detalleCab.rut.split('-');
            const codAux: string = rut[0].replace('.', '').replace('.', '')

            this.clienteService.getContactosClienteSoftland(codAux).subscribe(res => {
                this.contactosCliente = res;
                this.spinner.hide();
                this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
        }

    }

    onChange(item: any) {
        const added = this.contactosSeleccionados.find(x => x.correoContacto == item.correoContacto);
        if (added == null) {
            this.contactosSeleccionados.push(item)
        } else {
            for (let i: number = 0; i <= this.contactosSeleccionados.length - 1; i++) {
                if (this.contactosSeleccionados[i].correoContacto == item.correoContacto) {
                    this.contactosSeleccionados.splice(i, 1);
                    break;
                }
            }
        }
    }

    // onChangeRango(rango: any) {
    //     if (rango.nextId == 5) {
    //         this.showDashboard = 0;
    //     }
    //     else {
    //         this.showDashboard = 1;
    //         this.getDashboardEcommerce(rango.nextId);
    //     }

    // }

    // filter() {
    //     this.getDashboardEcommerce(5);
    //     this.showDashboard = 1;
    // }


    obtenerDocumentos(estado: number) {

        this.tipoDoc = estado;
        const user = this.authService.getuser();

        if (this.tipoDoc == 4) {
            if (user != null) {
                this.spinner.show();
                let codaux = '0';
                if (this.searchRut != null && this.searchRut != '') {
                    codaux = this.searchRut.replace('.', '').split('-')[0];
                }

                this.noResultText = "No se encontraron resultados"
                this.clienteService.getDocumentosPagadosAdministrador().subscribe((res: any[]) => {

                    this.documentosPagados = res;
                    this.documentosPagadosPorPagina = res.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                    this.documentosPagadosFiltro = res;
                    this.configDocumentos = {
                        itemsPerPage: this.paginadorDocumentos.endRow,
                        currentPage: 1,
                        totalItems: this.documentosPagados.length
                    };
                    this.spinner.hide();
                }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });
            }
        } else {
            switch (estado) {
                case 1:
                    if (this.cantidadPendiente == 0) {
                        this.notificationService.warning('No existen documentos pendientes', '', true);
                        return;
                    }
                    break;

                case 2:
                    if (this.cantidadVencidos == 0) {
                        this.notificationService.warning('No existen documentos vencidos', '', true);
                        return;
                    }
                    break;
                case 2:
                    if (this.cantidadPorVencer == 0) {
                        this.notificationService.warning('No existen documentos por vencer', '', true);
                        return;
                    }
                    break;
            }
            this.dateDesde = null;
            this.dateHasta = null;
            this.folio = null;
            this.paginadorDocumentos.startRow = 0;
            this.paginadorDocumentos.endRow = 10;
            this.paginadorDocumentos.sortBy = 'desc';
            this.paginadorDocumentos.search = '';

            if (user != null) {
                this.spinner.show();
                this.noResultText = "No se encontraron resultados"

                let model = {
                    TipoBusqueda: this.tipoDoc,
                    Pagina: 1
                }
                this.clienteService.getDocumentosResumenAdministrador(model).subscribe((res: any[]) => {
                    this.documentos = res;
                    this.documentosPorPagina = this.documentos;
                    this.documentosPorPagina.forEach(element => {
                        let paginadorDocumentos = {
                            startRow: 0,
                            endRow: 10,
                            sortBy: 'desc',
                            idPaginador: element.codaux
                        }
                        element.paginadorDocumentos = paginadorDocumentos;
                        element.documentosPorPagina = null;
                        element.idPaginador = element.codaux;

                        element.configDocumentosCliente = {
                            itemsPerPage: 10,
                            currentPage: 1,
                            totalItems: 0,
                            id: element.codaux
                        };
                    });
                    this.documentosFiltro = res;
                    this.configDocumentos = {
                        itemsPerPage: this.paginadorDocumentos.endRow,
                        currentPage: 1,
                        totalItems: this.documentos.length > 0 ? this.documentos[0].total : 0
                    };
                    this.spinner.hide();

                }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });
            }
        }

    }

    validaRut() {
        if (this.searchRut != "" && this.searchRut != null) {
            if (this.utils.isValidRUT(this.searchRut)) {
                this.searchRut = this.utils.checkRut(this.searchRut);
            } else {
                this.searchRut = '';
                this.notificationService.warning('Rut ingresado no es valido', '', true);
            }
        }
    }

    volverDashboard() {
        this.tipoDoc = 0;
        this.paginadorDocumentos.startRow = 0;
        this.paginadorDocumentos.endRow = 10;
        this.paginadorDocumentos.sortBy = 'desc';
        this.paginadorDocumentos.search = '';
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;
        this.otroCorreo = '';
        this.searchRut = '';
        this.documentosPorPagina = [];

        if (!this.graphicResumenLoad) {
            this.spinner.show(this.spinnerResumen);
        }

        if (!this.graphicDeudoresLoad) {
            this.spinner.show(this.spinnerDeudores);
        }
    }


    changePageDocumentos(event: any) {
        this.spinner.show();
        this.paginadorDocumentos.startRow = 0;
        this.paginadorDocumentos.endRow = 10;
        this.paginadorDocumentos.sortBy = 'desc';
        this.configDocumentos.currentPage = event;

        let fHasta = null;
        if (this.dateHasta != null) {
            fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
        }

        let fDesde = null;
        if (this.dateDesde != null) {
            fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        }

        let model = {
            TipoBusqueda: this.tipoDoc,
            fechaDesde: fDesde,
            fechaHasta: fHasta,
            Pagina: this.configDocumentos.currentPage,
            CodAux: this.searchRut,
            Folio: this.folio
        }
        this.clienteService.getDocumentosResumenAdministrador(model).subscribe((res: any[]) => {
            this.documentos = res;
            this.documentosPorPagina = this.documentos;
            this.documentosPorPagina.forEach(element => {
                let paginadorDocumentos = {
                    startRow: 0,
                    endRow: 10,
                    sortBy: 'desc',
                    idPaginador: element.codaux
                }
                element.paginadorDocumentos = paginadorDocumentos;
                element.documentosPorPagina = null;
                element.idPaginador = element.codaux;

                element.configDocumentosCliente = {
                    itemsPerPage: 10,
                    currentPage: 1,
                    totalItems: 0,
                    id: element.codaux
                };
            });
            this.documentosFiltro = res;
            this.configDocumentos = {
                itemsPerPage: this.paginadorDocumentos.endRow,
                currentPage: this.configDocumentos.currentPage,
                totalItems: this.documentos.length > 0 ? this.documentos[0].total : 0
            };
            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });
    }

    changePageDocumentosPagados(event: any) {
        this.paginadorDocumentos.startRow = ((event - 1) * 10);
        this.paginadorDocumentos.endRow = (event * 10);
        this.paginadorDocumentos.sortBy = 'desc';
        this.configDocumentos.currentPage = event;
        this.documentosPagadosPorPagina = this.documentosPagadosFiltro.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
        // this.updatePagados = true
        // this.updatePagadosEvent.emit(this.documentosFiltro.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow));

    }


    filterDocs() {
        this.spinner.show();

        let fHasta = null;
        if (this.dateHasta != null) {
            fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
        }

        let fDesde = null;
        if (this.dateDesde != null) {
            fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        }

        let model = {
            TipoBusqueda: this.tipoDoc,
            fechaDesde: fDesde,
            fechaHasta: fHasta,
            Pagina: 1,
            CodAux: this.searchRut,
            Folio: this.folio
        }
        this.clienteService.getDocumentosResumenAdministrador(model).subscribe((res: any[]) => {
            this.documentos = res;
            this.documentosPorPagina = this.documentos;
            this.documentosPorPagina.forEach(element => {
                let paginadorDocumentos = {
                    startRow: 0,
                    endRow: 10,
                    sortBy: 'desc',
                    idPaginador: element.codaux
                }
                element.paginadorDocumentos = paginadorDocumentos;
                element.documentosPorPagina = null;
                element.idPaginador = element.codaux;

                element.configDocumentosCliente = {
                    itemsPerPage: 10,
                    currentPage: 1,
                    totalItems: 0,
                    id: element.codaux
                };
            });
            this.documentosFiltro = res;
            this.configDocumentos = {
                itemsPerPage: this.paginadorDocumentos.endRow,
                currentPage: 1,
                totalItems: this.documentos.length > 0 ? this.documentos[0].total : 0
            };
            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });
    }


    filterDocsPagados() {
        debugger
        this.documentosPagadosFiltro = this.documentosPagados;

        if (this.selectedEstadoPagos == 2) {
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.comprobanteContable != '' && x.comprobanteContable != null)
        } else if (this.selectedEstadoPagos == 3) {
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.comprobanteContable == '' || x.comprobanteContable == null)
        }

        if (this.searchRut != null && this.searchRut != '') {
            let codAux = this.searchRut.replace('.', '').replace('.', '').split('-')[0];
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.rut == this.searchRut)
        }

        if (this.folio != null) {
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.comprobanteContable == this.folio)
        }

        if (this.dateDesde != null) {
            const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => new Date(x.fechaPago) >= fDesde)
        }

        if (this.dateHasta != null) {
            const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => new Date(x.fechaPago) <= fHasta)
        }

        this.paginadorDocumentos.startRow = 0;
        this.paginadorDocumentos.endRow = 10;
        this.paginadorDocumentos.sortBy = 'desc';
        this.configDocumentos = {
            itemsPerPage: this.paginadorDocumentos.endRow,
            currentPage: 1,
            totalItems: this.documentosPagadosFiltro.length
        };


        this.updatePagados = true
        //this.updatePagadosEvent.emit(this.documentosPagadosFiltro.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow));
        this.documentosPagadosPorPagina = this.documentosPagadosFiltro.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);


    }

    changeEstadoPagos() {
        if (this.selectedEstadoPagos == 3) {
            this.folio = null;
        }

    }

    limpiarFiltros() {
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;
        this.searchRut = null;

        this.filterDocs();
    }


    limpiarFiltrosPagados() {
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;
        this.searchRut = null;
        this.selectedEstadoPagos = 1;

        this.filterDocsPagados();
    }

    export() {
        let data = this.documentos;


        var listaExportacion = [];
        data.forEach((element) => {
            const model = { RUT: element.rut, Nombre: element.nombre, Cantidad_Documentos: element.cantidadDocumentos, Total: element.totalDocumentos, Saldo: element.totalSaldo };
            listaExportacion.push(model);
        });
        var nombre;
        if (this.tipoDoc == 1) {
            nombre = "Total por clientes: Documentos Pendientes";
        }
        if (this.tipoDoc == 2) {
            nombre = "Total por clientes: Documentos Vencidos";
        }
        if (this.tipoDoc == 3) {
            nombre = "Total por clientes: Documentos Por Vencer";
        }
        this.exportAsExcelFile(listaExportacion, nombre);
    }




    exportPagados() {
        let data = this.documentosPagados;

        if (this.searchRut != null && this.searchRut != '') {
            let codAux = this.searchRut.replace('.', '').replace('.', '').split('-')[0];
            data = data.filter(x => x.codAux == codAux)
        }
        if (this.folio != null) {
            data = data.filter(x => x.comprobanteContable == this.folio)
        }
        if (this.dateDesde != null) {
            const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
            data = data.filter(x => new Date(x.fechaPago) >= fDesde)
        }
        if (this.dateHasta != null) {
            const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
            data = data.filter(x => new Date(x.fechaPago) <= fHasta)
        }

        if (this.selectedEstadoPagos == 2) {
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.comprobanteContable != '' && x.comprobanteContable != null)
        } else if (this.selectedEstadoPagos == 3) {
            this.documentosPagadosFiltro = this.documentosPagadosFiltro.filter(x => x.comprobanteContable == '' || x.comprobanteContable == null)
        }

        let listaExportacion = [];
        let fechaPago;
        data.forEach((element) => {
            if (element.fechaPago == null || element.fechaPago == undefined) {
                fechaPago = '';
            } else {
                fechaPago = new Date(element.fechaPago);
                fechaPago = fechaPago.getDate() + "/" + (fechaPago.getMonth() + 1) + "/" + fechaPago.getFullYear();
            }

            var fechaEmision;
            fechaEmision = new Date(element.fechaEmision);
            fechaEmision = fechaEmision.getDate() + "/" + (fechaEmision.getMonth() + 1) + "/" + fechaEmision.getFullYear();
            let numComprobante = element.comprobanteContable == '' || element.comprobanteContable == null ? ' Error al generar comprobante' : element.comprobanteContable;
            const model = { Rut_Cliente: element.rut, Numero_Comprobante: numComprobante, Monto: element.montoPago, Fecha_Pago: fechaPago, Hora_Pago: element.horaPago };
            listaExportacion.push(model);
        });
        let nombre = "Pagos";

        this.exportAsExcelFile(listaExportacion, nombre);
    }

    public exportAsExcelFile(rows: any[], excelFileName: string): void {


        const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
        const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
        const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        this.saveAsExcelFile(excelBuffer, excelFileName);

    }

    private saveAsExcelFile(buffer: any, baseFileName: string): void {
        const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
        FileSaver.saveAs(data, baseFileName + EXCEL_EXTENSION);
    }



    verDetalleDocumento(compra: any) {
        // this.documentosPorPagina.forEach(element => {
        //     element.muestraDocumentos = false;
        // });
        var user = this.authService.getuser();
        var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

        if (user != null) {
            // let vm = JSON.parse(user);
            obj.Folio = compra.folio;
            obj.CodAux = user.rut;
            if (compra.folio == null || compra.folio == undefined) {
                obj.Folio = compra.nro;
            } else {
                obj.Folio = compra.folio;
            }

            obj.CodAux = compra.codAux;
            obj.TipoDoc = compra.tipoDoc; //FCA 05-07-2022
        }

        this.compraDetalleAux = obj;
        this.spinner.show();

        this.clienteService.getDetalleCompra(obj).subscribe((res: any) => {
            this.spinner.hide();

            if (res.cabecera != null) {
                if (res.cabecera.folio != 0) { //FCA 05-07-2022               
                    this.detalleCab = res.cabecera; //FCA 05-07-2022
                    this.detalleCab.tipo = obj.TipoDoc;
                    this.detalleDet = res.detalle;
                    this.showDetail = true;
                    this.tituloDetalle = compra.documento;
                } else {
                    this.notificationService.info('Compra no posee detalle asociado.', '', true);
                }
            } else {
                this.notificationService.info('Compra no posee detalle asociado.', '', true);
            }
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    }



    verDetalleDocumentoPagado(pago: any) {
        this.spinner.show();
        this.softlandService.getAllTipoDocSoftland().subscribe((res: any) => {
            this.tiposDocumentos = res;

            this.docPagado = pago;
            this.docPagado.pagosDetalle.forEach(element => {
                let doc = this.tiposDocumentos.filter(x => x.codDoc == element.tipoDocumento);
                if (doc.length > 0) {
                    element.desDoc = doc[0].desDoc;
                }
            });
            this.showDetail = true;
            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener tipos de  documentos.', '', true); });
    }


    printDocumento() {
        if (window) {

            this.clienteService.getdocumentoPDF(this.detalleCab).subscribe(res => {
                let a = document.createElement("a");
                a.href = "data:application/octet-stream;base64," + res;
                a.download = "documentName.pdf"
                a.click();
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });

        }
    }


    openModalComprobanteContable(content, doc: any) {
        this.pagoComprobante = '';
        this.idPago = doc.idPago;
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });

    }


    generarComprobante() {
        this.spinner.show();
        this.softlandService.generaComprobantePago(this.idPago).subscribe(res => {

            this.spinner.hide();
            this.modalService.dismissAll();
            if (res == '' || res == null) {
                this.notificationService.warning('Ocurrio un error al generar comprobante, Reintentelo mas tarde.', '', true);
            } else {
                this.notificationService.success('Se genero el comprobante N° ' + res, '', true);
            }
            this.obtenerDocumentos(4);
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
    }


    actualizaComprobante() {
        this.spinner.show();
        let model = {
            comprobanteContable: this.pagoComprobante,
            idPago: this.idPago
        }
        this.softlandService.actualizaComprobantePago(model).subscribe(res => {
            this.modalService.dismissAll();
            this.obtenerDocumentos(4);
            this.notificationService.success('Se ha actualizado correctamente el Número de comprobante', '', true);
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
    }



    descargarDocumentos(tipo: number, doc: any) {

        if (!this.enviaPdf && !this.enviaXml) {
            this.notificationService.warning('Debe seleccionar documentos a descargar.', '', true);
            return;
        }

        if (this.enviaPdf) {
            this.downloadPDF();
        }

        if (this.enviaXml) {
            this.downloadXML();
        }

    }



    downloadPDF() {
        var user = this.authService.getuser();
        var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

        if (user != null) {
            obj.Folio = this.documentoAEnviar.folio;
            obj.TipoDoc = this.documentoAEnviar.tipo;
            obj.CodAux = this.compraDetalleAux.CodAux;
        }





        this.spinner.show();

        //Obtengo ruta
        this.clienteService.getClienteDocumento(obj).subscribe(
            (res: any) => {
                if (res.base64 != null && res.base64 != '') {
                    var link = document.createElement("a");
                    link.download = res.nombreArchivo;
                    link.href = this.utils.transformaDocumento64(res.base64, res.tipo);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    link.remove();
                } else {
                    this.notificationService.warning('Documento sin archivo PDF, favor contactar con el administrador.', '', true);
                }

                this.spinner.hide();
            },
            err => { this.notificationService.error('Error al obtener Documento', '', true); this.spinner.hide(); }
        );
    }


    downloadXML() {

        var user = this.authService.getuser();
        var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

        if (user != null) {
            if (this.documentoAEnviar.nro != undefined) {
                obj.Folio = this.documentoAEnviar.nro;
                obj.TipoDoc = this.documentoAEnviar.movtipdocref;//fca 08-07-2022 FALTA TIPO DOCUMENTO NO VIEN EN LA API SI ES PRODUCTO
            } else {
                obj.Folio = this.documentoAEnviar.folio;
                obj.TipoDoc = this.documentoAEnviar.tipo;
            }
            debugger
            obj.CodAux = this.compraDetalleAux.CodAux;
        }

        this.spinner.show();

        //Obtengo ruta
        this.clienteService.getClienteXML(obj).subscribe(
            (res: any) => {

                if (res.base64 != null && res.base64 != '') {
                    var link = document.createElement("a");
                    link.download = res.nombreArchivo;
                    link.href = this.utils.transformaDocumento64(res.base64, res.tipo);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    link.remove();
                } else {
                    this.notificationService.warning('Documento sin archivo xml, favor contactar con el administrador.', '', true);
                }

                this.spinner.hide();
            },
            err => { this.notificationService.error('Error al obtener Documento', '', true); this.spinner.hide(); }
        );
    }


    public onAdd(tag: AutoCompleteModel, type: number) {
        this.removeFromArrayIfMailIsInvalid(tag.value, this.otrosCorreo);
        this.toLowerMails(this.otrosCorreo);
    }

    toLowerMails(arrayCorreos: AutoCompleteModel[]) {
        for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
            arrayCorreos[i].value = arrayCorreos[i].value.toLowerCase();
            arrayCorreos[i].display = arrayCorreos[i].display.toLowerCase();
        }
    }

    removeFromArrayIfMailIsInvalid(mail: string, arrayCorreos: AutoCompleteModel[]) {
        if (!this.utils.validateMail(mail)) {
            this.notificationService.warning('Debe ingresar un correo válido.', '', true);
            for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
                if (arrayCorreos[i].value == mail) {
                    arrayCorreos.splice(i, 1);
                    break;
                }
            }
            return;
        }
    }



    send() {
        if (this.contactosSeleccionados.length == 0 && this.otrosCorreo.length == 0) {
            if (this.configuracion.muestraContactosEnvio == 1) {
                this.notificationService.warning('Debe seleccionar o ingresar un correo.', '', true);
            } else {
                this.notificationService.warning('Debe ingresar un correo.', '', true);
            }

            return;
        }

        if (!this.enviaPdf && !this.enviaXml) {
            this.notificationService.warning('Debe seleccionar almenos un tipo de documento.', '', true);
            return;
        }

        this.spinner.show();

        let correos: string = '';
        this.contactosSeleccionados.forEach(element => {
            if (correos == '') {
                correos = correos + element.correo;
            } else {
                correos = correos + ";" + element.correo;
            }

        });

        this.otrosCorreo.forEach(element => {
            if (correos == '') {
                correos += `${element.value}`;
            } else {
                correos += `;${element.value}`;
            }

        });

        const user = this.authService.getuser();


        if (user != null) {
            if (!this.enviaPdf && !this.enviaXml) {
                this.notificationService.warning('Debe seleccionar un documento para enviar.', '', true);
                return;
            }

            var tipoEnvio = (this.enviaPdf && this.enviaXml) ? 3 : (this.enviaPdf) ? 1 : (this.enviaXml) ? 2 : 3;

            var envioDocumento = {
                destinatarios: correos,
                folio: this.documentoAEnviar.folio,
                codAux: this.compraDetalleAux.CodAux,
                tipoDoc: this.documentoAEnviar.tipo,
                subTipoDoc: this.documentoAEnviar.subTipo,
                tipoDocAEnviar: 1,
                docsAEnviar: tipoEnvio
            }


            this.clienteService.enviaDocumentoPDF(envioDocumento).subscribe((res: any) => {
                this.spinner.hide();
                this.otrosCorreo = [];
                this.contactosSeleccionados = [];
                this.enviaPdf = false;
                this.enviaXml = false;
                this.modalService.dismissAll();
                if (res == 0) {
                    this.notificationService.warning('No existen archivos asociados al documento.', '', true);
                } else {
                    this.notificationService.success('Documentos enviados correctamente.', '', true);
                }
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });
        }


    }


    desplegarDetallesDocumentos(cliente: any) {


        if (cliente.muestraDocumentos) {
            cliente.muestraDocumentos = false;
        } else {
            cliente.muestraDocumentos = true;
            this.spinner.show();

            let fHasta = null;
            if (this.dateHasta != null) {
                fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
            }

            let fDesde = null;
            if (this.dateDesde != null) {
                fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
            }

            let model = {
                TipoBusqueda: this.tipoDoc,
                fechaDesde: fDesde,
                fechaHasta: fHasta,
                Pagina: 1,
                CodAux: cliente.codaux,
                Folio: this.folio
            }



            this.clienteService.GetDocumentosContabilizadosXCliente(model).subscribe((res: any[]) => {
                debugger
                cliente.documentosPorPagina = res;
                cliente.paginadorDocumentos.startRow = 0;
                cliente.paginadorDocumentos.endRow = 10;
                cliente.paginadorDocumentos.sortBy = 'desc';
                cliente.configDocumentosCliente.itemsPerPage = cliente.paginadorDocumentos.endRow;
                cliente.configDocumentosCliente.currentPage = 1;
                cliente.configDocumentosCliente.totalItems = cliente.documentosPorPagina.length > 0 ? cliente.documentosPorPagina[0].totalFilas : 0;
                this.cliente = cliente;
                this.updateDocumentos = true
                this.updateDocumentosEvent.emit(cliente.documentosPorPagina);

                this.cdr.detectChanges();
                this.spinner.hide();
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });
        }

    }

    changePageDocumentosCliente(cliente: any, event: any) {

        this.spinner.show();

        let fHasta = null;
        if (this.dateHasta != null) {
            fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
        }

        let fDesde = null;
        if (this.dateDesde != null) {
            fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        }

        let model = {
            TipoBusqueda: this.tipoDoc,
            fechaDesde: fDesde,
            fechaHasta: fHasta,
            Pagina: event,
            CodAux: cliente.codaux,
            Folio: this.folio
        }

        this.clienteService.GetDocumentosContabilizadosXCliente(model).subscribe((res: any[]) => {
            cliente.documentosPorPagina = res;
            cliente.paginadorDocumentos.startRow = 0;
            cliente.paginadorDocumentos.endRow = 10;
            cliente.paginadorDocumentos.sortBy = 'desc';
            cliente.configDocumentosCliente.currentPage = event;

            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos', '', true); });


    }
}
