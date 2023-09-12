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
  public noResultsText: string = 'No se encontraron automatizaciones';

  public condicionesVenta: CondicionVentaDTO[] = [];
  public listasPrecio: ListaPrecioDTO[] = [];
  public categoriasCliente: CategoriaClienteDTO[] = [];
  public vendedores: VendedorDTO[] = [];
  public tiposDocumento: TipoDocumento[] = [];
  public selectedTiposDocumentos: any = []
  public mostrarEnPago: any[] = [{ id: 0, nombre: 'Estado de cuenta' }, { id: 1, nombre: 'Solo documentos vencidos' }];
  public diasSemana: any[] = [{ id: 1, nombre: 'Lunes' }, { id: 2, nombre: 'Martes' }, { id: 3, nombre: 'Miércoles' }, { id: 4, nombre: 'Jueves' }, { id: 5, nombre: 'Viernes' }, { id: 6, nombre: 'Sábado' }, { id: 0, nombre: 'Domingo' }];
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
  esCreacion: boolean = false;

  selectedAnio: any = null;
  selectedCargos: any = [];
  selectedCanalesVenta: any = [];
  selectedCobradores: any = [];
  selectedVendedores: any = [];
  selectedListasPrecio: any = [];
  selectedCategoriasCliente: any = [];
  selectedCondicionesVenta: any = [];
  cargos: any = [];

  selectedTipoFiltro: number = 0;
  selectedEstadoFiltro: number = 3;
  public estadosFiltro: any[] = [{ id: 3, nombre: 'TODOS' }, { id: 1, nombre: 'ACTIVOS' }, { id: 0, nombre: 'INACTIVOS' }];
  public tiposAutomatizacionesFiltro: TipoAutomatizacion[] = [{nombre: 'TODOS', idTipo: 0}];
  public config: any;
  public p: number = 1;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  constructor(private spinner: NgxSpinnerService, private cobranzaService: CobranzasService, private softlandService: SoftlandService,
    private modalService: NgbModal, private automatizacionService: AutomatizacionService, private configSoftlandService: ConfiguracionSoftlandService,
    private notificationService: NotificationService,
    private authService: AuthService, private condicionVentaService: CondicionVentaService) {
    ;
  }

  ngOnInit() {
    this.getCategoriasCliente();
    //this.getListasPrecio();
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
      this.canalesVenta.forEach(element => {
        element.canDes = element.canCod + ' - ' + element.canDes;
      });
      if(this.canalesVenta.length == 0){
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
      if(this.cobradores.length == 0){
        this.cobradores.push({ cobDes: 'Sin Datos', cobCod: '', disabled: true })
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar canales de venta', '', true); });
  }

  getcargos() {
    this.configSoftlandService.getCargos().subscribe((res: any) => {
      this.cargos = res;
      this.cargos.forEach(element => {
        element.carNom = element.carCod + ' - ' + element.carNom;
      });
      if(this.cargos.length == 0){
        this.cargos.push({ carNom: 'Sin Datos', carCod: '', disabled: true })
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar cargos', '', true); });
  }



  getCategoriasCliente() {
    this.spinner.show();
    this.softlandService.getCategoriasCliente().subscribe((resp: CategoriaClienteDTO[]) => {
      this.categoriasCliente = resp;
      this.categoriasCliente.forEach(element => {
        element.catDes = element.catCod + ' - ' + element.catDes;
      });

      if(this.categoriasCliente.length == 0){
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
      if(this.tiposDocumento.length == 0){
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
      if(this.listasPrecio.length == 0){
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
      if(this.vendedores.length == 0){
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
      if(this.condicionesVenta.length == 0){
        this.condicionesVenta.push({ cveDes: 'Sin Datos', cveCod: '', disabled: true })
      }
    }, err => {
      this.spinner.hide();
    });
  }

  getTiposAutomatizaciones() {
    this.spinner.show();
    this.automatizacionService.getTipoAutomatizaciones().subscribe((resp: TipoAutomatizacion[]) => {
      this.tiposAutomatizaciones = resp; 
      resp.forEach(element => {
        this.tiposAutomatizacionesFiltro.push(element);
      });
     
    }, err => {
      this.spinner.hide();
    });
  }
  volver(){
    if(this.esCreacion){
      this.selectedAnio = null;
      this.selectedCargos = [];
      this.selectedCanalesVenta = [];
      this.selectedCobradores = [];
      this.selectedVendedores = [];
      this.selectedListasPrecio = [];
      this.selectedCategoriasCliente = [];
      this.selectedCondicionesVenta = [];
      this.esCreacion = false;
      this.getAutomatizaciones();
    }else{
      this.esCreacion = true;
    }
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
    this.paginador.estado = this.selectedEstadoFiltro == 3 ? null : this.selectedEstadoFiltro;
    this.paginador.tipo = this.selectedTipoFiltro == 0 ? null : this.selectedTipoFiltro;
    this.automatizacionService.getAutomatizacionesByPage(this.paginador).subscribe((res: Automatizacion[]) => {
      this.listaAutomatizaciones = res;
      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.listaAutomatizaciones.length > 0 ? this.listaAutomatizaciones[0].totalFilas : 0
      };
      this.listaAutomatizaciones.forEach(element => {
        debugger
        let horario = this.horarios.filter(x => x.idHorario == element.idHorario);
        if(horario.length > 0){
          element.horaEnvio = horario[0].horario;
        }
      });
      this.loaded = true;
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener automatizaciones', '', true); });
  }



  async save() {

    let canalesVenta = '';
    let cobradores = '';
    let vendedores = '';
    let categoriasCliente = '';
    let listasPrecio = '';
    let condicionesVenta = '';
    let cargos = '';
    if(this.nuevaAutomaetizacion.nombre == null || this.nuevaAutomaetizacion.nombre == ''){
      this.notificationService.warning('Debe ingresar un nombre', '', true);
      return;
    }

    
    if (this.nuevaAutomaetizacion.idPerioricidad == null || this.nuevaAutomaetizacion.idPerioricidad == 0) {
      this.notificationService.warning('Debe seleccionar periodicidad', '', true);
      return;
    }

    if ((this.nuevaAutomaetizacion.diaEnvio == null || this.nuevaAutomaetizacion.diaEnvio.toString() == '' || this.nuevaAutomaetizacion.diaEnvio == 0) && (this.nuevaAutomaetizacion.idPerioricidad == 4 || this.nuevaAutomaetizacion.idPerioricidad == 5)) {
      this.notificationService.warning('Debe ingresar dia de envío', '', true);
      return;
    } else if (this.nuevaAutomaetizacion.idPerioricidad != 4 && this.nuevaAutomaetizacion.idPerioricidad != 5) {
      this.nuevaAutomaetizacion.diaEnvio = null;
    }

    switch (this.nuevaAutomaetizacion.idTipoAutomatizacion) {
      case 1:

        // if (this.selectedAnio == null) {
        //   this.notificationService.warning('Debe seleccionar un año', '', true);
        //   return;
        // }

        // if (this.selectedAnio == 'TODOS') {
        //   this.nuevaAutomaetizacion.anio = null
        // } else {
        //   this.nuevaAutomaetizacion.anio = this.selectedAnio;
        // }

        if (this.nuevaAutomaetizacion.diasVencimiento == null || this.nuevaAutomaetizacion.diasVencimiento.toString() == '') {
          this.notificationService.warning('Debe ingresar cantidad de Dias antes del vencimiento', '', true);
          return;
        }



        break;

      case 2:


        break;

      case 3:

        if (this.nuevaAutomaetizacion.diasVencimiento == null || this.nuevaAutomaetizacion.diasVencimiento.toString() == '') {
          this.notificationService.warning('Debe ingresar cantidad de Días después del vencimiento', '', true);
          return;
        }

        if (this.nuevaAutomaetizacion.muestraSoloVencidos == null || this.nuevaAutomaetizacion.muestraSoloVencidos.toString() == '') {
          this.notificationService.warning('Debe seleccionar Mostrar en pago', '', true);
          return;
        }


        break;
    }

    this.nuevaAutomaetizacion.tipoDocumentos = this.selectedTiposDocumentos.length > 0 ? this.selectedTiposDocumentos.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    canalesVenta = this.selectedCanalesVenta.length > 0 ? this.selectedCanalesVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    cobradores = this.selectedCobradores.length > 0 ? this.selectedCobradores.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    vendedores = this.selectedVendedores.length > 0 ? this.selectedVendedores.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    categoriasCliente = this.selectedCategoriasCliente.length > 0 ? this.selectedCategoriasCliente.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    listasPrecio = this.selectedListasPrecio.length > 0 ? this.selectedListasPrecio.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    condicionesVenta = this.selectedCondicionesVenta.length > 0 ? this.selectedCondicionesVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    cargos = this.selectedCargos.length > 0 ? this.selectedCargos.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    this.nuevaAutomaetizacion.enviaCorreoFicha = this.nuevaAutomaetizacion.enviaCorreoFicha ? 1 : 0;
    this.nuevaAutomaetizacion.enviaTodosContactos = this.nuevaAutomaetizacion.enviaTodosContactos ? 1 : 0;


    if (this.nuevaAutomaetizacion.idHorario == null) {
      this.notificationService.warning('Debe seleccionar un Horario de envío', '', true);
      return;
    }

    this.nuevaAutomaetizacion.agrupaCobranza = this.nuevaAutomaetizacion.agrupaCobranza ? 1 : 0;
    this.nuevaAutomaetizacion.estado = this.nuevaAutomaetizacion.estado ? 1 : 0;
    this.nuevaAutomaetizacion.excluyeClientes = this.nuevaAutomaetizacion.excluyeClientes ? 1 : 0;
    this.nuevaAutomaetizacion.excluyeFestivos = this.nuevaAutomaetizacion.excluyeFestivos ? 1 : 0;
    this.nuevaAutomaetizacion.codCategoriaCliente = categoriasCliente;
    this.nuevaAutomaetizacion.codListaPrecios = listasPrecio;
    this.nuevaAutomaetizacion.codCondicionVenta = condicionesVenta;

    this.nuevaAutomaetizacion.codVendedor = vendedores;
    this.nuevaAutomaetizacion.codCargo = cargos;



    this.spinner.show();

    if (this.nuevaAutomaetizacion.idAutomatizacion != 0) {
      this.automatizacionService.save(this.nuevaAutomaetizacion).subscribe((res: any) => {
        this.notificationService.success('Automatización guardada correctamente', '', true);
        this.selectedAnio = null;
        this.selectedCargos = [];
        this.selectedCanalesVenta = [];
        this.selectedCobradores = [];
        this.selectedVendedores = [];
        this.selectedListasPrecio = [];
        this.selectedCategoriasCliente = [];
        this.selectedCondicionesVenta = [];
        this.esCreacion = false;
        this.getAutomatizaciones();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar Automatización', '', true); });
    } else {
      this.automatizacionService.create(this.nuevaAutomaetizacion).subscribe((res: any) => {
        this.notificationService.success('Automatización guardada correctamente', '', true);
        this.selectedAnio = null;
        this.selectedCargos = [];
        this.selectedCanalesVenta = [];
        this.selectedCobradores = [];
        this.selectedVendedores = [];
        this.selectedListasPrecio = [];
        this.selectedCategoriasCliente = [];
        this.selectedCondicionesVenta = [];
        this.esCreacion = false;
        this.getAutomatizaciones();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar Automatización', '', true); });
    }
  }

  validaNumeros() {
    if (this.nuevaAutomaetizacion.diaEnvio > 31) {
      this.nuevaAutomaetizacion.diaEnvio = 30;
    }
    if (this.nuevaAutomaetizacion.diaEnvio < 1) {
      this.nuevaAutomaetizacion.diaEnvio = 1;
    }
  }

  changePeriodicidad() {
    this.nuevaAutomaetizacion.diaEnvio = null;
  }

  async enviaAutomatizaciones() {
    const response = await this.notificationService.confirmation('Ejecutar Envío', 'Se ejecutará el proceso de envio, esto puede tardar varios minutos dependiendo de la cantidad de documentos a enviar. ¿Desea continuar?');
    if (response.isConfirmed) {
      this.spinner.show();
      this.spinner.hide();
      this.automatizacionService.enviaAutomatizaciones().subscribe((res: any) => {
      }, err => {
        this.spinner.hide();
      });
    }
    
  }


  openModal(aut: Automatizacion) {
    if (aut != null) {
      this.modalTitle = 'Editar Automatización';
      this.nuevaAutomaetizacion = aut;
      let tipoDocsCobranza: string[] = this.nuevaAutomaetizacion.tipoDocumentos != null && this.nuevaAutomaetizacion.tipoDocumentos != '' ? this.nuevaAutomaetizacion.tipoDocumentos.split(';') : [];
      tipoDocsCobranza.forEach(element => {
        if (this.selectedTiposDocumentos.filter(x => x == element).length == 0) {
          this.selectedTiposDocumentos.push(element);
        }
      });

      let catCliCobranza: string[] = this.nuevaAutomaetizacion.codCategoriaCliente != null && this.nuevaAutomaetizacion.codCategoriaCliente != '' ? this.nuevaAutomaetizacion.codCategoriaCliente.split(';') : [];
      catCliCobranza.forEach(element => {
        if (this.selectedCategoriasCliente.filter(x => x == element).length == 0) {
          this.selectedCategoriasCliente.push(element);
        }
      });

      let listasPrecioCobranza: string[] = this.nuevaAutomaetizacion.codListaPrecios != null && this.nuevaAutomaetizacion.codListaPrecios != '' ? this.nuevaAutomaetizacion.codListaPrecios.split(';') : [];
      listasPrecioCobranza.forEach(element => {
        if (this.selectedListasPrecio.filter(x => x == element).length == 0) {
          this.selectedListasPrecio.push(element);
        }
      });

      let condicionVentaCobranza: string[] = this.nuevaAutomaetizacion.codCondicionVenta != null && this.nuevaAutomaetizacion.codCondicionVenta != '' ? this.nuevaAutomaetizacion.codCondicionVenta.split(';') : [];
      condicionVentaCobranza.forEach(element => {
        if (this.selectedCondicionesVenta.filter(x => x == element).length == 0) {
          this.selectedCondicionesVenta.push(element);
        }
      });

      let vendedoresCobranza: string[] = this.nuevaAutomaetizacion.codVendedor != null && this.nuevaAutomaetizacion.codVendedor != '' ? this.nuevaAutomaetizacion.codVendedor.split(';') : [];
      vendedoresCobranza.forEach(element => {
        if (this.selectedVendedores.filter(x => x == element).length == 0) {
          this.selectedVendedores.push(element);
        }
      });

      let canalVentaCobranza: string[] = this.nuevaAutomaetizacion.codCanalVenta != null && this.nuevaAutomaetizacion.codCanalVenta != '' ? this.nuevaAutomaetizacion.codCanalVenta.split(';') : [];
      canalVentaCobranza.forEach(element => {
        if (this.selectedCanalesVenta.filter(x => x == element).length == 0) {
          this.selectedCanalesVenta.push(element);
        }
      });

      let cobradoresCobranza: string[] = this.nuevaAutomaetizacion.codCobrador != null && this.nuevaAutomaetizacion.codCobrador != '' ? this.nuevaAutomaetizacion.codCobrador.split(';') : [];
      cobradoresCobranza.forEach(element => {
        if (this.selectedCobradores.filter(x => x == element).length == 0) {
          this.selectedCobradores.push(element);
        }
      });

      let cargosCobranza: string[] = this.nuevaAutomaetizacion.codCargo != null && this.nuevaAutomaetizacion.codCargo != '' ? this.nuevaAutomaetizacion.codCargo.split(';') : [];
      cargosCobranza.forEach(element => {
        if (this.selectedCargos.filter(x => x == element).length == 0) {
          this.selectedCargos.push(element);
        }
      });

      this.selectedAnio = this.nuevaAutomaetizacion.anio != null && this.nuevaAutomaetizacion.anio != 0 ? this.nuevaAutomaetizacion.anio : 'TODOS';
    } else {
      this.nuevaAutomaetizacion = new Automatizacion();
      this.nuevaAutomaetizacion.excluyeClientes = 1;
      this.modalTitle = 'Nueva Automatización';


    }
    this.esCreacion = true;
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';

    this.getAutomatizaciones()
  }

  limpiarFiltros(){
    this.p = 1;
    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.search = ''; 
    this.selectedEstadoFiltro = 3;
    this.selectedTipoFiltro = 0;
    this.getAutomatizaciones();
  }


  delete(idAutomatizacion: number){
    this.spinner.show();
    this.automatizacionService.delete(idAutomatizacion).subscribe((res: any) => {
      this.notificationService.success('Automatización eliminada correctamente', '', true);
      this.getAutomatizaciones();
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al eliminar Automatización', '', true); });
  }

}
