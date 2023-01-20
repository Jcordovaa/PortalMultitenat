import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { NgbDatepickerConfig, NgbDatepickerI18n, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Paginator } from '../../../shared/models/paginator.model';
import { AuthService } from 'src/app/shared/services/auth.service';
import { CondicionVentaService } from 'src/app/shared/services/condicionventa.service';
import { CategoriaClienteDTO, CondicionVentaDTO, ListaPrecioDTO, TipoDocumento, VendedorDTO } from 'src/app/shared/models/softland.model';
import { CobranzasService } from 'src/app/shared/services/cobranzas.service';
import { SoftlandService } from 'src/app/shared/services/softland.service';
import { Automatizacion, TipoAutomatizacion } from 'src/app/shared/models/automatizacion.model';
import { AutomatizacionService } from 'src/app/shared/services/automatizacion.service';
import { CanalVenta } from 'src/app/shared/models/canalventa.model';
import { Cobrador } from 'src/app/shared/models/cobrador.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';



@Component({
  selector: 'app-automation',
  templateUrl: './automation.component.html',
  styleUrls: ['./automation.component.scss'],
  animations: [SharedAnimations]

})

export class AutomationComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nueva Automatización';
  //FCA 07-03-2022
  public noResultsText: string = '';

  public condicionesVenta: CondicionVentaDTO[] = [];
  public listasPrecio: ListaPrecioDTO[] = [];
  public categoriasCliente: CategoriaClienteDTO[] = [];
  public vendedores: VendedorDTO[] = [];
  public tiposDocumento: TipoDocumento[] = [];
  public selectedTiposDocumentosRecordatorio: any = []
  public selectedTiposDocumentosEstadoCuenta: any = [];
  public selectedTiposDocumentosCobranza: any = [];
  public mostrarEnPago: any[] = [{ id: 0, nombre: 'Estado de cuenta' }, { id: 1, nombre: 'Solo documentos vencidos' }];
  public diasSemana: any[] = [{ id: 1, nombre: 'Lunes' }, { id: 2, nombre: 'Martes' }, { id: 3, nombre: 'Miercoles' }, { id: 4, nombre: 'Jueves' }, { id: 5, nombre: 'Viernes' }, { id: 6, nombre: 'Sabado' }, { id: 0, nombre: 'Domingo' }];
  public horarios: any = [];
  private ngbDatepickerConfig: NgbDatepickerConfig;
  private ngbDatepickerI18n: NgbDatepickerI18n;
  public anios: any = [];
  public periocidades: any = [];
  public tiposAutomatizaciones: TipoAutomatizacion[] = [];
  public nuevaAutomaetizacion: Automatizacion = new Automatizacion();
  public listaAutomatizaciones: Automatizacion[] = [];
  public cobradores: Cobrador[] = [];
  public canalesVenta: CanalVenta[] = [];
  public loaded: boolean = false;
  muestaRecordatorio: number = 0;
  muestraCobranza: number = 0;
  muestraEstadoCuenta: number = 0;
  recordatorio: Automatizacion = new Automatizacion();
  cobranza: Automatizacion = new Automatizacion();
  estadoCuenta: Automatizacion = new Automatizacion();

  //recordatorio
  selectedAnioRecordatorio: any = null;
  selectedCargosRecordatorio: any = [];
  selectedCanalesVentaRecordatorio: any = [];
  selectedCobradoresRecordatorio: any = [];
  selectedVendedoresRecordatorio: any = [];
  selectedListasPrecioRecordatorio: any = [];
  selectedCategoriasClienteRecordatorio: any = [];
  selectedCondicionesVentaRecordatorio: any = [];

  //estado de cuenta
  selectedAnioEstadoCuenta: any = null;
  selectedCargosEstadoCuenta: any = [];
  selectedCanalesVentaEstadoCuenta: any = [];
  selectedCobradoresEstadoCuenta: any = [];
  selectedVendedoresEstadoCuenta: any = [];
  selectedListasPrecioEstadoCuenta: any = [];
  selectedCategoriasClienteEstadoCuenta: any = [];
  selectedCondicionesVentaEstadoCuenta: any = [];

  //cobranza
  selectedAnioCobranza: any = null;
  selectedCargosCobranza: any = [];
  selectedCanalesVentaCobranza: any = [];
  selectedCobradoresCobranza: any = [];
  selectedVendedoresCobranza: any = [];
  selectedListasPrecioCobranza: any = [];
  selectedCategoriasClienteCobranza: any = [];
  selectedCondicionesVentaCobranza: any = [];

  cargos: any = [];

  constructor(private spinner: NgxSpinnerService, private cobranzaService: CobranzasService, private softlandService: SoftlandService,
    private modalService: NgbModal, private automatizacionService: AutomatizacionService, private configSoftlandService: ConfiguracionSoftlandService,
    private notificationService: NotificationService,
    private authService: AuthService, private condicionVentaService: CondicionVentaService) {
    ;
  }

  ngOnInit() {
    this.getCategoriasCliente();
    this.getListasPrecio();
    this.getCondicionesVenta();
    this.getVendedores();
    this.getcargos();
    this.getTipoDocumentosCobranza();
    this.getTiposAutomatizaciones();
    this.getAnios();
    this.getPeriodicidad();
    this.getHorarios();
    // this.getCanalesVenta();
    // this.getCobradores();
    this.getAutomatizaciones();
  }


  getCanalesVenta() {
    this.softlandService.getCanalesVenta().subscribe((res: CanalVenta[]) => {
      this.canalesVenta = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar canales de venta', '', true); });
  }

  getCobradores() {
    this.softlandService.getCobradores().subscribe((res: Cobrador[]) => {
      this.cobradores = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar canales de venta', '', true); });
  }

  getcargos() {
    this.configSoftlandService.getCargos().subscribe((res: any) => {
      this.cargos = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar cargos', '', true); });
  }



  getCategoriasCliente() {
    this.spinner.show();
    this.softlandService.getCategoriasCliente().subscribe((resp: CategoriaClienteDTO[]) => {
      this.categoriasCliente = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getTipoDocumentosCobranza() {
    this.spinner.show();
    this.cobranzaService.getTipoDocumentosPagos().subscribe((resp: TipoDocumento[]) => {
      this.tiposDocumento = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getListasPrecio() {
    this.spinner.show();
    this.softlandService.getListasPrecio().subscribe((resp: ListaPrecioDTO[]) => {
      this.listasPrecio = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getVendedores() {
    this.spinner.show();
    this.softlandService.getVendedores().subscribe((resp: VendedorDTO[]) => {
      this.vendedores = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getCondicionesVenta() {
    this.spinner.show();
    this.softlandService.getCondicionesVenta().subscribe((resp: CondicionVentaDTO[]) => {
      this.condicionesVenta = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getTiposAutomatizaciones() {
    this.spinner.show();
    this.automatizacionService.getTipoAutomatizaciones().subscribe((resp: TipoAutomatizacion[]) => {
      this.tiposAutomatizaciones = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getAnios() {
    this.cobranzaService.getAniosPagos().subscribe(resAnio => {
      this.anios = resAnio;
      this.anios = this.anios.reverse();
      this.anios.unshift('TODOS');
    }, err => {
      this.spinner.hide();
    });
  }

  getPeriodicidad() {
    this.cobranzaService.getCobranzaPeriocidad().subscribe((resp: any) => {
      this.periocidades = resp;
    }, err => {
      this.spinner.hide();
    });
  }

  getHorarios() {
    this.cobranzaService.getHorasEnvio().subscribe((res: any) => {
      this.horarios = res.horarios;
    }, err => { });
  }

  getAutomatizaciones() {
    this.spinner.show();
    this.automatizacionService.getAutomatizaciones().subscribe((res: Automatizacion[]) => {
      this.listaAutomatizaciones = res;
      this.recordatorio = this.listaAutomatizaciones[0];
      this.estadoCuenta = this.listaAutomatizaciones[1];
      this.cobranza = this.listaAutomatizaciones[2];


      //recordatorio
      this.selectedAnioRecordatorio =  [];
      this.selectedCargosRecordatorio =  [];
      this.selectedCanalesVentaRecordatorio= [];
      this.selectedCobradoresRecordatorio= [];
      this.selectedVendedoresRecordatorio= [];
      this.selectedListasPrecioRecordatorio= [];
      this.selectedCategoriasClienteRecordatorio= [];
      this.selectedCondicionesVentaRecordatorio= [];

      //estado de cuenta
      this.selectedAnioEstadoCuenta= [];
      this.selectedCargosEstadoCuenta= [];
      this.selectedCanalesVentaEstadoCuenta= [];
      this.selectedCobradoresEstadoCuenta= [];
      this.selectedVendedoresEstadoCuenta= [];
      this.selectedListasPrecioEstadoCuenta= [];
      this.selectedCategoriasClienteEstadoCuenta= [];
      this.selectedCondicionesVentaEstadoCuenta= [];

      //cobranza
      this.selectedAnioCobranza= [];
      this.selectedCargosCobranza= [];
      this.selectedCanalesVentaCobranza= [];
      this.selectedCobradoresCobranza= [];
      this.selectedVendedoresCobranza= [];
      this.selectedListasPrecioCobranza= [];
      this.selectedCategoriasClienteCobranza= [];
      this.selectedCondicionesVentaCobranza= [];

      //recordatorio
      let tipoDocsRecordatorio: string[] = this.recordatorio.tipoDocumentos != null && this.recordatorio.tipoDocumentos != '' ? this.recordatorio.tipoDocumentos.split(';') : [];
      tipoDocsRecordatorio.forEach(element => {
        this.selectedTiposDocumentosRecordatorio.push(element);
      });

      let catCliRecordatorio: string[] = this.recordatorio.codCategoriaCliente != null && this.recordatorio.codCategoriaCliente != '' ? this.recordatorio.codCategoriaCliente.split(';') : [];
      catCliRecordatorio.forEach(element => {
        this.selectedCategoriasClienteRecordatorio.push(element);
      });

      let listasPrecioRecordatorio: string[] = this.recordatorio.codListaPrecios != null && this.recordatorio.codListaPrecios != '' ? this.recordatorio.codListaPrecios.split(';') : [];
      listasPrecioRecordatorio.forEach(element => {
        this.selectedListasPrecioRecordatorio.push(element);
      });

      let condicionVentaRecordatorio: string[] = this.recordatorio.codCondicionVenta != null && this.recordatorio.codCondicionVenta != '' ? this.recordatorio.codCondicionVenta.split(';') : [];
      condicionVentaRecordatorio.forEach(element => {
        this.selectedCondicionesVentaRecordatorio.push(element);
      });

      let vendedoresRecordatorio: string[] = this.recordatorio.codVendedor != null && this.recordatorio.codVendedor != '' ? this.recordatorio.codVendedor.split(';') : [];
      vendedoresRecordatorio.forEach(element => {
        this.selectedVendedoresRecordatorio.push(element);
      });

      let canalVentaRecordatiorio: string[] = this.recordatorio.codCanalVenta != null && this.recordatorio.codCanalVenta != '' ? this.recordatorio.codCanalVenta.split(';') : [];
      canalVentaRecordatiorio.forEach(element => {
        this.selectedCanalesVentaRecordatorio.push(element);
      });

      let cobradoresRecordatorio: string[] = this.recordatorio.codCobrador != null && this.recordatorio.codCobrador != '' ? this.recordatorio.codCobrador.split(';') : [];
      cobradoresRecordatorio.forEach(element => {
        this.selectedCobradoresRecordatorio.push(element);
      });

      let cargosRecordatorio: string[] = this.recordatorio.codCargo != null && this.recordatorio.codCargo != '' ? this.recordatorio.codCargo.split(';') : [];
      cargosRecordatorio.forEach(element => {
        this.selectedCargosRecordatorio.push(element);
      });

      this.selectedAnioRecordatorio = this.recordatorio.anio != null ? this.recordatorio.anio : 'TODOS';


      //estado de cuenta
      
      let tipoDocsEstadoCuenta: string[] = this.estadoCuenta.tipoDocumentos != null && this.estadoCuenta.tipoDocumentos != '' ? this.estadoCuenta.tipoDocumentos.split(';') : [];
      tipoDocsEstadoCuenta.forEach(element => {
        this.selectedTiposDocumentosEstadoCuenta.push(element);
      });

      let catCliEstadoCuenta: string[] = this.estadoCuenta.codCategoriaCliente != null && this.estadoCuenta.codCategoriaCliente != '' ? this.estadoCuenta.codCategoriaCliente.split(';') : [];
      catCliEstadoCuenta.forEach(element => {
        this.selectedCategoriasClienteEstadoCuenta.push(element);
      });

      let listasPrecioEstadoCuenta: string[] = this.estadoCuenta.codListaPrecios != null && this.estadoCuenta.codListaPrecios != '' ? this.estadoCuenta.codListaPrecios.split(';') : [];
      listasPrecioEstadoCuenta.forEach(element => {
        this.selectedListasPrecioEstadoCuenta.push(element);
      });

      let condicionVentaEstadoCuenta: string[] = this.estadoCuenta.codCondicionVenta != null && this.estadoCuenta.codCondicionVenta != '' ? this.estadoCuenta.codCondicionVenta.split(';') : [];
      condicionVentaEstadoCuenta.forEach(element => {
        this.selectedCondicionesVentaEstadoCuenta.push(element);
      });

      let vendedoresEstadoCuenta: string[] = this.estadoCuenta.codVendedor != null && this.estadoCuenta.codVendedor != '' ? this.estadoCuenta.codVendedor.split(';') : [];
      vendedoresEstadoCuenta.forEach(element => {
        this.selectedVendedoresEstadoCuenta.push(element);
      });

      let canalVentaEstadoCuenta: string[] = this.estadoCuenta.codCanalVenta != null && this.estadoCuenta.codCanalVenta != '' ? this.estadoCuenta.codCanalVenta.split(';') : [];
      canalVentaEstadoCuenta.forEach(element => {
        this.selectedCanalesVentaEstadoCuenta.push(element);
      });

      let cobradoresEstadoCuenta: string[] = this.estadoCuenta.codCobrador != null && this.estadoCuenta.codCobrador != '' ? this.estadoCuenta.codCobrador.split(';') : [];
      cobradoresEstadoCuenta.forEach(element => {
        this.selectedCobradoresEstadoCuenta.push(element);
      });

      let cargosEstadoCuenta: string[] = this.estadoCuenta.codCargo != null && this.estadoCuenta.codCargo != '' ? this.estadoCuenta.codCargo.split(';') : [];
      cargosEstadoCuenta.forEach(element => {
        this.selectedCargosEstadoCuenta.push(element);
      });

      this.selectedAnioEstadoCuenta = this.estadoCuenta.anio != null ? this.estadoCuenta.anio : 'TODOS';

      //cobranza
      let tipoDocsCobranza: string[] = this.cobranza.tipoDocumentos != null && this.cobranza.tipoDocumentos != '' ? this.cobranza.tipoDocumentos.split(';') : [];
      tipoDocsCobranza.forEach(element => {
        this.selectedTiposDocumentosCobranza.push(element);
      });

      let catCliCobranza: string[] = this.cobranza.codCategoriaCliente != null && this.cobranza.codCategoriaCliente != '' ? this.cobranza.codCategoriaCliente.split(';') : [];
      catCliCobranza.forEach(element => {
        this.selectedCategoriasClienteCobranza.push(element);
      });

      let listasPrecioCobranza: string[] = this.cobranza.codListaPrecios != null && this.cobranza.codListaPrecios != '' ? this.cobranza.codListaPrecios.split(';') : [];
      listasPrecioCobranza.forEach(element => {
        this.selectedListasPrecioCobranza.push(element);
      });

      let condicionVentaCobranza: string[] = this.cobranza.codCondicionVenta != null && this.cobranza.codCondicionVenta != '' ? this.cobranza.codCondicionVenta.split(';') : [];
      condicionVentaCobranza.forEach(element => {
        this.selectedCondicionesVentaCobranza.push(element);
      });

      let vendedoresCobranza: string[] = this.cobranza.codVendedor != null && this.cobranza.codVendedor != '' ? this.cobranza.codVendedor.split(';') : [];
      vendedoresCobranza.forEach(element => {
        this.selectedVendedoresCobranza.push(element);
      });

      let canalVentaCobranza: string[] = this.cobranza.codCanalVenta != null && this.cobranza.codCanalVenta != '' ? this.cobranza.codCanalVenta.split(';') : [];
      canalVentaCobranza.forEach(element => {
        this.selectedCanalesVentaCobranza.push(element);
      });

      let cobradoresCobranza: string[] = this.cobranza.codCobrador != null && this.cobranza.codCobrador != '' ? this.cobranza.codCobrador.split(';') : [];
      cobradoresCobranza.forEach(element => {
        this.selectedCobradoresCobranza.push(element);
      });

      let cargosCobranza: string[] = this.cobranza.codCargo != null && this.cobranza.codCargo != '' ? this.cobranza.codCargo.split(';') : [];
      cargosCobranza.forEach(element => {
        this.selectedCargosCobranza.push(element);
      });

      this.selectedAnioCobranza = this.cobranza.anio != null ? this.cobranza.anio : 'TODOS';


      this.loaded = true;
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener automatizaciones', '', true); });
  }



  async save(aut: Automatizacion) {

    let canalesVenta = '';
    let cobradores = '';
    let vendedores = '';
    let categoriasCliente = '';
    let listasPrecio = '';
    let condicionesVenta = '';
    let cargos = '';

    switch (aut.idTipoAutomatizacion) {
      case 1:

        if (this.selectedAnioRecordatorio == null) {
          this.notificationService.warning('Debe seleccionar un año', '', true);
          return;
        }

        if (this.selectedAnioRecordatorio == 'TODOS') {
          aut.anio = null
        } else {
          aut.anio = this.selectedAnioRecordatorio;
        }

        if (aut.diasVencimiento == null || aut.diasVencimiento.toString() == '') {
          this.notificationService.warning('Debe ingresar cantidad de Dias antes del vencimiento', '', true);
          return;
        }

        aut.tipoDocumentos = this.selectedTiposDocumentosRecordatorio.length > 0 ? this.selectedTiposDocumentosRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        canalesVenta = this.selectedCanalesVentaRecordatorio.length > 0 ? this.selectedCanalesVentaRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cobradores = this.selectedCobradoresRecordatorio.length > 0 ? this.selectedCobradoresRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        vendedores = this.selectedVendedoresRecordatorio.length > 0 ? this.selectedVendedoresRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        categoriasCliente = this.selectedCategoriasClienteRecordatorio.length > 0 ? this.selectedCategoriasClienteRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        listasPrecio = this.selectedListasPrecioRecordatorio.length > 0 ? this.selectedListasPrecioRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        condicionesVenta = this.selectedCondicionesVentaRecordatorio.length > 0 ? this.selectedCondicionesVentaRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cargos = this.selectedCargosRecordatorio.length > 0 ? this.selectedCargosRecordatorio.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        break;

      case 2:
        
        if (this.selectedAnioEstadoCuenta == null) {
          this.notificationService.warning('Debe seleccionar un año', '', true);
          return;
        }

        if (this.selectedAnioEstadoCuenta == 'TODOS') {
          aut.anio = null
        } else {
          aut.anio = this.selectedAnioEstadoCuenta;
        }

        if (aut.idPerioricidad == null || aut.idPerioricidad == 0) {
          this.notificationService.warning('Debe seleccionar periodicidad', '', true);
          return;
        }

        if ((aut.diaEnvio == null || aut.diaEnvio.toString() == '') && (aut.idPerioricidad == 4 || aut.idPerioricidad == 5)) {
          this.notificationService.warning('Debe ingresar dia de envio', '', true);
          return;
        } else if (aut.idPerioricidad != 4 && aut.idPerioricidad != 5) {
          aut.diaEnvio = null;
        }

        aut.tipoDocumentos = this.selectedTiposDocumentosEstadoCuenta.length > 0 ? this.selectedTiposDocumentosEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        canalesVenta = this.selectedCanalesVentaEstadoCuenta.length > 0 ? this.selectedCanalesVentaEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cobradores = this.selectedCobradoresEstadoCuenta.length > 0 ? this.selectedCobradoresEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        vendedores = this.selectedVendedoresEstadoCuenta.length > 0 ? this.selectedVendedoresEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        categoriasCliente = this.selectedCategoriasClienteEstadoCuenta.length > 0 ? this.selectedCategoriasClienteEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        listasPrecio = this.selectedListasPrecioEstadoCuenta.length > 0 ? this.selectedListasPrecioEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        condicionesVenta = this.selectedCondicionesVentaEstadoCuenta.length > 0 ? this.selectedCondicionesVentaEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cargos = this.selectedCargosEstadoCuenta.length > 0 ? this.selectedCargosEstadoCuenta.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        break;

      case 3:

        if (this.selectedAnioCobranza == null) {
          this.notificationService.warning('Debe seleccionar un año', '', true);
          return;
        }

        if (this.selectedAnioCobranza == 'TODOS') {
          aut.anio = null
        } else {
          aut.anio = this.selectedAnioCobranza;
        }

        if (aut.diasVencimiento == null || aut.diasVencimiento.toString() == '') {
          this.notificationService.warning('Debe ingresar cantidad de Dias despues del vencimiento', '', true);
          return;
        }

        if (aut.muestraSoloVencidos == null || aut.muestraSoloVencidos.toString() == '') {
          this.notificationService.warning('Debe seleccionar Mostrar en pago', '', true);
          return;
        }

        // if (aut.diasRecordatorio == null || aut.diasRecordatorio.toString() == '') {
        //   this.notificationService.warning('Debe ingresar Días Recordatorio', '', true);
        //   return;
        // }

        if (aut.idPerioricidad == null || aut.idPerioricidad == 0) {
          this.notificationService.warning('Debe seleccionar periodicidad', '', true);
          return;
        }

        if ((aut.diaEnvio == null || aut.diaEnvio.toString() == '') && (aut.idPerioricidad == 4 || aut.idPerioricidad == 5)) {
          this.notificationService.warning('Debe ingresar dia de envio', '', true);
          return;
        } else if (aut.idPerioricidad != 4 && aut.idPerioricidad != 5) {
          aut.diaEnvio = null;
        }

        aut.tipoDocumentos = this.selectedTiposDocumentosCobranza.length > 0 ? this.selectedTiposDocumentosCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        canalesVenta = this.selectedCanalesVentaCobranza.length > 0 ? this.selectedCanalesVentaCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cobradores = this.selectedCobradoresCobranza.length > 0 ? this.selectedCobradoresCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        vendedores = this.selectedVendedoresCobranza.length > 0 ? this.selectedVendedoresCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        categoriasCliente = this.selectedCategoriasClienteCobranza.length > 0 ? this.selectedCategoriasClienteCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        listasPrecio = this.selectedListasPrecioCobranza.length > 0 ? this.selectedListasPrecioCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        condicionesVenta = this.selectedCondicionesVentaCobranza.length > 0 ? this.selectedCondicionesVentaCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        cargos = this.selectedCargosCobranza.length > 0 ? this.selectedCargosCobranza.reduce((accumulator, item) => {
          return `${accumulator};${item}`;
        }) : null;

        break;
    }

    aut.enviaCorreoFicha = aut.enviaCorreoFicha ? 1 : 0;
    aut.enviaTodosContactos = aut.enviaTodosContactos ? 1 : 0;


    if (aut.idHorario == null) {
      this.notificationService.warning('Debe seleccionar un Horario de envio', '', true);
      return;
    }

    // if (aut.tipoDocumentos == null || aut.tipoDocumentos == '') {
    //   this.notificationService.warning('Debe seleccionar tipos de documentos', '', true);
    //   return;
    // }

    // if (categoriasCliente == null || categoriasCliente == '') {
    //   this.notificationService.warning('Debe seleccionar almenos una categoria cliente', '', true);
    //   return;
    // }

    // if (listasPrecio == null || listasPrecio == '') {
    //   this.notificationService.warning('Debe seleccionar almenos una lista de precio', '', true);
    //   return;
    // }

    // if (condicionesVenta == null || condicionesVenta == '') {
    //   this.notificationService.warning('Debe seleccionar almenos una condicion de venta', '', true);
    //   return;
    // }

    // if (vendedores == null || vendedores == '') {
    //   this.notificationService.warning('Debe seleccionar almenos un vendedor', '', true);
    //   return;
    // }

    // if (canalesVenta == null || canalesVenta == '') {
    //   this.notificationService.warning('Debe seleccionar almenos un canal de venta', '', true);
    //   return;
    // }

    // if (cobradores == null || cobradores == '') {
    //   this.notificationService.warning('Debe seleccionar almenos un cobrador', '', true);
    //   return;
    // }

    // if (cargos == null || cargos == '' && aut.enviaTodosContactos == 0) {
    //   this.notificationService.warning('Debe seleccionar almenos un cargo', '', true);
    //   return;
    // }

    aut.agrupaCobranza = aut.agrupaCobranza ? 1 : 0;
    aut.estado = aut.estado ? 1 : 0;
    aut.excluyeClientes = aut.excluyeClientes ? 1 : 0;
    aut.excluyeFestivos = aut.excluyeFestivos ? 1 : 0;
    aut.codCategoriaCliente = categoriasCliente;
    aut.codListaPrecios = listasPrecio;
    aut.codCondicionVenta = condicionesVenta;

    aut.codVendedor = vendedores;
    // aut.codCanalVenta = canalesVenta;
    // aut.codCobrador = cobradores;
    aut.codCargo = cargos;



    this.spinner.show();

    this.automatizacionService.save(aut).subscribe((res: any) => {
      this.notificationService.success('Automatización guardada correctamente', '', true);
      this.getAutomatizaciones();
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar Automatización', '', true); });

  }

  MostrarSeccion(seccion: number) {

    switch (seccion) {
      case 1:
        if (this.muestaRecordatorio == 0) {
          this.muestaRecordatorio = 1;
        } else {
          this.muestaRecordatorio = 0;
        }
        break;

      case 2:
        if (this.muestraEstadoCuenta == 0) {
          this.muestraEstadoCuenta = 1;
        } else {
          this.muestraEstadoCuenta = 0;
        }
        break;

      case 3:
        if (this.muestraCobranza == 0) {
          this.muestraCobranza = 1;
        } else {
          this.muestraCobranza = 0;
        }
        break;
    }
  }

  validaNumeros() {
    if (this.estadoCuenta.diaEnvio > 30) {
      this.estadoCuenta.diaEnvio = 30;
    }
  }

  changePeriodicidad(tipoAutometizacion: number) {
    switch (tipoAutometizacion) {
      case 1:
        this.recordatorio.diaEnvio = null;
        break;
      case 2:
        this.estadoCuenta.diaEnvio = null;
        break;

      case 3:
        this.cobranza.diaEnvio = null;
        break;
    }
  }

  enviaAutomatizaciones() {
    this.spinner.show();
    this.automatizacionService.enviaAutomatizaciones().subscribe((res: any) => {
      this.spinner.hide();
    }, err => {
      this.spinner.hide();
    });
  }
}
