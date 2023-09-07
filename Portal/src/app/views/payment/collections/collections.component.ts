
import { NotificationService } from '../../../shared/services/notificacion.service';
import { AuthService } from '../../../shared/services/auth.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { CobranzasService } from 'src/app/shared/services/cobranzas.service';
import { NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker, NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Utils } from '../../../shared/utils';
import { Paginator } from 'src/app/shared/models/paginator.model';
import { Cobranza, CobranzaDetalle, CobranzaPeriocidad, EstadoCobranza } from 'src/app/shared/models/cobranzas.model';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { echartStyles } from 'src/app/shared/echart-styles';
import { number } from 'ngx-custom-validators/src/app/number/validator';
import { DatePipe } from '@angular/common';
import { MailService } from 'src/app/shared/services/mail.service';
import { TiposDocumentoService } from 'src/app/shared/services/tipodocumento.service';
import { SoftlandService } from 'src/app/shared/services/softland.service';
import { CategoriaClienteDTO, CondicionVentaDTO, ListaPrecioDTO, TipoDocumento, VendedorDTO } from 'src/app/shared/models/softland.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { Cobrador } from 'src/app/shared/models/cobrador.model';
import { CanalVenta } from 'src/app/shared/models/canalventa.model';
import { ClientesService } from 'src/app/shared/services/clientes.service';






const I18N_VALUES = {
  en: {
    weekdays: ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'],
    months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  },
  es: {
    weekdays: ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'],
    months: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
  }
};



@Component({
  selector: 'app-collections',
  templateUrl: './collections.component.html',
  styleUrls: ['./collections.component.scss'],
  animations: [SharedAnimations],
  //FCA 22-11-2021 Se agrega datepipe
  providers: [DatePipe]
})
export class CollectionsComponent implements OnInit {
  //CAMPOS NUEVOS

  searchRutDetalle: string = '';
  public muestraDetalleClientes: boolean = false; //FCA 07-12-2021
  public img: string = 'assets/images/Icono-Listado-Cobranzas.png'; //OK
  public img2: string = 'assets/images/Icono-Listado-Cobranzas.png';
  public nuevaCobranza: boolean = false;
  public muestraCabecera: boolean = false;
  public chkEstadoNuevo: boolean = true;
  public textoEstado: string = 'Inactivo'
  public tiposCobranza: any = [];
  public modalidadCobranza: any = [{ id: 1, modalidad: "Clásica" }, { id: 2, modalidad: "Inteligente" }]; //FCA 13-12-2021
  public selectedModalidadCobranza: any = 1; //FCA 13-12-2021
  public selectedTipoCobranza: number = null;
  public chkClientesExcluidos: boolean = true;
  public chkFeriados: boolean = false;
  public chkFines: boolean = false;
  public fecha: NgbDateStruct;
  public fechaVencimiento: NgbDateStruct;
  public horarios: any = [];
  public selectedProgramacion: number = 1;
  public frecuencias: any = [{ id: 1, frecuencia: "SEMANAL" }, { id: 2, frecuencia: "MENSUAL" }];
  public clientesSeleccionados: any[] = [];
  public clientesEliminados: any[] = [];
  public selectedFrecuencia: number = null;
  public frecuenciaDias: any = [{ id: 1, dia: "LUNES" }, { id: 2, dia: "MARTES" }, { id: 3, dia: "MIÉRCOLES" }, { id: 4, dia: "JUEVES" }, { id: 5, dia: "VIERNES" }, { id: 6, dia: "SÁBADO" }, { id: 7, dia: "DOMINGO" }];
  public selectedHorario: number = null;
  public selectedDia: any;
  public selectedPeriocidad: number = null;
  public muestraDetalleDocumentos: boolean = false;
  public dateDesde: NgbDateStruct;
  public fechaInicioMin = new Date().setDate(new Date().getDate() + 1);
  public dateHasta: NgbDateStruct;
  public dateDesdeFiltro: NgbDateStruct;
  public dateHastaFiltro: NgbDateStruct;
  public enviaTodosCargos: boolean = false;
  public totalClientes: number = 0;
  public clienteNombre: string = '';
  public clienteRut: string = '';
  checkAll: boolean = false;
  //FCA 13-12-2021
  public montoDesdeFiltro: number = null;
  public montoHastaFiltro: number = null;
  abonos: any = null;

  public nombreCobranza: string = null;
  public paso: number = 0;
  public anios: any = [];
  public tiposDocumento: TipoDocumento[] = [];
  public tiposDocumentoApoderado: any = [];
  public selectedTipoDoc: any = [];
  public selectedAnio: any = 'TODOS';
  public cantidadDocumentosFiltro: number = 0;
  public diasVencimiento: number = 0;
  public documentos: any = [];
  public searchRut: string = null;
  public searchCodAux: string = null;
  public searchNombre: string = null;
  public selectedEstadoCorreo: string = null;
  public estadosCorreo: any = [{ id: 1, nombre: "SIN CORREO ASIGNADO" }, { id: 2, nombre: "CORREO ASIGNADO" }];
  public detalleClientes: any = [];
  public documentosEliminados: any[] = [];
  public detalleClientePag: any = [];
  public detalleClienteFiltro: any = [];
  public selected: any = [];
  public detalleDocumentos: any;
  public nuevoCorreo: string = null;
  public cantidadCorreos: number = 0;
  public tituloFinal: string = '';
  public selectedClientes: any = [];
  public idUsuario: number = 0;
  public correosDisponibles: number = 0;
  public correosSeleccionados: number = 0
  public totalPagar: number = 0;
  public enviaLinkPago: number = null;
  public textoCantidad: string = "Cantidad días posterior al vencimiento";
  public selectedTipoFiltro: number = null;
  public selectedEstadoFiltro: number = null;
  public nombreFiltro: string = null;
  public muestraFiltroCobranza: boolean = false;
  public textoBotonFiltro: string = 'Mostrar Filtros';
  public estadosCobranza: any = [];
  public totalItems: number = 0;
  public viewMode: 'list' | 'grid' = 'list';
  public listaCobranzas: Cobranza[] = [];
  public listaCobranzasRes: Cobranza[] = [];
  public cobranzaDatos: Cobranza;
  public cobranzaDetalle: CobranzaDetalle[] = [];
  public cobranzaDetalleRes: CobranzaDetalle[] = []; //FCA 22-11-2021
  public muestraDetalle: boolean = false;
  public periocidades: CobranzaPeriocidad[] = [];
  public diaEnvio: number = null;
  public selectedCurso: number = null;
  public selectedArancel: number = null;
  public deudaDesde: number = 0;
  public deudaHata: number = 0;
  public estadosDetalle: any[] = [{ id: 0, descripcion: 'TODOS' }, { id: 1, descripcion: 'PENDIENTE' }, { id: 3, descripcion: 'ENVIADA' }, { id: 4, descripcion: 'PARCIALMENTE PAGADA' }, { id: 5, descripcion: 'PAGADA' }];
  public filtroFolioDetalle: any = '';
  public selectedEstadoDetalle: any = null;
  public condicionesVenta: CondicionVentaDTO[] = [];
  public listasPrecio: ListaPrecioDTO[] = [];
  public categoriasCliente: CategoriaClienteDTO[] = [];
  public vendedores: VendedorDTO[] = [];
  public cobradores: Cobrador[] = [];
  public canalesVenta: CanalVenta[] = [];

  public estadosClientes: any[] = [{ id: 'TODOS', descripcion: 'TODOS' }, { id: 'S', descripcion: 'Bloqueado' }, { id: 'N', descripcion: 'No Bloqueado' }];
  selectedEstadoCliente: any = 'TODOS';
  selectedCondicionVenta: any[] = [];
  selectedListaPrecio: any[] = [];
  selectedCatCliente: any[] = [];
  selectedVendedor: any[] = [];
  selectedCargosContactos: any[] = [];
  selectedCanalesVenta: any[] = [];
  selectedCobradores: any[] = [];
  enviaTodosContactos: boolean = false;
  enviaCorreoFicha: boolean = false;
  cargos: any = [];
  mensajeFechaInicioErronea: string = '';
  mensajeFechaTerminoErronea: string = '';

  noResultsText: string = 'Sin cobranzas creadas';
  mensajeInfo: string = '';
  public config: any;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };


  //FCA 22-11-2021 
  public totalItemsDetalle: number = 0;
  public configDetalle: any = {
    itemsPerPage: 0,
    currentPage: 1,
    totalItems: 0
  };
  public paginadorDetalle: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  chartMonto: any;
  chartDocumentos: any;



  constructor(
    private cobranzaService: CobranzasService,
    private ngbDatepickerConfig: NgbDatepickerConfig,
    private ngbDatepickerI18n: NgbDatepickerI18n,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private mailService: MailService,
    private utils: Utils,
    private authService: AuthService,
    private modalService: NgbModal, private softlandService: SoftlandService, private configSoftlandService: ConfiguracionSoftlandService, private clientesService: ClientesService,
    private miDatePipe: DatePipe //FCA 22-11-2021 Se agrega datepipe
  ) {

    this.ngbDatepickerConfig.firstDayOfWeek = 1;
    this.ngbDatepickerConfig.minDate = { day: 1, month: 1, year: 1950 };

    this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
      return I18N_VALUES['es'].weekdays[weekday - 1];
    };

    this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
      return I18N_VALUES['es'].months[months - 1];
    };
  }

  ngOnInit() {

    const currentDate = new Date();
    this.fecha = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() + 1 };
    const user = this.authService.getuser();
    if (user) {
      debugger
      this.idUsuario = user.idUsuario;
    }

    this.cobranzaService.getTiposCobranza().subscribe(res => {
      this.tiposCobranza = res;

      this.cobranzaService.getEstadoCobranza().subscribe(resc => {
        this.estadosCobranza = resc;
        this.cobranzaService.getCobranzaPeriocidad().subscribe((resp: any) => {
          this.periocidades = resp;
          this.searchCobranzaClasica();
          this.getCategoriasCliente();
          //this.getListasPrecio();
          this.getCondicionesVenta();
          this.getVendedores();
          this.getTipoDocumentosCobranza();
          this.getcargos();
          // this.getCanalesVenta();
          // this.getCobradores();
        }, err => {
          this.spinner.hide();
        });
      }, err => {
        this.spinner.hide();
      });
    }, err => {
      this.spinner.hide();
    });
  }

  getcargos() {
    this.configSoftlandService.getCargos().subscribe((res: any) => {
      this.cargos = res;
      this.cargos.forEach(element => {
        element.carNom = element.carCod + ' - ' + element.carNom;
      });
      if (this.cargos.length == 0) {
        this.cargos.push({ carNom: 'Sin Datos', carCod: '', disabled: true })
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar cargos', '', true); });
  }

  getCanalesVenta() {
    this.softlandService.getCanalesVenta().subscribe((res: CanalVenta[]) => {
      this.canalesVenta = res;
      this.canalesVenta.forEach(element => {
        element.canDes = element.canCod + ' - ' + element.canDes;
      });
      if (this.canalesVenta.length == 0) {
        this.canalesVenta.push({ canDes: 'Sin Datos', canCod: '', disabled: true })
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar canales de venta', '', true); });
  }

  getCobradores() {
    this.softlandService.getCobradores().subscribe((res: Cobrador[]) => {
      this.cobradores = res;
      this.cobradores.forEach(element => {
        element.cobDes = element.cobCod + ' - ' + element.cobDes;
      });
      if (this.cobradores.length == 0) {
        this.cobradores.push({ cobDes: 'Sin Datos', cobCod: '', disabled: true })
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar canales de venta', '', true); });
  }

  getCategoriasCliente() {
    this.spinner.show();
    this.softlandService.getCategoriasCliente().subscribe((resp: CategoriaClienteDTO[]) => {
      this.categoriasCliente = resp;
      this.categoriasCliente.forEach(element => {
        element.catDes = element.catCod + ' - ' + element.catDes;
      });
      if (this.categoriasCliente.length == 0) {
        this.categoriasCliente.push({ catDes: 'Sin Datos', catCod: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  getTipoDocumentosCobranza() {
    this.spinner.show();
    this.cobranzaService.getTipoDocumentosPagos().subscribe((resp: TipoDocumento[]) => {
      this.tiposDocumento = resp;
      this.tiposDocumento.forEach(element => {
        element.desDoc = element.codDoc + ' - ' + element.desDoc;
      });
      if (this.tiposDocumento.length == 0) {
        this.tiposDocumento.push({ desDoc: 'Sin Datos', codDoc: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  getListasPrecio() {
    this.spinner.show();
    this.softlandService.getListasPrecio().subscribe((resp: ListaPrecioDTO[]) => {

      this.listasPrecio = resp;

      this.listasPrecio.forEach(element => {
        element.desLista = element.codLista + ' - ' + element.desLista;
      });
      if (this.listasPrecio.length == 0) {
        this.listasPrecio.push({ desLista: 'Sin Datos', codLista: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  getVendedores() {
    this.spinner.show();
    this.softlandService.getVendedores().subscribe((resp: VendedorDTO[]) => {
      this.vendedores = resp;
      this.vendedores.forEach(element => {
        element.venDes = element.venCod + ' - ' + element.venDes;
      });
      if (this.vendedores.length == 0) {
        this.vendedores.push({ venDes: 'Sin Datos', venCod: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  getCondicionesVenta() {
    this.spinner.show();
    this.softlandService.getCondicionesVenta().subscribe((resp: CondicionVentaDTO[]) => {
      this.condicionesVenta = resp;
      this.condicionesVenta.forEach(element => {
        element.cveDes = element.cveCod + ' - ' + element.cveDes;
      });
      if (this.condicionesVenta.length == 0) {
        this.condicionesVenta.push({ cveDes: 'Sin Datos', cveCod: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  //FCA 13-12-2021
  changeModalidadCobranza() {
    this.cleanFiltersCobranzas();
    this.searchCobranzaClasica();
  }
  searchCobranzaClasica() {
    var fechaDesde = new Date();
    var fechaHasta = new Date();

    if (this.dateDesdeFiltro != null) {
      const fDesde = new Date(this.dateDesdeFiltro.year, this.dateDesdeFiltro.month - 1, this.dateDesdeFiltro.day, 0, 0, 0);
      fechaDesde = fDesde;
    } else { fechaDesde = null; }

    if (this.dateHastaFiltro != null) {
      const fHasta = new Date(this.dateHastaFiltro.year, this.dateHastaFiltro.month - 1, this.dateHastaFiltro.day, 0, 0, 0);
      fechaHasta = fHasta;
    } else { fechaHasta = null; }

    this.spinner.show();
    const model = { IdTipoCobranza: this.selectedTipoFiltro, NombreCobranza: this.nombreFiltro, IdEstadoCobranza: this.selectedEstadoFiltro, FechaHasta: fechaHasta, FechaDesde: fechaDesde, TipoProgramacion: this.selectedModalidadCobranza } //FCA 13-12-2021
    this.cobranzaService.getCobranzasTipo(model).subscribe((res: any) => {
      this.listaCobranzasRes = res;
      this.paginador.startRow = 0;
      this.paginador.endRow = 10;
      this.paginador.sortBy = 'desc';
      this.paginador.search = '';
      this.config = {
        itemsPerPage: this.paginador.endRow,
        currentPage: 1,
        totalItems: this.listaCobranzasRes.length
      }
      this.totalItems = this.listaCobranzasRes.length;
      this.listaCobranzas = this.listaCobranzasRes.slice(this.paginador.startRow, this.paginador.endRow);

      this.spinner.hide();
    }, err => { });

  }

  changePage(event: any){
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    this.listaCobranzas = this.listaCobranzasRes.slice(this.paginador.startRow, this.paginador.endRow);
  }

  //METODOS NUEVOS COBRANZA SE MANTENDRAN
  crearCobranza() {
    this.spinner.show()
    this.limpiar();
    this.chkClientesExcluidos = true;
    this.nuevaCobranza = true;
    this.muestraCabecera = true;

    this.cobranzaService.getHorasEnvio().subscribe((res: any) => {
      // const fechaPropuesta = new Date(res.fecha);
      // this.fecha = { year: fechaPropuesta.getFullYear(), month: fechaPropuesta.getMonth() + 1, day: fechaPropuesta.getDate() };
      this.horarios = res.horarios;
      this.spinner.hide();
    }, err => { });


  }

  onChangeNuevoEstado() {
    if (this.chkEstadoNuevo) {
      this.textoEstado = 'Activo'
    } else {
      this.textoEstado = 'Inactivo'
    }
  }

  changeTipoCobranza(p: any, e: any) {
    if (e.srcElement.checked) {
      this.selectedTipoCobranza = p.idTipoCobranza;

      this.tiposCobranza.forEach(element => {
        if (element.idTipoCobranza != p.idTipoCobranza) {
          let input = document.getElementById("cb" + element.idTipoCobranza) as HTMLInputElement;
          input.checked = false;
        }
      });

      if (this.selectedTipoCobranza == 1) {
        this.textoCantidad = "Cantidad días posterior al vencimiento";
      } else if (this.selectedTipoCobranza == 2) {
        this.textoCantidad = "Cantidad de días previo al vencimiento";
      }

    } else {
      this.selectedTipoCobranza = 0;
    }
  }

  get IsStepOneOk() {

    if (this.selectedTipoCobranza == null || this.selectedProgramacion == null) {
      return false
    }

    if (this.selectedProgramacion == 1) //Clasica
    {
      if (this.fecha == null || this.selectedHorario == null) {
        return false;
      }
    }

    if (this.selectedProgramacion == 2) //Inteligente
    {
      if (this.selectedFrecuencia == null) {
        return false;
      }

      if (this.selectedFrecuencia == 1) //Semanalmente
      {
        if (this.selectedDia == null || this.selectedHorario == null) {
          return false;
        }
      } else if (this.selectedFrecuencia == 2) //Mensualmente
      {

      }
    }





    const currentDate = new Date();
    var fechaActual = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() };

    let fechaInicio: string = this.fecha.year.toString() + '/' + this.fecha.month.toString() + '/' + this.fecha.day.toString();
    let actual: string = fechaActual.year.toString() + '/' + fechaActual.month.toString() + '/' + fechaActual.day.toString();


    if (Date.parse(actual) >= Date.parse(fechaInicio)) {
      return false;
    }

    if (this.fechaVencimiento != null) {
      let val2: string = this.fechaVencimiento.year.toString() + '/' + this.fechaVencimiento.month.toString() + '/' + this.fechaVencimiento.day.toString();
      if (Date.parse(val2) <= Date.parse(fechaInicio)) {
        return false;
      }

    }

    return true
  }

  get IsStepTwoOk() {

    if (this.selectedTipoDoc == null || this.selectedTipoDoc == '') {
      return false;
    }
    return true
  }

  get IsStepFinalOk() {
    if (this.nombreCobranza == null || this.nombreCobranza == '') {
      return false
    }

    if (this.selectedCargosContactos.length == 0 && !this.enviaTodosCargos) {
      return false;
    }
    return true
  }

  onStep1Next(e) {
    this.paso = 1;
  }


  onStep2Next(e) {
    this.paso = 2;
    this.muestraDetalleClientes = false; //FCA 07-12-2021
  }



  onStep3Next(e) {
    this.paso = 3;
    this.muestraDetalleClientes = false; //FCA 07-12-2021
  }


  async onComplete(e) {


    if (this.paso == 4) {

      // const response = await this.notificationService.confirmation('Ingresar de cobranza', '¿Confirma generar la cobranza?');
      // if (response.isConfirmed) {

      this.spinner.show();


      let tiposDocs = this.selectedTipoDoc != null ? this.selectedTipoDoc.length > 0 ? this.selectedTipoDoc.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;


      let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let listaPrecio = this.selectedListaPrecio != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let catCli = this.selectedCatCliente != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let vendedor = this.selectedVendedor != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;


      let cargosContactos = null;
      if (this.enviaTodosCargos) {
        cargosContactos = this.cargos != null ? this.cargos.length > 0 ? this.cargos.reduce((accumulator, item) => {
          return `${accumulator};${item.carCod}`;
        }) : null : null;
      } else {

        cargosContactos = this.selectedCargosContactos != null ? this.selectedCargosContactos.length > 0 ? this.selectedCargosContactos.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null : null;

      }


      let canalesVenta = this.selectedCanalesVenta != null ? this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let cobradores = this.selectedCobradores != null ? this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      var fechaDesde = new Date();
      var fechaHasta = new Date();

      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        fechaDesde = fDesde;
      } else { fechaDesde = null; }

      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
        fechaHasta = fHasta;
      } else { fechaHasta = null; }

      var estadoTipoCobranza = '';
      if (this.selectedTipoCobranza == 1) {
        estadoTipoCobranza = "VENCIDO";
      } else {
        estadoTipoCobranza = "PENDIENTE";
      }

      var excluye = 0;
      if (this.chkClientesExcluidos) { excluye = 1; }

      let SelAnio = 0;
      if (this.selectedAnio == 'TODOS') {
        SelAnio = 0;
      } else {
        SelAnio = this.selectedAnio;
      }



      let cobranza: Cobranza = new Cobranza();

      //Cabecera
      cobranza.idCobranza = 0;
      cobranza.nombre = this.nombreCobranza;
      cobranza.idTipoCobranza = this.selectedTipoCobranza;
      cobranza.estado = (this.chkEstadoNuevo) ? 1 : 0;
      cobranza.idUsuario = this.idUsuario;
      cobranza.tipoProgramacion = this.selectedProgramacion;
      cobranza.fechaInicio = new Date(this.fecha.year, this.fecha.month - 1, this.fecha.day);
      if (this.fechaVencimiento != null) {
        cobranza.fechaFin = new Date(this.fechaVencimiento.year, this.fechaVencimiento.month - 1, this.fechaVencimiento.day);
      } else {
        cobranza.fechaFin = null;
      }

      cobranza.horaDeEnvio = this.selectedHorario;
      //cobranza.diaSemanaEnvio para cobranza programada
      cobranza.diasToleranciaVencimiento = this.diasVencimiento;
      cobranza.idEstado = 1; //Estado Pendiente


      cobranza.anio = SelAnio;
      cobranza.tipoDocumento = tiposDocs;
      cobranza.vendedor = vendedor;
      cobranza.categoriaCliente = catCli;
      cobranza.listaPrecio = listaPrecio;
      cobranza.condicionVenta = condVenta;
      cobranza.cargosContactos = cargosContactos;
      cobranza.enviaTodosContactos = this.enviaTodosContactos ? 1 : 0;
      cobranza.enviaCorreoFicha = this.enviaCorreoFicha ? 1 : 0;
      cobranza.enviaTodosCargos = this.enviaTodosCargos ? 1 : 0;
      cobranza.cobradores = cobradores;
      cobranza.canalesVenta = canalesVenta;

      if (this.dateDesde != null) {
        cobranza.fechaDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day);
      } else {
        cobranza.fechaDesde = null;
      }
      if (this.dateHasta != null) {
        cobranza.fechaHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day);
      } else {
        cobranza.fechaHasta = null;
      }

      cobranza.aplicaClientesExcluidos = (this.chkClientesExcluidos) ? 1 : 0;
      cobranza.esCabeceraInteligente = (this.selectedProgramacion == 2) ? 1 : 0;
      cobranza.enviaEnlacePago = 1;

      //FCA 10-12-2021
      if (this.selectedFrecuencia == 1) {
        cobranza.idPeriodicidad = 1;
      } else if (this.selectedFrecuencia == 2) {
        cobranza.idPeriodicidad = this.selectedPeriocidad + 1; //FCA 07-12-2021
      }

      //FCA 09-12-2021
      if (this.chkFeriados) {
        cobranza.excluyeFeriados = 1;
      } else {
        cobranza.excluyeFeriados = 0;
      }

      if (this.chkFines) {
        cobranza.excluyeFinDeSemana = 1;
      } else {
        cobranza.excluyeFinDeSemana = 0;
      }
      let dias: string = '';
      if (this.selectedDia != null) {
        this.selectedDia.forEach(element => {
          if (dias == '') {
            dias = element.toString();
          } else {
            dias = dias + ';' + element.toString();
          }
        });
      }

      cobranza.diaSemanaEnvio = dias;
      cobranza.diaEnvio = this.diaEnvio;

      const model = {
        tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
        excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta, canalesVenta: canalesVenta,
        cobradores: cobradores, pagina: 1, cantidad: 20,
        documentosEliminados: this.documentosEliminados,
        clientesEliminados: this.clientesEliminados,
        clientesSeleccionados: this.clientesSeleccionados,
        cobranzaCabecera: cobranza,
        enviaTodos: this.checkAll
      }

      this.cobranzaService.saveCobranza(model).subscribe(async (res: any) => {
      }, err => {
        this.spinner.hide();
        this.notificationService.error('Ocurrió un problema al crear cobranza, favor intente nuevamente.', '', true);
        this.nuevaCobranza = false;
        this.muestraCabecera = false;
        this.limpiar();
        this.cleanFiltersCobranzas();
        this.searchCobranzaClasica();
        return;
      });
      this.spinner.hide();
      const response = await this.notificationService.warningAync('', 'Se creara la cobranza o pre cobranza según los parámetros indicados, esto puede tardar varios minutos, se le enviara un correo cuando finalice la creación de esta.');
      if (response.isConfirmed) {
        this.nuevaCobranza = false;
        this.muestraCabecera = false;
        this.limpiar();
        this.cleanFiltersCobranzas();
        this.searchCobranzaClasica();

      } else {
        this.nuevaCobranza = false;
        this.muestraCabecera = false;
        this.limpiar();
        this.cleanFiltersCobranzas();
        this.searchCobranzaClasica();
      }

      // } else {
      //   e.noComplete();
      //   return;
      // }
    }
  }

  async onStepChangedl(e, p1, p2, p3, p4, p5) {


    if (this.paso === 1) {
      const currentDate = new Date();
      var fechaActual = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() };

      if (this.selectedTipoCobranza == null) {
        this.notificationService.warning('Debe seleccionar el tipo de cobranza que se enviará.', '', true);
        this.paso = 1;
        e.goToStep(p1);
        return;
      }

      if (this.selectedProgramacion == null) {
        this.notificationService.warning('Debe seleccionar el tipo de programación.', '', true);
        this.paso = 1;
        e.goToStep(p1);
        return;
      }

      if (this.selectedProgramacion == 1) //Clasica
      {

        if (this.fecha == null) {
          this.notificationService.warning('Debe ingresar fecha de programación para envío.', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        }
        if (this.fecha.year < fechaActual.year) {
          this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        } else if (this.fecha.month < fechaActual.month) {
          this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        } else if (this.fecha.day <= fechaActual.day) {
          this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        }

        if (this.selectedHorario == null) {
          this.notificationService.warning('Debe seleccionar la hora de envío', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        }

        // if (this.fechaVencimiento == null) {
        //   this.notificationService.warning('Debe ingresar fecha de vencimiento para la cobranza.', '', true);
        //   this.paso = 1;
        //   e.goToStep(p1);
        //   return;
        // }

        // if (this.fechaVencimiento.year < this.fecha.year) {
        //   this.notificationService.warning('Fecha vencimiento no puede ser menor a fecha de inicio.', '', true);
        //   this.paso = 1;
        //   e.goToStep(p1);
        //   return;
        // } else if (this.fechaVencimiento.month < this.fecha.month) {
        //   this.notificationService.warning('Fecha vencimiento no puede ser menor a fecha de inicio.', '', true);
        //   this.paso = 1;
        //   e.goToStep(p1);
        //   return;
        // } else if (this.fechaVencimiento.day < this.fecha.day) {
        //   this.notificationService.warning('Fecha vencimiento no puede ser menor a fecha de inicio.', '', true);
        //   this.paso = 1;
        //   e.goToStep(p1);
        //   return;
        // }

      } else if (this.selectedProgramacion == 2) //Inteligente
      {
        if (this.selectedFrecuencia == null) {
          this.notificationService.warning('Debe seleccionar la frecuencia de envío.', '', true);
          this.paso = 1;
          e.goToStep(p1);
          return;
        }

        if (this.selectedFrecuencia == 1) {
          if (this.selectedDia == null) {
            this.notificationService.warning('Debe los días en los que se realizará el envío', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.selectedHorario == null) {
            this.notificationService.warning('Debe seleccionar la hora de envío', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fecha == null) {
            this.notificationService.warning('Debe ingresar fecha de programación para envío.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fecha.year < fechaActual.year) {
            this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          } else if (this.fecha.month < fechaActual.month) {
            this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          } else if (this.fecha.day <= fechaActual.day) {
            this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fechaVencimiento == null) {
            this.notificationService.warning('Debe ingresar fecha de vencimiento para la cobranza.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fechaVencimiento.year < this.fecha.year) {
            this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          } else if (this.fechaVencimiento.month < this.fecha.month) {
            this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          } else if (this.fechaVencimiento.day < this.fecha.day) {
            this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

        }

        if (this.selectedFrecuencia == 2) {
          if (this.selectedPeriocidad == null) {
            this.notificationService.warning('Debe seleccionar la periocidad del envío.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.selectedPeriocidad == 4) {
            if (this.diaEnvio == null) {
              this.notificationService.warning('Debe ingresar el dia que se realizará el envío.', '', true);
              this.paso = 1;
              e.goToStep(p1);
              return;
            }
          }

          if (this.selectedHorario == null) {
            this.notificationService.warning('Debe seleccionar la hora de envío', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fecha == null) {
            this.notificationService.warning('Debe ingresar fecha de programación para envío.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }

          if (this.fechaVencimiento == null) {
            this.notificationService.warning('Debe ingresar fecha de vencimiento para la cobranza.', '', true);
            this.paso = 1;
            e.goToStep(p1);
            return;
          }
        }

      }

      this.cobranzaService.getAniosPagos().subscribe(resAnio => {
        this.anios = resAnio;
        this.anios = this.anios.reverse();
        this.anios.unshift('TODOS');
        // this.ngbDatepickerConfig.minDate = { day: 1, month: 1, year: this.anios[0] };
        // this.ngbDatepickerConfig.maxDate = { day: 31, month: 12, year: this.anios[this.anios.length - 1] };
      }, err => {
        this.spinner.hide();
      });

    }

    if (this.paso === 2) {


      // if (this.selectedAnio == null || this.selectedAnio == '') {
      //   this.notificationService.warning('Debe ingresar el campo obligatorio Año.', '', true);
      //   this.paso = 0;
      //   e.goToStep(p2);
      //   return;
      // }

      if (this.selectedTipoDoc == null || this.selectedTipoDoc == '') {
        this.notificationService.warning('Debe ingresar el campo obligatorio Tipo Documento.', '', true);
        this.paso = 0;
        e.goToStep(p2);
        return;
      }



      var fechaDesde = new Date();
      var fechaHasta = new Date();

      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        fechaDesde = fDesde;
      } else { fechaDesde = null; }

      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
        fechaHasta = fHasta;
      } else { fechaHasta = null; }

      var estadoTipoCobranza = '';
      if (this.selectedTipoCobranza == 1) {
        estadoTipoCobranza = "VENCIDO";
      } else {
        estadoTipoCobranza = "PENDIENTE";
      }

      var excluye = 0;
      if (this.chkClientesExcluidos) { excluye = 1; }

      let tiposDocs = this.selectedTipoDoc != null ? this.selectedTipoDoc.length > 0 ? this.selectedTipoDoc.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let listaPrecio = this.selectedListaPrecio != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let catCli = this.selectedCatCliente != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let vendedor = this.selectedVendedor != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;


      let canalesVenta = this.selectedCanalesVenta != null ? this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let cobradores = this.selectedCobradores != null ? this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null : null;

      let SelAnio = 0;
      if (this.selectedAnio == 'TODOS') {
        SelAnio = 0;
      } else {
        SelAnio = this.selectedAnio;
      }
      this.spinner.show();

      this.documentosEliminados = [];
      this.clientesSeleccionados = [];
      this.clientesEliminados = [];
      const model = {
        tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
        excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta, canalesVenta: canalesVenta,
        cobradores: cobradores, pagina: 1, cantidad: 20
      }
      this.cobranzaService.getDocumentosClientes(model).subscribe((res: any) => {

        this.detalleClientes = res;
        this.paginador.startRow = 0;
        this.paginador.endRow = 20;
        this.paginador.sortBy = 'desc';
        this.config = {
          itemsPerPage: this.paginador.endRow,
          currentPage: 1,
          totalItems: this.detalleClientes.length > 0 ? this.detalleClientes[0].total : 0
        };
        if (this.detalleClientes.length > 0) {
          this.totalClientes = this.detalleClientes[0].total;
        }
        this.calculaSeleccionados();
        this.spinner.hide();
      }, err => {
        this.spinner.hide();
        this.notificationService.error('Ocurrió problema al obtener documentos.', '', true);
      });

    }

    if (this.paso === 3) {

      var sinDatos = false;

      // if(this.selectedProgramacion == 2){
      //   this.detalleClientes.forEach(element => {
      //     if (element.emailCliente == "" || element.emailCliente == null)
      //     {
      //       sinDatos = true;
      //     }
      //   }); 
      // }else{
      //   var aPagar = this.detalleClientes.filter(x => x.selected == true);
      //   aPagar.forEach(element => {
      //     if (element.emailCliente == "" || element.emailCliente == null)
      //     {
      //       sinDatos = true;
      //     }
      //   }); 
      // }



      if (sinDatos) //Valida si continua
      {
        this.paso = 3;
        e.goToStep(p3);
        const response = await this.notificationService.confirmation('Clientes Cobranza', 'Existen clientes seleccionados sin datos de apoderado para envío, estos no serán considerados. ¿Desea continuar?');
        if (!response.isConfirmed) {
          return;
        } else {
          this.paso = 4;
          //e.goToStep(p4);
          //FCA 07-12-2021 Verifica que tipo de envio cobranza es
          if (this.selectedProgramacion == 2) {
            var aPagar = this.detalleClientes.filter(x => x.emailCliente != null && x.emailCliente != "");
          } else {
            var aPagar = this.detalleClientes.filter(x => x.emailCliente != null && x.emailCliente != "" && x.selected == true);
            this.cantidadCorreos = aPagar.length;
          }

          this.totalPagar = 0;
          aPagar.forEach(element => {
            this.totalPagar = this.totalPagar + element.montoDeuda;
          });
        }
      } else {
        this.paso = 4;
        //e.goToStep(p4);
        //FCA 07-12-2021 Verifica que tipo de envio cobranza es
        if (this.selectedProgramacion == 2) {
          var aPagar = this.detalleClientes.filter(x => x.emailCliente != null);
        } else {
          var aPagar = this.detalleClientes.filter(x => x.selected == true);
          this.cantidadCorreos = aPagar.length;
        }

        this.totalPagar = 0;
        aPagar.forEach(element => {
          this.totalPagar = this.totalPagar + element.montoDeuda;
        });
      }

      if (this.selectedTipoCobranza == 1) //COBRANZA
      {
        this.tituloFinal = 'Detalle Cobranza a enviar';
      } else if (this.selectedTipoCobranza == 2) //PRECOBRANZA
      {
        this.tituloFinal = 'Detalle Precobranza a enviar';
      }
    }
  }

  searchCantidadDocumentos() {
    this.documentos = [];
    this.muestraDetalleDocumentos = false;


    var fechaDesde = new Date();
    var fechaHasta = new Date();

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      fechaDesde = fDesde;
    } else { fechaDesde = null; }

    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
      fechaHasta = fHasta;
    } else { fechaHasta = null; }

    var estadoTipoCobranza = '';
    if (this.selectedTipoCobranza == 1) {
      estadoTipoCobranza = "VENCIDO";
    } else {
      estadoTipoCobranza = "PENDIENTE";
    }



    this.spinner.show();
    let tiposDocs = this.selectedTipoDoc ? this.selectedTipoDoc.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;


    var excluye = 0;
    if (this.chkClientesExcluidos) { excluye = 1; }

    let condVenta = this.selectedCondicionVenta ? this.selectedCondicionVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let listaPrecio = this.selectedListaPrecio ? this.selectedListaPrecio.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let catCli = this.selectedCatCliente ? this.selectedCatCliente.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let vendedor = this.selectedVendedor ? this.selectedVendedor.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let SelAnio = 0;
    if (this.selectedAnio == 'TODOS') {
      SelAnio = 0;
    } else {
      SelAnio = this.selectedAnio;
    }

    this.spinner.show();

    const model = {
      tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
      excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta
    }
    this.cobranzaService.getCantidadDocumentos(model).subscribe((res: any) => {

      this.cantidadDocumentosFiltro = res;
      this.spinner.hide();
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al obtener pagos.', '', true);
    });
  }

  onChangeSelect(control: number) {
    if (control == 1 || control == 2)//Año y tipo de documento
    {
      this.diasVencimiento = 0;
      this.dateDesde = null;
      this.dateHasta = null;
    }

    if (this.selectedTipoDoc != null) {
      //Valida si tipo de documento corresponde a pago de apoderado
      if (this.selectedTipoCobranza == 1) {
        var pagoApoderado = this.tiposDocumentoApoderado.find(e => e.tipoDocumento == this.selectedTipoDoc);
        if (pagoApoderado == null) {
          this.enviaLinkPago = 0;
        } else {
          this.enviaLinkPago = 1;
        }
      } else if (this.selectedTipoCobranza == 2) {
        this.enviaLinkPago = 0;
      } else {
        this.enviaLinkPago = null;
      }

    } else {
      this.enviaLinkPago = null;
    }

    this.searchCantidadDocumentos();
  }

  mostrarDetalle() {
    if (this.muestraDetalleDocumentos == false) {
      if (this.cantidadDocumentosFiltro > 0) {
        var fechaDesde = new Date();
        var fechaHasta = new Date();

        if (this.dateDesde != null) {
          const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
          fechaDesde = fDesde;
        } else { fechaDesde = null; }

        if (this.dateHasta != null) {
          const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
          fechaHasta = fHasta;
        } else { fechaHasta = null; }

        var estadoTipoCobranza = '';
        if (this.selectedTipoCobranza == 1) {
          estadoTipoCobranza = "VENCIDO";
        } else {
          estadoTipoCobranza = "PENDIENTE";
        }

        var excluye = 0;
        if (this.chkClientesExcluidos) { excluye = 1; }

        this.spinner.show();

        let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null : null;

        let listaPrecio = this.selectedCondicionVenta != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null : null;

        let catCli = this.selectedCondicionVenta != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null : null;

        let vendedor = this.selectedCondicionVenta != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null : null;

        let SelAnio = 0;
        if (this.selectedAnio == 'TODOS') {
          SelAnio = 0;
        } else {
          SelAnio = this.selectedAnio;
        }

        this.spinner.show();

        const model = {
          tipoDocumento: this.selectedTipoDoc, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
          excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta
        }
        this.cobranzaService.getDocumentosFiltros(model).subscribe((res: any) => {
          if (res.length > 0) {
            this.documentos = res;
            this.muestraDetalleDocumentos = true;

          } else {
            this.notificationService.warning('Sin información para los filtros seleccionados', '', true);
          }
          this.spinner.hide();
        }, err => {
          this.spinner.hide();
          this.notificationService.error('Ocurrió problema al obtener documentos.', '', true);
        });
      }
    } else {
      this.muestraDetalleDocumentos = false;
    }

  }

  validateRut() {
    if (this.searchRut) {
      if (this.utils.isValidRUT(this.searchRut)) {
        this.searchRut = this.utils.estandarizaRut(this.searchRut);
      } else {
        this.searchRut = null;
        this.searchRut = '';
        this.notificationService.warning('Rut ingresado no es válido.', '', true);
      }
    }
  }

  onSelect(val: any) {

    // for (let i: number = 0; i <= val.selected.length -1; i++) {
    //   if (val.selected[i].tieneApoderadoDatos == 0)
    //   {
    //     this.notificationService.warning('Cliente sin información de apoderado, debe completar los datos para poder realizar el envío.', '', true);
    //     for (let x: number = 0; x <= this.selected.length -1; x++) {
    //       if (this.selected[x].rutCliente == val.selected[i].rutCliente) {
    //         this.selected.splice(x, 1);
    //         this.selected = [...this.selected]
    //         break;
    //       }
    //     }
    //   }


    // if (val.selected[i].cantidadDocumentos == 0) {
    //   this.notificationService.warning('Sin documentos pendientes.', '', true);
    //   for (let x: number = 0; x <= this.selected.length -1; x++) {
    //     if (this.selected[x].rutCliente == val.selected[i].rutCliente) {
    //       this.selected.splice(x, 1);
    //       this.selected = [...this.selected]
    //       break;
    //     }
    //   }
    //   // this.selected.splice(i, 1);
    //   break;
    // }
    // }

  }

  verDetalle(cliente: any, content) {
    if (cliente.cantidadDoctos == 0) {
      this.notificationService.warning('Sin documentos.', '', true);
      return;
    }
    this.spinner.show();
    var fechaDesde = new Date();
    var fechaHasta = new Date();

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      fechaDesde = fDesde;
    } else { fechaDesde = null; }

    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
      fechaHasta = fHasta;
    } else { fechaHasta = null; }

    var estadoTipoCobranza = '';
    if (this.selectedTipoCobranza == 1) {
      estadoTipoCobranza = "VENCIDO";
    } else {
      estadoTipoCobranza = "PENDIENTE";
    }

    var excluye = 0;
    if (this.chkClientesExcluidos) { excluye = 1; }

    let tiposDocs = this.selectedTipoDoc != null ? this.selectedTipoDoc.length > 0 ? this.selectedTipoDoc.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let listaPrecio = this.selectedListaPrecio != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let catCli = this.selectedCatCliente != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let vendedor = this.selectedVendedor != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;


    let canalesVenta = this.selectedCanalesVenta != null ? this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let cobradores = this.selectedCobradores != null ? this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let SelAnio = 0;
    if (this.selectedAnio == 'TODOS') {
      SelAnio = 0;
    } else {
      SelAnio = this.selectedAnio;
    }

    const model = {
      tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
      excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta, canalesVenta: canalesVenta,
      cobradores: cobradores, rutCliente: cliente.rutaux,
      codAuxCliente: cliente.codaux

    }
    this.cobranzaService.getDocumentosPorCliente(model).subscribe((res: any) => {
      this.detalleDocumentos = res;
      if (this.detalleDocumentos.length > 0) {
        this.clienteRut = this.detalleDocumentos[0].rutaux;
        this.clienteNombre = this.detalleDocumentos[0].nomaux;

        for (let index = 0; index < res.length; index++) {
          let esEliminado = this.documentosEliminados.filter(x => x.ttdcod == this.detalleDocumentos[index].ttdcod && x.numdoc == this.detalleDocumentos[index].numdoc);
          if (esEliminado.length > 0) {
            this.detalleDocumentos.splice(index, 1);
          }

        }
        this.spinner.hide();
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
      } else {
        this.spinner.hide();
        this.notificationService.warning('No se encontraron documentos.', '', true);
      }

    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al obtener documentos.', '', true);
    });
  }

  agregarCorreoApoderado(doc: any, content) {
    this.nuevoCorreo = '';
    this.detalleDocumentos = doc;
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  agregarCorreo() {
    if (!this.utils.validateMail(this.nuevoCorreo)) {
      this.notificationService.warning('Correo ingresado invalido.', '', true);
      this.nuevoCorreo = '';
    } else {
      this.detalleClientes.forEach(element => {
        if (element.rutCliente == this.detalleDocumentos.rutCliente) {

          element.emailCliente = this.nuevoCorreo;
        }
      });

      this.notificationService.success('Correo agregado correctamente.', '', true);
      this.modalService.dismissAll();
    }
  }

  eliminarDoc(doc: any, index: number) {
    debugger
    this.detalleDocumentos.splice(index, 1);
    this.detalleClientes.forEach(element => {
      if (element.codaux == doc.codAux) {
        element.cantidadDoctos = element.cantidadDoctos - 1;
        element.saldototal = element.saldototal - doc.saldoadic;
        if (element.cantidadDoctos == 0) {
          if (this.checkAll && element.selected) {
            element.selected = false;
          }

          if (!this.checkAll && element.selected) {
            element.selected = false;
            let cliente = this.clientesSeleccionados.findIndex(x => x.codaux == element.codaux);
            if (cliente != -1) {
              this.clientesSeleccionados.splice(cliente, 1);
            }
          }
          this.clientesEliminados.push(element);
        }
      }
    });
    this.documentosEliminados.push(doc)
    this.calculaSeleccionados();
  }

  changePageClientes(event: any) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 20);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    var fechaDesde = new Date();
    var fechaHasta = new Date();

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      fechaDesde = fDesde;
    } else { fechaDesde = null; }

    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
      fechaHasta = fHasta;
    } else { fechaHasta = null; }

    var estadoTipoCobranza = '';
    if (this.selectedTipoCobranza == 1) {
      estadoTipoCobranza = "VENCIDO";
    } else {
      estadoTipoCobranza = "PENDIENTE";
    }

    var excluye = 0;
    if (this.chkClientesExcluidos) { excluye = 1; }

    let tiposDocs = this.selectedTipoDoc != null ? this.selectedTipoDoc.length > 0 ? this.selectedTipoDoc.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let listaPrecio = this.selectedListaPrecio != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let catCli = this.selectedCatCliente != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let vendedor = this.selectedVendedor != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;


    let canalesVenta = this.selectedCanalesVenta != null ? this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let cobradores = this.selectedCobradores != null ? this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let SelAnio = 0;
    if (this.selectedAnio == 'TODOS') {
      SelAnio = 0;
    } else {
      SelAnio = this.selectedAnio;
    }
    this.spinner.show();


    const model = {
      tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
      excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta, canalesVenta: canalesVenta,
      cobradores: cobradores, pagina: this.config.currentPage, cantidad: 20,
      rutCliente: this.searchRut == null ? '' : this.searchRut,
      codAuxCliente: this.searchCodAux == null ? '' : this.searchCodAux
    }
    this.cobranzaService.getDocumentosClientes(model).subscribe((res: any) => {

      this.detalleClientes = res;
      for (let index = 0; index < res.length; index++) {
        var existenEliminados = this.documentosEliminados.filter(x => x.codAux == res[index].codaux);
        if (existenEliminados.length > 0) {
          existenEliminados.forEach(element => {
            this.detalleClientes[index].cantidadDoctos = res[index].cantidadDoctos - 1;
            this.detalleClientes[index].saldototal = this.detalleClientes[index].saldototal - element.saldoadic;
          });
        }
      }
      if (this.checkAll) {
        this.detalleClientes.forEach(element => {
          let cliente = this.clientesEliminados.findIndex(x => x.codaux == element.codaux);
          if (cliente != -1) {
            element.selected = true;
          } else {
            element.selected = false;
          }
        });
      } else {
        this.detalleClientes.forEach(element => {
          let cliente = this.clientesSeleccionados.findIndex(x => x.codaux == element.codaux);
          if (cliente != -1) {
            element.selected = true;
          } else {
            element.selected = false;
          }
        });
      }
      this.calculaSeleccionados();
      this.spinner.hide();
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al obtener pagos.', '', true);
    });

  }

  onSel(val: any, c: any) {
    this.detalleClientes.forEach(element => {
      if (c.rutaux == element.rutaux) {
        element.selected = val.target.checked
        if (element.selected) {
          if (this.checkAll) {
            let cliente = this.clientesEliminados.findIndex(x => x.codaux == element.codaux);
            if (cliente != -1) {
              this.clientesEliminados.splice(cliente, 1);
            }
          } else {
            this.clientesSeleccionados.push(element);
          }
        } else {
          if (this.checkAll) {
            this.clientesEliminados.push(element)
          } else {
            let cliente = this.clientesSeleccionados.findIndex(x => x.codaux == element.codaux);
            if (cliente != -1) {
              this.clientesSeleccionados.splice(cliente, 1);
            }
          }
        }
      }
    });
    this.calculaSeleccionados();
  }


  onSelAll(val: any) {
    this.checkAll = val.target.checked;
    this.detalleClientes.forEach(element => {
      let clienteEliminado = this.clientesEliminados.findIndex(x => x.codaux == element.codaux);
      if (clienteEliminado == -1) {
        element.selected = val.target.checked
      } else {
        element.selected = false;
      }
    });
    if (this.checkAll) {
      this.clientesSeleccionados = [];
    }
    this.calculaSeleccionados();

  }

  deleteCobranza(item: any) {

    this.spinner.show();
    this.cobranzaService.deleteCobranza(item.idCobranza).subscribe((res: any) => {
      this.searchCobranzaClasica();
      this.spinner.hide();
      this.notificationService.success('Se ha eliminado exitosamente', '', true);
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al eliminar cobranza.', '', true);
    });
  }


  calculaSeleccionados() {
    debugger
    if (this.checkAll) {
      this.correosSeleccionados = this.totalClientes - this.clientesEliminados.length;
    } else {
      this.correosSeleccionados = this.clientesSeleccionados.length;
    }
  }

  //FCA 13-12-2021
  filter2() {

    let data: any = Object.assign([], this.detalleClientes);

    if (this.montoDesdeFiltro != null) {
      data = data.filter(x => x.montoDeuda >= this.montoDesdeFiltro)
    }

    if (this.montoHastaFiltro != null) {
      data = data.filter(x => x.montoDeuda <= this.montoHastaFiltro)
    }

    this.detalleClienteFiltro = data;

    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: this.detalleClienteFiltro.length
    };

    this.detalleClientePag = this.detalleClienteFiltro.slice(this.paginador.startRow, this.paginador.endRow);
  }

  //FCA 13-12-2021
  cleanFilters2() {
    this.selectedArancel = null;
    this.selectedCurso = null;
    this.montoHastaFiltro = null;
    this.montoDesdeFiltro = null;
  }

  filter() {
    this.paginador.startRow = 0;
    this.paginador.endRow = 20;
    this.paginador.sortBy = 'desc';
    this.config.currentPage = 1;
    var fechaDesde = new Date();
    var fechaHasta = new Date();

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      fechaDesde = fDesde;
    } else { fechaDesde = null; }

    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 0, 0, 0);
      fechaHasta = fHasta;
    } else { fechaHasta = null; }

    var estadoTipoCobranza = '';
    if (this.selectedTipoCobranza == 1) {
      estadoTipoCobranza = "VENCIDO";
    } else {
      estadoTipoCobranza = "PENDIENTE";
    }

    var excluye = 0;
    if (this.chkClientesExcluidos) { excluye = 1; }

    let tiposDocs = this.selectedTipoDoc != null ? this.selectedTipoDoc.length > 0 ? this.selectedTipoDoc.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let condVenta = this.selectedCondicionVenta != null ? this.selectedCondicionVenta.length > 0 ? this.selectedCondicionVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let listaPrecio = this.selectedListaPrecio != null ? this.selectedListaPrecio.length > 0 ? this.selectedListaPrecio.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let catCli = this.selectedCatCliente != null ? this.selectedCatCliente.length > 0 ? this.selectedCatCliente.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let vendedor = this.selectedVendedor != null ? this.selectedVendedor.length > 0 ? this.selectedVendedor.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;


    let canalesVenta = this.selectedCanalesVenta != null ? this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let cobradores = this.selectedCobradores != null ? this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null : null;

    let SelAnio = 0;
    if (this.selectedAnio == 'TODOS') {
      SelAnio = 0;
    } else {
      SelAnio = this.selectedAnio;
    }
    this.spinner.show();


    const model = {
      tipoDocumento: tiposDocs, anio: SelAnio, fecha: fechaDesde, fechaHasta: fechaHasta, cantidadDias: this.diasVencimiento, estado: estadoTipoCobranza,
      excluyeClientes: excluye, listasPrecio: listaPrecio, vendedores: vendedor, categoriasClientes: catCli, CondicionesVenta: condVenta, canalesVenta: canalesVenta,
      cobradores: cobradores, pagina: 1, cantidad: 20,
      rutCliente: this.searchRut == null ? '' : this.searchRut,
      codAuxCliente: this.searchCodAux == null ? '' : this.searchCodAux
    }
    this.cobranzaService.getDocumentosClientes(model).subscribe((res: any) => {

      this.detalleClientes = res;
      for (let index = 0; index < res.length; index++) {
        var existenEliminados = this.documentosEliminados.filter(x => x.codAux == res[index].codaux);
        if (existenEliminados.length > 0) {
          existenEliminados.forEach(element => {
            this.detalleClientes[index].cantidadDoctos = res[index].cantidadDoctos - 1;
            this.detalleClientes[index].saldototal = this.detalleClientes[index].saldototal - element.saldoadic;
          });
        }
      }
      if (this.checkAll) {
        this.detalleClientes.forEach(element => {
          let cliente = this.clientesEliminados.findIndex(x => x.codaux == element.codaux);
          if (cliente != -1) {
            element.selected = false;
          } else {
            element.selected = true;
          }
        });
      } else {
        this.detalleClientes.forEach(element => {
          let cliente = this.clientesSeleccionados.findIndex(x => x.codaux == element.codaux);
          if (cliente != -1) {
            element.selected = true;
          } else {
            element.selected = false;
          }
        });
      }
      this.spinner.hide();
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al obtener documentos.', '', true);
    });

  }

  cleanFilters() {
    this.searchRut = null;
    this.searchNombre = null;
    this.searchCodAux = null;
    this.selectedEstadoCorreo = null;
    this.filter();
  }

  cleanFiltersCobranzas() {
    this.selectedTipoFiltro = null;
    this.nombreFiltro = null;
    this.selectedEstadoFiltro = null;
    this.dateDesdeFiltro = null;
    this.dateHastaFiltro = null;

    this.searchCobranzaClasica();
  }

  limpiar() {
    const currentDate = new Date();
    this.enviaCorreoFicha = false;
    this.enviaTodosContactos = false;
    this.selectedCargosContactos = [];
    this.enviaTodosCargos = false;
    this.checkAll = false;
    this.selectedEstadoCliente = 'TODOS';
    this.selectedCondicionVenta = null;
    this.selectedVendedor = null;
    this.selectedCatCliente = null;
    this.selectedListaPrecio = null;
    this.chkEstadoNuevo = true;
    this.textoEstado = 'Inactivo';
    this.selectedTipoCobranza = null;
    this.chkClientesExcluidos = false;
    this.fecha = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() + 1 };
    this.fechaVencimiento = null;
    this.selectedProgramacion = 1;
    this.selectedFrecuencia = null;
    this.selectedHorario = null;
    this.selectedDia = null;
    this.muestraDetalleDocumentos = false;
    this.dateDesde = null;
    this.dateHasta = null;
    this.nombreCobranza = null;
    this.paso = 0;
    this.selectedTipoDoc = null;
    this.selectedAnio = 'TODOS';
    this.cantidadDocumentosFiltro = 0;
    this.diasVencimiento = 0;
    this.documentos = [];
    this.searchRut = null;
    this.searchNombre = null;
    this.selectedEstadoCorreo = null;
    this.detalleClientes = [];
    this.detalleClientePag = [];
    this.detalleClienteFiltro = [];
    this.selected = [];
    this.detalleDocumentos = [];
    this.nuevoCorreo = null;
    this.cantidadCorreos = 0;
    this.tituloFinal = '';
    this.selectedClientes = [];
    this.correosDisponibles = 0;
    this.correosSeleccionados = 0;
    this.enviaLinkPago = null;
    this.totalPagar = 0;

    this.muestraDetalleClientes = false; //FCA 07-12-2021
  }

  async enviarCobranza() {
    const response = await this.notificationService.confirmation('Ejecutar Envío', 'Se ejecutará el proceso de envío, esto puede tardar varios minutos dependiendo de la cantidad de documentos a enviar. ¿Desea continuar?');
    if (response.isConfirmed) {
      this.cobranzaService.enviaCobranza().subscribe((res: any) => {
      }, err => {
        this.spinner.hide();
      });
    }
  }

  mostrarFiltro() {
    this.muestraFiltroCobranza = !this.muestraFiltroCobranza;

    if (this.muestraFiltroCobranza) {
      this.textoBotonFiltro = 'Ocultar Filtros';

    } else {
      this.textoBotonFiltro = 'Mostrar Filtros';
    }
  }

  cancelar() {
    this.limpiar();
    this.muestraCabecera = false;
    this.nuevaCobranza = false;
  }
  openModalAbonos(content, documento: any) {
    this.spinner.show();
    const model = { codAux: documento.codAuxCliente, folio: documento.folio, TipoDoc: documento.codTipoDocumento }
    this.clientesService.getPagosDocumento(model).subscribe((res: any) => {

      this.abonos = [];
      this.abonos = res;

      if (this.abonos.length <= 0) {
        this.spinner.hide();
        this.notificationService.warning('El documento no posee abonos asociados', '', true);
        return;
      }

      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
      this.spinner.hide();
    }, err => { this.spinner.hide(); });

  }



  closeModal() {
    this.modalService.dismissAll();
  }

  verDetalleCobranza(item: Cobranza) {
    this.spinner.show();

    const model = { idCobranza: item.idCobranza }
    this.cobranzaService.getCobranzasDetalle(model).subscribe((res: any) => {

      //FCA 22-11-2021 Se modifica para agregar paginador y arreglar fechas
      this.cobranzaDetalleRes = res;


      this.paginadorDetalle.startRow = 0;
      this.paginadorDetalle.endRow = 10;
      this.paginadorDetalle.sortBy = 'desc';
      this.paginadorDetalle.search = '';
      this.configDetalle = {
        itemsPerPage: this.paginadorDetalle.endRow,
        currentPage: 1,
        totalItems: this.cobranzaDetalleRes.length
      }
      this.totalItemsDetalle = this.cobranzaDetalleRes.length;

      this.cobranzaDetalle = this.cobranzaDetalleRes.slice(this.paginadorDetalle.startRow, this.paginadorDetalle.endRow);

      this.cobranzaService.getCobranzaGraficos(item.idCobranza).subscribe((res: any) => {
        this.cobranzaDatos = res;
        this.muestraDetalle = true;

        this.chartMonto = {
          ...echartStyles.defaultOptions, ...{
            series: [{
              type: 'pie',
              itemStyle: echartStyles.pieLineStyle,
              data: [{
                name: 'Pendiente Recaudar',
                value: this.cobranzaDatos.totalRecaudar - this.cobranzaDatos.totalRecaudado,
                ...echartStyles.pieLabelOff,
                itemStyle: {
                  borderColor: '#ff4d4d',
                }
              }, {
                name: 'Recaudado',
                value: this.cobranzaDatos.totalRecaudado,
                ...echartStyles.pieLabelOff,
                itemStyle: {
                  borderColor: '#009933',
                }
              }]
            }]
          }
        };

        this.chartDocumentos = {
          ...echartStyles.defaultOptions, ...{
            series: [{
              type: 'pie',
              itemStyle: echartStyles.pieLineStyle,
              data: [{
                name: 'Documentos Pendientes',
                value: this.cobranzaDatos.cantidadDocumentosEnviadosCobrar - this.cobranzaDatos.cantidadDocumentosPagados,
                ...echartStyles.pieLabelOff,
                itemStyle: {
                  borderColor: '#ff4d4d',
                }
              }, {
                name: 'Documentos Pagados',
                value: this.cobranzaDatos.cantidadDocumentosPagados,
                ...echartStyles.pieLabelOff,
                itemStyle: {
                  borderColor: '#009933',
                }
              }]
            }]
          }
        };
        this.spinner.hide();
      }, err => {
        this.spinner.hide();
      });

    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió problema al obtener documentos.', '', true);
    });

  }

  //FCA 22-11-2021
  changePageDetalle(event: any) {
    this.paginadorDetalle.startRow = ((event - 1) * 10);
    this.paginadorDetalle.endRow = (event * 10);
    this.paginadorDetalle.sortBy = 'desc';
    this.configDetalle.currentPage = event;
    this.cobranzaDetalle = this.cobranzaDetalleRes.slice(this.paginadorDetalle.startRow, this.paginadorDetalle.endRow);
  }

  filterDetalle() {

    this.paginadorDetalle.startRow = 0
    this.paginadorDetalle.endRow = 10
    this.paginadorDetalle.sortBy = 'desc';
    this.configDetalle.currentPage = 1;
    let data = [...this.cobranzaDetalleRes];

    if (this.selectedEstadoDetalle != 0 && this.selectedEstadoDetalle != null) {
      data = data.filter(x => x.idEstado == this.selectedEstadoDetalle)
    }

    if (this.searchRutDetalle != '' && this.searchRutDetalle != null) {
      data = data.filter(x => x.rutCliente == this.searchRutDetalle);
    }

    if (this.filtroFolioDetalle != 0 && this.filtroFolioDetalle != null) {
      data = data.filter(x => x.folio == this.filtroFolioDetalle);
    }
    this.cobranzaDetalle = data.slice(this.paginadorDetalle.startRow, this.paginadorDetalle.endRow);
  }

  volver() {
    this.muestraDetalle = false;
    this.cobranzaDatos = new Cobranza();
    this.cobranzaDetalle = [];
  }

  handleFocus(): void {
    setTimeout(() => {
      const myCustomClass: string = "custom-dropdown-item"
      const panel = document.querySelector('.ng-dropdown-panel-items');
      panel.classList.add(myCustomClass);
      console.log('panel', panel);
    }, 0);
  }

  seleccionaPeriocidad() {
    //FCA 07-12-2021 Se valida si excluye fin de semana.
    if (this.chkFines) {
      this.frecuenciaDias = this.frecuenciaDias.splice(0, 5);
      this.selectedDia = null;
    } else {
      if (this.frecuenciaDias.length == 5) {
        let diasSemana: any = [{ id: 1, dia: "LUNES" }, { id: 2, dia: "MARTES" }, { id: 3, dia: "MIERCOLES" }, { id: 4, dia: "JUEVES" }, { id: 5, dia: "VIERNES" }, { id: 6, dia: "SABADO" }, { id: 7, dia: "DOMINGO" }];
        this.frecuenciaDias = diasSemana;
        this.selectedDia = null;
      }
    }
    if (this.selectedPeriocidad != null) {

      if (this.selectedPeriocidad == 1) //Primer dia del mes
      {
        if (this.chkFines && this.chkFeriados) {
          this.mensajeInfo = "Si primer día del mes es fin de semana o feriado, cobranza sera enviada el día habil siguente."
        } else if (this.chkFines) {
          this.mensajeInfo = "Si primer día del mes es fin de semana, cobranza sera enviada el día habil siguente."
        } else if (this.chkFeriados) {
          this.mensajeInfo = "Si primer día del mes es feriado, cobranza sera enviada el día habil siguente."
        } else {
          this.mensajeInfo = '';
        }
      } else if (this.selectedPeriocidad == 2) //mitad de mes
      {
        if (this.chkFines && this.chkFeriados) {
          this.mensajeInfo = "Si día 15 del mes es fin de semana o feriado, cobranza sera enviada el día habil siguente."
        } else if (this.chkFines) {
          this.mensajeInfo = "Si día 15 del mes es fin de semana, cobranza sera enviada el día habil siguente."
        } else if (this.chkFeriados) {
          this.mensajeInfo = "Si día 15 del mes es feriado, cobranza sera enviada el día habil siguente."
        } else {
          this.mensajeInfo = '';
        }
      } else if (this.selectedPeriocidad == 3) //Ultimo dia
      {
        if (this.chkFines && this.chkFeriados) {
          this.mensajeInfo = "Si ultimo día del mes es fin de semana o feriado, cobranza sera enviada el día habil siguente."
        } else if (this.chkFines) {
          this.mensajeInfo = "Si ultimo día del mes es fin de semana, cobranza sera enviada el día habil siguente."
        } else if (this.chkFeriados) {
          this.mensajeInfo = "Si ultimo día del mes es feriado, cobranza sera enviada el día habil siguente."
        } else {
          this.mensajeInfo = '';
        }
      }
    }
  }

  cambiaFecha(val: any, tipoFecha: number) {
    //1 fecha inicio
    //2 fecha vencimiento
    const currentDate = new Date();
    var fechaActual = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() };


    let actual: string = fechaActual.year.toString() + '/' + fechaActual.month.toString() + '/' + fechaActual.day.toString();



    if (tipoFecha == 1) {
      let fechaInicio: string = val.year.toString() + '/' + val.month.toString() + '/' + val.day.toString();
      if (Date.parse(actual) >= Date.parse(fechaInicio)) {
        this.mensajeFechaInicioErronea = 'Fecha inicio no puede ser menor o igual a fecha actual.'
        return;
        //this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
      }

      if (this.fechaVencimiento != null) {
        let fechaFin: string = this.fechaVencimiento.year.toString() + '/' + this.fechaVencimiento.month.toString() + '/' + this.fechaVencimiento.day.toString();
        if (Date.parse(fechaInicio) > Date.parse(fechaFin)) {
          this.mensajeFechaInicioErronea = 'Fecha inicio no puede ser mayor o igual a fecha de termino.'
          //this.notificationService.warning('Fecha inicio no puede ser mayor a fecha de termino.', '', true);
          return;
        }
      }

      this.mensajeFechaInicioErronea = '';
      // if (val.year < fechaActual.year) {

      //   this.fecha = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() + 1 };
      // } else if (val.month < fechaActual.month) {
      //   this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
      //   this.fecha = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() + 1 };
      // } else if (val.day <= fechaActual.day) {
      //   this.notificationService.warning('Fecha inicio no puede ser menor o igual a fecha actual.', '', true);
      //   this.fecha = { year: currentDate.getFullYear(), month: currentDate.getMonth() + 1, day: currentDate.getDate() + 1 };
      // } else {
      //   let val2: string = this.fechaVencimiento.year.toString() + '/' + this.fechaVencimiento.month.toString() + '/' + this.fechaVencimiento.day.toString();
      //   let fecha: string = val.year.toString() + '/' + val.month.toString() + '/' + val.day.toString();
      //   if (Date.parse(val2) < Date.parse(fecha)) {
      //     this.notificationService.warning('Fecha vencimiento no puede ser menor a fecha de inicio.', '', true);
      //     this.fechaVencimiento = { year: this.fecha.year, month: this.fecha.month, day: this.fecha.day };
      //   }

      // }
    }

    if (tipoFecha == 2) {
      let fechaFin: string = val.year.toString() + '/' + val.month.toString() + '/' + val.day.toString();
      let fechaInicio: string = this.fecha.year.toString() + '/' + this.fecha.month.toString() + '/' + this.fecha.day.toString();
      if (Date.parse(fechaFin) <= Date.parse(fechaInicio)) {
        this.mensajeFechaTerminoErronea = 'Fecha vencimiento debe ser mayor a fecha de inicio.'
        return;
        //this.notificationService.warning('Fecha vencimiento no puede ser menor a fecha de inicio.', '', true);
      }
      if (this.mensajeFechaInicioErronea == 'Fecha inicio no puede ser mayor o igual a fecha de termino.') {
        this.mensajeFechaInicioErronea = '';
      }
      this.mensajeFechaTerminoErronea = '';
    }
    else if (tipoFecha == 3) {
      if (val.year < this.fecha.year) {
        this.mensajeFechaTerminoErronea = 'Fecha vencimiento no puede ser menor o igual a fecha de inicio.'
        //this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
      } else if (val.month < this.fecha.month) {
        this.mensajeFechaTerminoErronea = 'Fecha vencimiento no puede ser menor o igual a fecha de inicio.'
        //this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
      } else if (val.day <= this.fecha.day) {
        this.mensajeFechaTerminoErronea = 'Fecha vencimiento no puede ser menor o igual a fecha de inicio.'
        //this.notificationService.warning('Fecha vencimiento no puede ser menor o igual a fecha de inicio.', '', true);
      }
    }

  }

  mostrarDetalleCliente() {
    this.muestraDetalleClientes = true;
  }


  //FCA 13-12-2021
  async delete(item: Cobranza) {
    const response = await this.notificationService.confirmation('Eliminar Cobranza', '¿Confirma eliminar esta Cobranza?');
    if (response.isConfirmed) {
      this.cobranzaService.deleteCobranza(item.idCobranza).subscribe(res => {
        this.notificationService.success('Cobranza eliminada correctamente', 'Correcto', true);
        this.searchCobranzaClasica();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar la cobranza', '', true); });
    }
  }

  //FCA 13-12-2021
  async desactivar(item: Cobranza) {
    let estado = ""
    if (item.estado == 1) {
      estado = "Desactivar"
    } else if (item.estado == 0) {
      estado = "Activar"
    }
    const response = await this.notificationService.confirmation(estado + ' Cobranza', '¿Confirma ' + estado + ' esta Cobranza?');
    if (response.isConfirmed) {
      if (item.estado == 1) {
        item.estado = 0;
      } else if (item.estado == 0) {
        item.estado = 1;
      }

      this.cobranzaService.modificarEstado(item.idCobranza, item.estado).subscribe(res => {
        if (item.estado == 1) {
          this.notificationService.success('Cobranza desactivada correctamente', 'Correcto', true);
        } else if (item.estado == 0) {
          this.notificationService.success('Cobranza activada correctamente', 'Correcto', true);
        }

        this.searchCobranzaClasica();
      }, err => { this.notificationService.error('Ocurrió un error al cambiar el estado de la cobranza', '', true); });
    }
  }

  cleanFiltersDetalle() {
    this.searchRut = '';
    this.filtroFolioDetalle = '';
    this.selectedEstadoDetalle = 0;
    this.filterDetalle();
  }


}







