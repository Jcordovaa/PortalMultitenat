import { Component, NgModule, OnInit, ViewChild } from '@angular/core';
import { EChartOption } from 'echarts';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { echartStyles } from '../../../shared/echart-styles';
import { DashboardEcommerce, DashboardVentas } from '../../../shared/models/dashboards.model';
import { DashboardsService } from '../../../shared/services/dashboards.service';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NgxSpinnerService } from "ngx-spinner";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { Documento } from 'src/app/shared/models/documentos.model';
import { User } from '../../chat/chat.service';
import { Paginator } from 'src/app/shared/models/paginator.model';
import { SharedModule } from 'src/app/shared/shared.module';
import { formatDate } from '@angular/common';
import * as FileSaver from "file-saver";
import * as XLSX from 'xlsx';
import { date } from 'ngx-custom-validators/src/app/date/validator';
import { ConfiguracionPagoCliente, ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service'; //FCA 01-07-2022
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ResumenContable } from 'src/app/shared/models/resumencontable.model';
import { SoftlandService } from 'src/app/shared/services/softland.service';
import { Utils } from 'src/app/shared/utils';

interface AutoCompleteModel {
    value: any;
    display: string;
}


const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';


@Component({
    selector: 'app-dashboard-cliente',
    templateUrl: './dashboard-cliente.component.html',
    styleUrls: ['./dashboard-cliente.component.scss'],
    animations: [SharedAnimations]
})



export class DashboardClienteComponent implements OnInit {
    @ViewChild('tabset') tabset;
    chartLineOption1: EChartOption;
    chartLineOption2: EChartOption;
    chartLineOption3: EChartOption;
    salesChartBar: EChartOption;
    configuracionPagos: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
    chartPie1: any;
    detalleCab: any = null;
    detalleDet: any = [];
    showDetail: boolean = false;
    tituloDetalle: string = '';
    contactosCliente: any = [];
    otroCorreo: string = '';
    contactosSeleccionados: any = [];
    cantidadVencidos: number = 0;
    totalVencidos: number = 0;
    cantidadPorVencer: number = 0;
    totalPorVencer: number = 0;
    cantidadPendiente: number = 0;
    totalPendiente: number = 0;
    existComprasSegundaMoneda: boolean = false;
    estadoBloqueo: string = '';
    configuracion: ConfiguracionPortal = new ConfiguracionPortal(); //FCA 05-07-2022
    noResultText: string = '';
    resumenContable: ResumenContable = new ResumenContable(); //FCA 05-07-2022
    existModuloInventario: boolean = false;
    estadoDoc: string = '';
    selectedTabMonedas: number = 1;
    fechaActual = new Date();
    dateDesde: NgbDateStruct;
    dateHasta: NgbDateStruct;
    folio: number = null;
    documentoAEnviar: any = null;
    valorUfActrual: number = 0;
    varloUfOrigen: number = 0;
    public totalItems: number = 0;
    documentos: Documento[] = [];
    documentosOtrasMonedas: Documento[] = [];
    tipoDoc: number = 0;
    public configDocumentos: any;
    public otrosCorreo: AutoCompleteModel[] = [];
    public enviaPdf: boolean = false;
    public enviaXml: boolean = false;
    documentosPorPagina: Documento[] = [];
    documentosFiltro: Documento[] = [];


    public paginadorDocumentos: Paginator = {
        startRow: 0,
        endRow: 10,
        sortBy: 'desc',
        search: ''
    };

    public paginadorDocumentosOtrasMonedas: Paginator = {
        startRow: 0,
        endRow: 10,
        sortBy: 'desc',
        search: ''
    };


    dashboardEcommerceResult: DashboardEcommerce = new DashboardEcommerce();
    dashboardVentasResult: DashboardVentas = new DashboardVentas();

    public topCompras: any[] = [];
    public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno(); //FCA 01-07-2022
    compraDetalleAux = null;

    constructor(private dashboardsService: DashboardsService, private disenoSerivce: ConfiguracionDisenoService,//FCA 01-07-2022
        private clienteService: ClientesService, private softlandService: SoftlandService,
        private authService: AuthService,
        private spinner: NgxSpinnerService, private utils: Utils,
        private notificationService: NotificationService,
        private modalService: NgbModal, private configuracionService: ConfiguracionPagoClientesService) { }

    ngOnInit() {
        this.getDashboardEcommerce();
    }



    getDashboardEcommerce() {
        this.spinner.show();
        const user = this.authService.getuser();
        if (user != null) {
            const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
            if (configuracionCompletaPortal != null) {
                this.configDiseno = configuracionCompletaPortal.configuracionDiseno;

                this.configuracion = configuracionCompletaPortal.configuracionPortal;
                this.configuracionPagos = configuracionCompletaPortal.configuracionPagoCliente;
                this.existModuloInventario = configuracionCompletaPortal.existModuloInventario;
            }

            this.spinner.show();

            this.clienteService.getDashboardDocumentos(user.codAux).subscribe((res: any) => {
                this.cantidadVencidos = res.cantidadVencida;
                this.totalVencidos = res.montoVencido;
                this.cantidadPendiente = res.cantidadDocPendiente;
                this.totalPendiente = res.saldoPendiente;
                this.cantidadPorVencer = res.cantidadxVencer;
                this.totalPorVencer = res.saldoxvencer;
                this.spinner.hide();
            }, err => { this.spinner.hide(); });

            this.clienteService.getTopCompras(user.codAux).subscribe((res: any) => {
                this.topCompras = res;
                if (this.topCompras.length > 0) {
                    this.configDiseno.tituloUltimasCompras = this.configDiseno.tituloUltimasCompras.replace('{cantidad}', this.topCompras.length.toString())
                }

            }, err => { this.spinner.hide(); });


        } else {
            this.authService.signoutExpiredToken();
        }

    }

    verDetalle(compra: any) {
        var user = this.authService.getuser();
        var obj = { Folio: 0, TipoDoc: '', CodAux: '' };
        this.estadoDoc = compra.estado == 'V' ? 'VENCIDO' : 'PENDIENTE';
        if (user != null) {

            // let vm = JSON.parse(user);
            obj.Folio = compra.folio;
            obj.CodAux = user.rut;
            if (compra.folio == null || compra.folio == undefined) {
                obj.Folio = compra.nro;
            } else {
                obj.Folio = compra.folio;
            }

            if (compra.tipoDoc == null || compra.tipoDoc == undefined) {
                obj.TipoDoc = compra.tipo;
            } else {
                obj.TipoDoc = compra.tipoDoc;
            }

            obj.CodAux = user.codAux;

            this.compraDetalleAux = obj;
            this.spinner.show();

            this.clienteService.getDetalleCompra(obj).subscribe((res: any) => {

                this.spinner.hide();
                if (res.cabecera != null) {
                    if (res.cabecera.folio != 0) { //FCA 05-07-2022               
                        this.detalleCab = res.cabecera; //FCA 05-07-2022
                        this.detalleDet = res.detalle;
                        this.showDetail = true;
                        this.tituloDetalle = compra.documento;
                    } else {
                        this.notificationService.info('Documento no posee detalle asociado.', '', true);
                    }
                } else {
                    this.notificationService.info('Documento no posee detalle asociado.', '', true);
                }

            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
        } else {
            this.authService.signoutExpiredToken();
        }
    }


    volver() {
        this.detalleCab = null;
        this.detalleDet = [];
        this.showDetail = false;
    }

    print() {
        if (window) {
            window.print();
            // this.clienteService.getdocumentoPDF(this.detalleCab).subscribe(res => {
            //     let a = document.createElement("a");
            //     a.href = "data:application/octet-stream;base64," + res;
            //     a.download = "documentName.pdf"
            //     a.click();
            // }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });

        }
    }

    openModalSend(content, doc: any) {
        this.spinner.show();
        this.contactosCliente = [];
        this.documentoAEnviar = doc;
        this.enviaPdf = false;
        this.enviaXml = false;
        this.otroCorreo = ''; //FCA 05-07-2022
        var user = this.authService.getuser();
        if (user != null) {
            // let vm = JSON.parse(user);
            const rut: string[] = user.rut.split('-');
            const codAux: string = rut[0].replace('.', '').replace('.', '')

            this.clienteService.getContactosClienteSoftland(codAux).subscribe(res => {
                this.contactosCliente = res;
                this.spinner.hide();
                this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
        } else {
            this.authService.signoutExpiredToken();
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


    obtenerDocumentos(tipo: number) {


        const user = this.authService.getuser();
        switch (tipo) {
            case 1:
                if (this.cantidadPendiente == 0) {
                    this.notificationService.warning('No existen resultados para Documentos Pendientes', '', true);
                    return;
                }

                this.noResultText = "No existen resultados para Documentos Pendientes";
                this.dateDesde = null;
                this.dateHasta = null;
                this.folio = null;
                this.tipoDoc = tipo;
                this.paginadorDocumentos.startRow = 0;
                this.paginadorDocumentos.endRow = 10;
                this.paginadorDocumentos.sortBy = 'desc';
                this.paginadorDocumentos.search = '';

                if (user != null) {
                    this.spinner.show();
                    let model = {
                        CodAux: user.codAux,
                        Pagina: 1
                    }
                    this.clienteService.getDocumentosPendientes(model).subscribe((res: Documento[]) => {
                        this.tabset.select(1);
                        this.documentos = res;
                        this.documentosPorPagina = res.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                        this.documentosFiltro = res;
                        this.configDocumentos = {
                            itemsPerPage: this.paginadorDocumentos.endRow,
                            currentPage: 1,
                            totalItems: this.documentos.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
                        };

                        let compraSsegundaMoneda = this.documentos.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);

                        if (compraSsegundaMoneda.length > 0) {
                            this.existComprasSegundaMoneda = true;
                        }
                        this.spinner.hide();
                    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos pendientes', '', true); });
                } else {
                    this.authService.signoutExpiredToken();
                }
                break;

            case 2:
                if (this.cantidadVencidos == 0) {
                    this.notificationService.warning('No existen resultados para Documentos Vencidos', '', true);
                    return;
                }

                this.noResultText = "No existen resultados para Documentos Vencidos";
                this.dateDesde = null;
                this.dateHasta = null;
                this.folio = null;
                this.tipoDoc = tipo;
                this.paginadorDocumentos.startRow = 0;
                this.paginadorDocumentos.endRow = 10;
                this.paginadorDocumentos.sortBy = 'desc';
                this.paginadorDocumentos.search = '';

                if (user != null) {
                    this.spinner.show();
                    let model = {
                        CodAux: user.codAux,
                        Pagina: 1
                    }
                    this.clienteService.getDocumentosVencidos(model).subscribe((res: Documento[]) => {
                        this.tabset.select(1);
                        this.documentos = res;
                        this.documentosPorPagina = res.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                        this.documentosFiltro = res;
                        this.configDocumentos = {
                            itemsPerPage: this.paginadorDocumentos.endRow,
                            currentPage: 1,
                            totalItems: this.documentos.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
                        };
                        let compraSsegundaMoneda = this.documentos.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);

                        if (compraSsegundaMoneda.length > 0) {
                            this.existComprasSegundaMoneda = true;
                        }
                        this.spinner.hide();
                    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos vencidos', '', true); });
                } else {
                    this.authService.signoutExpiredToken();
                }
                break;

            case 3:
                if (this.cantidadPorVencer == 0) {
                    this.notificationService.warning('No existen resultados para Documentos Por Vencer', '', true);
                    return;
                }

                this.noResultText = "No existen resultados para Documentos Por Vencer"
                this.dateDesde = null;
                this.dateHasta = null;
                this.folio = null;
                this.tipoDoc = tipo;
                this.paginadorDocumentos.startRow = 0;
                this.paginadorDocumentos.endRow = 10;
                this.paginadorDocumentos.sortBy = 'desc';
                this.paginadorDocumentos.search = '';

                if (user != null) {
                    this.spinner.show();
                    let model = {
                        CodAux: user.codAux,
                        Pagina: 1
                    }
                    this.clienteService.getDocumentosPorVencer(model).subscribe((res: Documento[]) => {
                        this.tabset.select(1);
                        this.documentos = res;
                        this.documentosPorPagina = res.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                        this.documentosFiltro = res;
                        this.configDocumentos = {
                            itemsPerPage: this.paginadorDocumentos.endRow,
                            currentPage: 1,
                            totalItems: this.documentos.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
                        };

                        let compraSsegundaMoneda = this.documentos.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);

                        if (compraSsegundaMoneda.length > 0) {
                            this.existComprasSegundaMoneda = true;
                        }
                        this.spinner.hide();
                    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documentos por vencer', '', true); });
                } else {
                    this.authService.signoutExpiredToken();
                }
                break;


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
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;
        this.otroCorreo = '';
    }


    volverDetalle() {
        this.showDetail = false;
    }

    changePageDocumentos(event: any, tipo: number) {
        this.paginadorDocumentos.startRow = ((event - 1) * 10);
        this.paginadorDocumentos.endRow = (event * 10);
        this.paginadorDocumentos.sortBy = 'desc';
        this.configDocumentos.currentPage = event;

        switch (tipo) {
            case 1:
                this.documentosPorPagina = this.documentos.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                break;

            case 2:
                this.documentosPorPagina = this.documentos.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada).slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);
                break;
        }
    }



    filter(moneda: number) {
        this.documentosFiltro = this.documentos;

        if (moneda == 1) {
            this.documentosFiltro = this.documentosFiltro.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada);
        } else if (moneda == 2) {
            this.documentosFiltro = this.documentosFiltro.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
        }

        if (this.folio != null) {
            this.documentosFiltro = this.documentosFiltro.filter(x => x.nro == this.folio)
        }
        if (this.dateDesde != null) {
            const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
            this.documentosFiltro = this.documentosFiltro.filter(x => new Date(x.fechaEmision) >= fDesde)
        }
        if (this.dateHasta != null) {
            const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
            this.documentosFiltro = this.documentosFiltro.filter(x => new Date(x.fechaEmision) <= fHasta)
        }
        this.paginadorDocumentos.startRow = 0;
        this.paginadorDocumentos.endRow = 10;
        this.paginadorDocumentos.sortBy = 'desc';
        this.configDocumentos = {
            itemsPerPage: this.paginadorDocumentos.endRow,
            currentPage: 1,
            totalItems: this.documentosFiltro.length
        };

        this.documentosPorPagina = this.documentosFiltro.slice(this.paginadorDocumentos.startRow, this.paginadorDocumentos.endRow);

        // this.dateDesde = null;
        // this.dateHasta = null;
        // this.folio = null;
    }

    limpiarFiltros() {
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;

        this.filter(this.selectedTabMonedas);
    }


    export() {
        this.documentosFiltro = this.documentos;

        if (this.folio != null) {
            this.documentosFiltro = this.documentosFiltro.filter(x => x.nro == this.folio)
        }
        if (this.dateDesde != null) {
            const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
            this.documentosFiltro = this.documentosFiltro.filter(x => new Date(x.fechaEmision) >= fDesde)
        }
        if (this.dateHasta != null) {
            const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
            this.documentosFiltro = this.documentosFiltro.filter(x => new Date(x.fechaEmision) <= fHasta)
        }

        var listaExportacion = [];
        var fechavto;
        this.documentosFiltro.forEach((element) => {
            if (element.fechaVcto == null || element.fechaVcto == undefined) {
                fechavto = '';
            } else {
                fechavto = new Date(element.fechaVcto);
                fechavto = fechavto.getDate() + "/" + (fechavto.getMonth() + 1) + "/" + fechavto.getFullYear();
            }

            var fechaEmision;
            fechaEmision = new Date(element.fechaEmision);
            fechaEmision = fechaEmision.getDate() + "/" + (fechaEmision.getMonth() + 1) + "/" + fechaEmision.getFullYear();
            if (this.existComprasSegundaMoneda) {
                const model = { Tipo: element.documento, Folio: element.nro, Emision: fechaEmision, Vencimiento: fechavto, Moneda: element.desMon, Monto_Moneda_Origen: element.debe, Monto_Moneda_Nacional_Emision: element.montoOriginalBase, Monto_Mda_Nacional_Actualizado: element.montoBase, Saldo_En_Mda_Nacional: element.saldoBase, Estado: element.estado };
                listaExportacion.push(model);
            } else {
                const model = { Tipo: element.documento, Folio: element.nro, Emision: fechaEmision, Vencimiento: fechavto, Moneda: element.desMon, Monto: element.debe, Saldo: element.saldo, Estado: element.estado };
                listaExportacion.push(model);
            }
        });
        var nombre;
        if (this.tipoDoc == 1) {
            nombre = "Documentos Pendientes";
        }
        if (this.tipoDoc == 2) {
            nombre = "Documentos Vencidos";
        }
        if (this.tipoDoc == 3) {
            nombre = "Documentos por Vencer";
        }
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

    //FCA 01-07-2022
    private getConfigDiseno() {

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
            this.notificationService.warning('Debe seleccionar al menos un tipo de documento.', '', true);
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

    private getResumenContable() {

        const user = this.authService.getuser();
        if (user != null) {

        }
    }


    changeTabDocumentos(event: any) {
        this.selectedTabMonedas = event.nextId;
        switch (event.nextId) {
            case 1:
                this.paginadorDocumentos.startRow = 0;
                this.paginadorDocumentos.endRow = 10;
                this.paginadorDocumentos.sortBy = 'desc';
                this.paginadorDocumentos.search = '';
                this.configDocumentos = {
                    itemsPerPage: this.paginadorDocumentos.endRow,
                    currentPage: 1,
                    totalItems: this.documentos.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
                };
                this.limpiarFiltros();

                break;

            case 2:
                this.paginadorDocumentos.startRow = 0;
                this.paginadorDocumentos.endRow = 10;
                this.paginadorDocumentos.sortBy = 'desc';
                this.paginadorDocumentos.search = '';
                this.configDocumentos = {
                    itemsPerPage: this.paginadorDocumentos.endRow,
                    currentPage: 1,
                    totalItems: this.documentos.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada).length
                };
                this.limpiarFiltros();
                break;
        }
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
            obj.TipoDoc = this.documentoAEnviar.tipo + this.documentoAEnviar.subTipo;
            obj.CodAux = this.compraDetalleAux.CodAux;





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
        } else {
            this.authService.signoutExpiredToken();
        }

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
                obj.TipoDoc = this.documentoAEnviar.tipo + this.documentoAEnviar.subTipo;
            }
            obj.CodAux = this.compraDetalleAux.CodAux;

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
        } else {
            this.authService.signoutExpiredToken();
        }

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
}
