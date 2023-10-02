import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { AuthService } from 'src/app/shared/services/auth.service';
import { ConfiguracionService } from 'src/app/shared/services/configuracion.service';
import { Utils } from 'src/app/shared/utils';
import { ImplementacionService } from 'src/app/shared/services/implementacion.service';
import { EmpresaImplementacion, Tenant } from 'src/app/shared/models/empresaimplementacion.model';

export interface IPreviewImgs {
  url?: string;
}
interface AutoCompleteModel {
  value: any;
  display: string;
}

@Component({
  selector: 'app-empresas',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss'],
  animations: [SharedAnimations]
})
export class CompanyComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = '';
  public noResultsText: string = '';
  public selectedEstado: number = 0;
  public esCreacion: number = 0;
  empresas: any[] = [];
  implementadores: any[] = [];
  areasComerciales: any[] = [];
  lineasProductos: any[] = [];
  empresaImplementacion: EmpresaImplementacion = new EmpresaImplementacion();
  tenant: Tenant = new Tenant();
  planes: any[] = [];
  tiposCliente: any[] = [{ id: 0, nombre: 'On Premise' }, { id: 1, nombre: 'Cloud' }]
  empresaTenants: any = null;
  conexionApiValid: boolean = false;
  conexionBaseDatosValid: boolean = false;
  cuentasContables: any[] = [];
  tiposDocumentos: any[] = [];
  cuentasContablesPago: any[] = [];
  selectedCuentasContables: any[] = [];
  selectedTiposDocumentos: any[] = [];
  public verContraseña: number = 0; //FCA 10-03-2022
  public icon: string = 'assets/images/icon/view.png'
  public verContraseñaCorreo: number = 0; //FCA 10-03-2022
  public iconClaveCorreo: string = 'assets/images/icon/view.png'
  public verApiKey: number = 0; //FCA 10-03-2022
  public iconApiKey: string = 'assets/images/icon/view.png'
  public loaded: boolean = false;
  public totalItems: number = 0;
  public config: any;
  public p: number = 1;
  public paginador: any = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: '',
    rutEmpresa: '',
    implementador: 0,
    estado: 0,
    cantidad: 10,
    pagina: 1
  };
  correo1: AutoCompleteModel[] = [];
  general: number = 0;
  pagos: number = 0;
  activacion: number = 0;
  cambioDatos: number = 0;
  actualizaClave: number = 0;
  actualizaCorreo: number = 0;
  recuperaClave: number = 0;
  envioDocumentos: number = 0;
  notificacionPago: number = 0;
  notificacionPagoSinComprobante: number = 0;
  cobranza: number = 0;
  preCobranza: number = 0;
  estadoCuenta: number = 0;
  public defaultImageLogoPortada: FileList = null;
  public urlImagenLogoPortada: IPreviewImgs = null;


  inicioDeSesion: number = 0;
  sidebar: number = 0;
  pagoRapido: number = 0;
  dashboardCliente: number = 0;
  misCompras: number = 0;
  miPerfil: number = 0;
  public defaultImageLogoPortadaDiseno: FileList = null;
  public urlImagenLogoPortadaDiseno: IPreviewImgs = null;
  public defaultImagenPortada: FileList = null;
  public urlImagenPortada: IPreviewImgs = null;
  public defaultImageLogoPrincipalSidebar: FileList = null;
  public urlImagenLogoPrincipalSidebar: IPreviewImgs = null;
  public defaultImageLogoSecundarioSidebar: FileList = null;
  public urlImagenLogoSecundarioSidebar: IPreviewImgs = null;
  public defaultImageBannerPagoRapido: FileList = null;
  public urlImagenBannerPagoRapido: IPreviewImgs = null;
  public defaultImageUltimasCompras: FileList = null;
  public urlImagenUltimasCompras: IPreviewImgs = null;
  public defaultImageMisCompras: FileList = null;
  public urlImagenMisCompras: IPreviewImgs = null;
  public defaultImageBannerMisCompras: FileList = null;
  public urlImagenBannerMisCompras: IPreviewImgs = null;
  public defaultImageBannerPerfil: FileList = null;
  public urlImagenBannerPerfil: IPreviewImgs = null;
  public defaultImagenUsuario: FileList = null;
  public urlImagenUsuario: IPreviewImgs = null;
  public defaultImageIconoContactos: FileList = null;
  public urlImagenIconoContactos: IPreviewImgs = null;
  public defaultImageIconoClavePerfil: FileList = null;
  public urlImagenIconoClavePerfil: IPreviewImgs = null;
  public defaultImageIconoEditarPerfil: FileList = null;
  public urlImagenIconoEditarPerfil: IPreviewImgs = null;
  public defaultImageIconoEstadoPerfil: FileList = null;
  public urlImagenIconoEstadoPerfil: IPreviewImgs = null;
  public defaultImageFavicon: FileList = null;
  public urlImagenFavicon: IPreviewImgs = null;

  constructor(
    private spinner: NgxSpinnerService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private authService: AuthService, private configService: ConfiguracionService, private utils: Utils, private implementacionService: ImplementacionService) {
  }

  ngOnInit() {
    this.spinner.show();
    this.getAreasComerciales();
    this.getImplementadores();
    this.getLineasProductos();
    this.getPlanes();
    this.getEmpresas();

  }

  openModal(content: any, empresa: any) {
    if (empresa == null) {
      this.modalTitle = 'Nueva Empresa'
      this.empresaImplementacion = new EmpresaImplementacion();
    } else {
      this.modalTitle = 'Editar Empresa'
      this.empresaImplementacion = empresa;
    }
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  openModalTenants(content: any, empresa: any) {
    this.modalTitle = 'Tenants'
    this.empresaTenants = empresa
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', windowClass: 'modal-xl' });
  }

  getImplementadores() {
    this.implementacionService.getImplementadores().subscribe((res: any[]) => {

      this.implementadores = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener implementadores', '', true); });
  }

  getAreasComerciales() {
    this.implementacionService.getAreasComerciales().subscribe((res: any[]) => {

      this.areasComerciales = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener areas comerciales', '', true); });
  }


  getLineasProductos() {
    this.implementacionService.getLineasProductos().subscribe((res: any[]) => {

      this.lineasProductos = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener linea de productos', '', true); });
  }

  getPlanes() {
    this.implementacionService.getPlanes().subscribe((res: any[]) => {

      this.planes = res;
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener planes', '', true); });
  }

  getEmpresas() {
    this.spinner.show();
    this.implementacionService.getEmpresasByPage(this.paginador).subscribe((res: any[]) => {
      debugger
      this.empresas = res;
      this.config = {
        itemsPerPage: this.paginador.endRow,
        currentPage: this.paginador.pagina,
        totalItems: this.empresas.length > 0 ? this.empresas[0].totalFilas : 0
      };
      this.loaded = true;
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener empresas', '', true); });
  }

  validaRut() {
    if (this.paginador.rutEmpresa != "" && this.paginador.rutEmpresa != null) {
      if (this.utils.isValidRUT(this.paginador.rutEmpresa)) {
        this.paginador.rutEmpresa = this.utils.checkRut(this.paginador.rutEmpresa);
      } else {
        this.paginador.rutEmpresa = '';
        this.notificationService.warning('Rut ingresado no es valido', '', true);
      }
    }
  }

  validaUrlPortal() {
    if (this.tenant.datosImplementacion.configuracionEmpresa.urlPortal != '' && this.tenant.datosImplementacion.configuracionEmpresa.urlPortal != null) {
      let pattern = /^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$/
      if (!pattern.test(this.tenant.datosImplementacion.configuracionEmpresa.urlPortal)) {
        this.tenant.datosImplementacion.configuracionEmpresa.urlPortal = '';
        this.notificationService.warning('Url no tiene un formato valido', '', true);
      }

    }

  }


  validaRutEmpresaImplementacion() {
    if (this.empresaImplementacion.rut != "" && this.empresaImplementacion.rut != null) {
      if (this.utils.isValidRUT(this.empresaImplementacion.rut)) {
        this.empresaImplementacion.rut = this.utils.checkRut(this.empresaImplementacion.rut);
      } else {
        this.empresaImplementacion.rut = '';
        this.notificationService.warning('Rut ingresado no es valido', '', true);
      }
    }
  }

  limpiarFiltros() {
    this.paginador.search = '';
    this.paginador.rutEmpresa = '';
    this.paginador.implementador = 0;
    this.paginador.estado = 0;
    this.paginador.pagina = 1;
    this.p = 1;
    this.getEmpresas();
  }

  filtrar() {
    this.p = 1;
    this.paginador.pagina = 1;
    this.getEmpresas();
  }

  changePage(event: any) {
    this.p = event;
    this.paginador.pagina = event;
    this.getEmpresas();
  }

  save() {
    if (this.empresaImplementacion.rut == '' || this.empresaImplementacion.rut == null) {
      this.notificationService.warning('Debe ingresar un rut', '', true);
      return;
    }
    if (this.empresaImplementacion.razonSocial == '' || this.empresaImplementacion.razonSocial == null) {
      this.notificationService.warning('Debe ingresar razón social', '', true);
      return;
    }

    if (this.empresaImplementacion.tipoCliente == 0 || this.empresaImplementacion.tipoCliente == null) {
      this.notificationService.warning('Debe ingresar tipo de cliente', '', true);
      return;
    }
    if (this.empresaImplementacion.idEmpresa == 0) {
      this.spinner.show();
      this.implementacionService.crearEmpresa(this.empresaImplementacion).subscribe((res: any) => {
        this.getEmpresas();
        this.modalService.dismissAll();
        this.notificationService.success('Empresa creada correctamente', '', true);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al crear empresa', '', true); });
    } else {
      this.spinner.show();
      this.implementacionService.editarEmpresa(this.empresaImplementacion).subscribe((res: any) => {
        this.getEmpresas();
        this.modalService.dismissAll();
        this.notificationService.success('Empresa editada correctamente', '', true);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar empresa', '', true); });
    }
  }

  async eliminar(idEmpresa: number) {
    const response = await this.notificationService.confirmation('Eliminar Empresa', 'Se eliminara la empresa, ¿Desea continuar?');
    if (response.isConfirmed) {
      this.spinner.show();

      this.implementacionService.eliminarEmpresa(idEmpresa).subscribe((res: any) => {
        this.getEmpresas();
        this.modalService.dismissAll();
        this.notificationService.success('Empresa eliminada correctamente', '', true);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar empresa', '', true); });
    }

  }

  nuevaImplementacion(content: any, tenant: any) {
    this.conexionApiValid = false;
    this.conexionBaseDatosValid = false;
    this.selectedCuentasContables = null;
    this.selectedTiposDocumentos = null;
    this.correo1 = [];
    if (tenant != null) {
      this.tenant = tenant;
    } else {
      debugger
      this.tenant = new Tenant();
      this.tenant.rutEmpresa = this.empresaTenants.rut;
      this.tenant.nombreEmpresa = this.empresaTenants.razonSocial;
    }
    this.llenaDatosTenant(content);
  }

  validaEmail() {
    if (!this.utils.validateMail(this.tenant.correoImplementador)) {
      this.notificationService.warning('Debe ingresar un correo válido.', '', true);
      this.tenant.correoImplementador = '';
      return;
    }
  }

  generaToken() {
    this.tenant.datosImplementacion.apiSoftland.token = 'PX_tcNEzdLWFAbcfmg5ZWtx2faHgLOBymv2KjtGmhzGeqAXH5Si4t4l5WuURRpb4ZvzVCnIHBKR7RxgrtxpWhzbZGIIZUldYatX8nhQ4gqk1iP8h0h3BG3Zjfrp9D4kErxE_' + this.tenant.datosImplementacion.apiSoftland.areaDatos
  }

  validaUrlApi() {
    if (this.tenant.datosImplementacion.apiSoftland.url != '' && this.tenant.datosImplementacion.apiSoftland.url != null) {
      let pattern = /^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$/
      if (!pattern.test(this.tenant.datosImplementacion.apiSoftland.url)) {
        this.tenant.datosImplementacion.apiSoftland.url = '';
        this.notificationService.warning('Url no tiene un formato valido', '', true);
      }

    }

  }

  validarConexionApi() {
    this.spinner.show();
    this.implementacionService.validaConexionApi(this.tenant.datosImplementacion.apiSoftland).subscribe((res: boolean) => {
      this.conexionApiValid = res;
      if (this.conexionApiValid) {
        this.notificationService.success('Conexión exitosa', '', true);
      } else {
        this.notificationService.warning('No se pudo realizar la conexión, compruebe los datos y reintente', '', true);
      }

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al validar conexión api', '', true); });
  }

  validaConexionBaseDatos() {
    this.spinner.show();
    this.implementacionService.validaConexionBd(this.tenant.datosImplementacion).subscribe((res: boolean) => {
      this.conexionBaseDatosValid = res;
      if (this.conexionBaseDatosValid) {
        this.notificationService.success('Conexión exitosa', '', true);
      } else {
        this.notificationService.warning('No se pudo realizar la conexión, compruebe los datos y reintente', '', true);
      }

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al validar conexión a la base de datos', '', true); });
  }

  verPass() {
    if (this.verContraseña == 1) {
      this.verContraseña = 0;
    } else {
      this.verContraseña = 1;
    }


    if (this.verContraseña == 0) {
      this.icon = 'assets/images/icon/view.png';
      document.getElementsByName("clavePortal")[0].setAttribute('type', 'password');
    } else {
      this.icon = 'assets/images/icon/invisible.png';
      document.getElementsByName("clavePortal")[0].setAttribute('type', 'text');
    }
  }

  verPassCorreo() {
    if (this.verContraseñaCorreo == 1) {
      this.verContraseñaCorreo = 0;
    } else {
      this.verContraseñaCorreo = 1;
    }


    if (this.verContraseñaCorreo == 0) {
      this.iconClaveCorreo = 'assets/images/icon/view.png';
      document.getElementsByName("claveCorreoPortal")[0].setAttribute('type', 'password');
    } else {
      this.iconClaveCorreo = 'assets/images/icon/invisible.png';
      document.getElementsByName("claveCorreoPortal")[0].setAttribute('type', 'text');
    }
  }


  verApiKeyTbk() {
    if (this.verApiKey == 1) {
      this.verApiKey = 0;
    } else {
      this.verApiKey = 1;
    }


    if (this.verApiKey == 0) {
      this.iconApiKey = 'assets/images/icon/view.png';
      document.getElementsByName("apiKeyTbk")[0].setAttribute('type', 'password');
    } else {
      this.iconApiKey = 'assets/images/icon/invisible.png';
      document.getElementsByName("apiKeyTbk")[0].setAttribute('type', 'text');
    }
  }

  get IsStepDatosClienteOk() {
    if (this.tenant.idAreaComercial == null || this.tenant.idAreaComercial == 0) {
      return false
    }

    if (this.tenant.idLineaProducto == null || this.tenant.idLineaProducto == 0) {
      return false
    }

    if (this.tenant.idPlan == null || this.tenant.idPlan == 0) {
      return false
    }

    if (this.tenant.idImplementador == null || this.tenant.idImplementador == 0) {
      return false
    }

    if (this.tenant.nombreImplementador == null || this.tenant.nombreImplementador == '') {
      return false;
    }

    if (this.tenant.telefonoImplementador == null || this.tenant.telefonoImplementador == '') {
      return false;
    }

    if (this.tenant.correoImplementador == null || this.tenant.correoImplementador == '') {
      return false;
    }

    if (this.tenant.otImplementacion == null || this.tenant.otImplementacion == '') {
      return false;
    }

    if (this.tenant.fechaInicioContrato == null) {
      return false;
    }

    if (this.tenant.fechaTerminoContrato == null) {
      return false;
    }

    if (this.tenant.fechaInicioImplementacion == null) {
      return false;
    }

    if (this.tenant.fechaTerminoImplementacion == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionEmpresa.urlPortal == null || this.tenant.datosImplementacion.configuracionEmpresa.urlPortal == '') {
      return false;
    }

    return true
  }

  get IsStepConexionesOk() {
    if (this.tenant.datosImplementacion.servidorPortal == null || this.tenant.datosImplementacion.servidorPortal == '') {
      this.conexionBaseDatosValid = false;
      return false
    }

    if (this.tenant.datosImplementacion.baseDatosPortal == null || this.tenant.datosImplementacion.baseDatosPortal == '') {
      this.conexionBaseDatosValid = false;
      return false;
    }

    if (this.tenant.datosImplementacion.usuarioBaseDatosPortal == null || this.tenant.datosImplementacion.usuarioBaseDatosPortal == '') {
      this.conexionBaseDatosValid = false;
      return false;
    }

    if (this.tenant.datosImplementacion.claveBaseDatosPortal == null || this.tenant.datosImplementacion.claveBaseDatosPortal == '') {
      this.conexionBaseDatosValid = false;
      return false;
    }


    if (this.tenant.datosImplementacion.apiSoftland.areaDatos == null || this.tenant.datosImplementacion.apiSoftland.areaDatos == '') {
      this.conexionApiValid = false;
      return false;
    }

    if (this.tenant.datosImplementacion.apiSoftland.url == null || this.tenant.datosImplementacion.apiSoftland.url == '') {
      this.conexionApiValid = false;
      return false;
    }

    if (this.tenant.datosImplementacion.apiSoftland.token == null || this.tenant.datosImplementacion.apiSoftland.token == '') {
      this.conexionApiValid = false;
      return false;
    }

    if (!this.conexionApiValid) {
      return false;
    }

    if (!this.conexionBaseDatosValid) {
      return false;
    }

    return true
  }

  get IsStepConfiguracionPortalOk() {
    if (this.selectedCuentasContables == null || this.selectedCuentasContables.length == 0) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionPagoCliente.glosaComprobante == null || this.tenant.datosImplementacion.configuracionPagoCliente.glosaComprobante == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.utilizaTransbank == 1) {
      if (this.tenant.datosImplementacion.cuentaContableTransbank == null || this.tenant.datosImplementacion.cuentaContableTransbank == '') {
        return false;
      }

      if (this.tenant.datosImplementacion.documentoContableTransbank == null || this.tenant.datosImplementacion.documentoContableTransbank == '') {
        return false;
      }

      if (this.tenant.datosImplementacion.codigoComercioTransbank == null || this.tenant.datosImplementacion.codigoComercioTransbank == '') {
        return false;
      }

      if (this.tenant.datosImplementacion.apiKeyTransbank == null || this.tenant.datosImplementacion.apiKeyTransbank == '') {
        return false;
      }
    }

    if (this.tenant.datosImplementacion.utilizaVirtualPos == 1) {
      if (this.tenant.datosImplementacion.cuentaContableVirtualPos == null || this.tenant.datosImplementacion.cuentaContableVirtualPos == '') {
        return false;
      }

      if (this.tenant.datosImplementacion.documentoContableVirtualPos == null || this.tenant.datosImplementacion.documentoContableVirtualPos == '') {
        return false;
      }
    }
    return true;
  }

  get IsStepConfiguracionCorreoOk() {

    if (this.tenant.datosImplementacion.configuracionCorreo.smtpServer == null || this.tenant.datosImplementacion.configuracionCorreo.smtpServer == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.usuario == null || this.tenant.datosImplementacion.configuracionCorreo.usuario == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.clave == null || this.tenant.datosImplementacion.configuracionCorreo.clave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.correoOrigen == null || this.tenant.datosImplementacion.configuracionCorreo.correoOrigen == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.nombreCorreos == null || this.tenant.datosImplementacion.configuracionCorreo.nombreCorreos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.puerto == null || this.tenant.datosImplementacion.configuracionCorreo.puerto == 0) {
      return false;
    }

    if (this.correo1 == null || this.correo1.length == 0) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoPagoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoPagoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.colorBoton == null || this.tenant.datosImplementacion.configuracionCorreo.colorBoton == '') {
      return false;
    }

    if (this.urlImagenLogoPortada == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoPagoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoPagoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloPagoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.tituloPagoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoPagoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.textoPagoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoAccesoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoAccesoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloAccesoCliente == null || this.tenant.datosImplementacion.configuracionCorreo.tituloAccesoCliente == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoMensajeActivacion == null || this.tenant.datosImplementacion.configuracionCorreo.textoMensajeActivacion == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoCambioDatos == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoCambioDatos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloCambioDatos == null || this.tenant.datosImplementacion.configuracionCorreo.tituloCambioDatos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoCambioDatos == null || this.tenant.datosImplementacion.configuracionCorreo.textoCambioDatos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoCambioClave == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoCambioClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloCambioClave == null || this.tenant.datosImplementacion.configuracionCorreo.tituloCambioClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoCambioClave == null || this.tenant.datosImplementacion.configuracionCorreo.textoCambioClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoRecuperarClave == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoRecuperarClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloRecuperarClave == null || this.tenant.datosImplementacion.configuracionCorreo.tituloRecuperarClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoRecuperarClave == null || this.tenant.datosImplementacion.configuracionCorreo.textoRecuperarClave == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoEnvioDocumentos == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoEnvioDocumentos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloEnvioDocumentos == null || this.tenant.datosImplementacion.configuracionCorreo.tituloEnvioDocumentos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoEnvioDocumentos == null || this.tenant.datosImplementacion.configuracionCorreo.textoEnvioDocumentos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.tituloCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.textoCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoPreCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoPreCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloPreCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.tituloPreCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoPreCobranza == null || this.tenant.datosImplementacion.configuracionCorreo.textoPreCobranza == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.asuntoEstadoCuenta == null || this.tenant.datosImplementacion.configuracionCorreo.asuntoEstadoCuenta == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.tituloEstadoCuenta == null || this.tenant.datosImplementacion.configuracionCorreo.tituloEstadoCuenta == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoEstadoCuenta == null || this.tenant.datosImplementacion.configuracionCorreo.textoEstadoCuenta == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionCorreo.textoEstadoCuenta == null || this.tenant.datosImplementacion.configuracionCorreo.textoEstadoCuenta == '') {
      return false;
    }
    return true;
  }


  get IsStepConfiguracionDisenoOk() {

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.textoDescargaCobranza == null || this.tenant.datosImplementacion.configuracionDiseno.textoDescargaCobranza == '') {
      return false;
    }

    if (this.urlImagenLogoPortadaDiseno == null) {
      return false;
    }

    if (this.urlImagenPortada == null) {
      return false;
    }

    if (this.urlImagenLogoPrincipalSidebar == null) {
      return false;
    }

    if (this.urlImagenLogoSecundarioSidebar == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.textoCobranzaExpirada == null || this.tenant.datosImplementacion.configuracionDiseno.textoCobranzaExpirada == '') {
      return false;
    }

    if (this.urlImagenBannerPagoRapido == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.textoNoConsideraTodaDeuda == null || this.tenant.datosImplementacion.configuracionDiseno.textoNoConsideraTodaDeuda == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes == null || this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos == null || this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer == null || this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos == null || this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras == '') {
      return false;
    }

    if (this.urlImagenUltimasCompras == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras == null || this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar == '') {
      return false;
    }

    if (this.urlImagenMisCompras == null) {
      return false;
    }

    if (this.urlImagenBannerMisCompras == null) {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil == null || this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil == '') {
      return false;
    }

    if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil == null || this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil == '') {
      return false;
    }

    if (this.urlImagenUsuario == null) {
      return false;
    }

    if (this.urlImagenBannerPerfil == null) {
      return false;
    }

    if (this.urlImagenIconoContactos == null) {
      return false;
    }

    if (this.urlImagenIconoClavePerfil == null) {
      return false;
    }

    if (this.urlImagenIconoClavePerfil == null) {
      return false;
    }

    if (this.urlImagenIconoEditarPerfil == null) {
      return false;
    }

    return true;
  }

  onStepConexionesNext() {
    this.spinner.show();
    this.implementacionService.obtieneCuentasContables(this.tenant.datosImplementacion.apiSoftland).subscribe((res: any[]) => {
      this.cuentasContables = res;
      this.cuentasContables.forEach(element => {
        element.nombre = element.codigo + ' - ' + element.nombre;
      });
      if (this.cuentasContables.length == 0) {
        this.cuentasContables.push({ nombre: 'Sin Datos', codigo: '', disabled: true })
      }

      let ccs = []
      const cuentasContables = this.tenant.datosImplementacion.configuracionPagoCliente.cuentasContablesDeuda ? this.tenant.datosImplementacion.configuracionPagoCliente.cuentasContablesDeuda.split(';') : [];

      cuentasContables.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          ccs.push(element)
        }
      });

      this.selectedCuentasContables = ccs.length > 0 ? ccs : null;

      this.implementacionService.obtieneTiposDocumentos(this.tenant.datosImplementacion.apiSoftland).subscribe((res2: any[]) => {
        this.tiposDocumentos = res2;
        this.tiposDocumentos.forEach(element => {
          element.desDoc = element.codDoc + ' - ' + element.desDoc;
        });
        if (this.tiposDocumentos.length == 0) {
          this.tiposDocumentos.push({ desDoc: 'Sin Datos', codDoc: '', disabled: true })
        }

        let ccs = []
        const tiposDocumentos = this.tenant.datosImplementacion.configuracionPagoCliente.tiposDocumentosDeuda ? this.tenant.datosImplementacion.configuracionPagoCliente.tiposDocumentosDeuda.split(';') : [];

        tiposDocumentos.forEach(element => {
          if (element && element.trim().length > 0 && element.trim() !== ';') {
            ccs.push(element)
          }
        });

        this.selectedTiposDocumentos = ccs.length > 0 ? ccs : null;
        this.implementacionService.obtieneCuentasContablesPasarela(this.tenant.datosImplementacion.apiSoftland).subscribe((res3: any[]) => {
          this.cuentasContablesPago = res3;
          this.cuentasContablesPago.forEach(element => {
            element.nombre = element.codigo + ' - ' + element.nombre;
          });
          if (this.cuentasContablesPago.length == 0) {
            this.cuentasContablesPago.push({ desDoc: 'Sin Datos', codDoc: '', disabled: true })
          }
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cuentas contables para pago', '', true); });
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener tipos de documentos', '', true); });
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cuentas contables', '', true); });
  }


  validaEmailConfiguracionCorreo() {
    if (this.tenant.datosImplementacion.configuracionCorreo.correoOrigen != '' && this.tenant.datosImplementacion.configuracionCorreo.correoOrigen != null) {
      if (!this.utils.validateMail(this.tenant.datosImplementacion.configuracionCorreo.correoOrigen)) {
        this.notificationService.warning('Debe ingresar un correo válido.', '', true);
        this.tenant.datosImplementacion.configuracionCorreo.correoOrigen = '';
        return;
      }
    }

  }

  MostrarSeccion(tipo: number) {
    switch (tipo) {
      case 1:
        this.general == 0 ? this.general = 1 : this.general = 0;
        break;

      case 2:
        this.pagos == 0 ? this.pagos = 1 : this.pagos = 0;
        break;

      case 3:
        this.activacion == 0 ? this.activacion = 1 : this.activacion = 0;
        break;

      case 4:
        this.cambioDatos == 0 ? this.cambioDatos = 1 : this.cambioDatos = 0;
        break;

      case 5:
        this.actualizaClave == 0 ? this.actualizaClave = 1 : this.actualizaClave = 0;
        break;

      case 6:
        this.actualizaCorreo == 0 ? this.actualizaCorreo = 1 : this.actualizaCorreo = 0;
        break;

      case 7:
        this.recuperaClave == 0 ? this.recuperaClave = 1 : this.recuperaClave = 0;
        break;

      case 8:
        this.envioDocumentos == 0 ? this.envioDocumentos = 1 : this.envioDocumentos = 0;
        break;

      case 9:
        this.notificacionPago == 0 ? this.notificacionPago = 1 : this.notificacionPago = 0;
        break;

      case 10:
        this.notificacionPagoSinComprobante == 0 ? this.notificacionPagoSinComprobante = 1 : this.notificacionPagoSinComprobante = 0;
        break;

      case 11:
        this.cobranza == 0 ? this.cobranza = 1 : this.cobranza = 0;
        break;

      case 12:
        this.preCobranza == 0 ? this.preCobranza = 1 : this.preCobranza = 0;
        break;

      case 13:
        this.estadoCuenta == 0 ? this.estadoCuenta = 1 : this.estadoCuenta = 0;
        break;
    }
  }

  verificaColor() {
    let pattern = /^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$/

    if (this.tenant.datosImplementacion.configuracionCorreo.colorBoton != '' && this.tenant.datosImplementacion.configuracionCorreo.colorBoton != null) {  //FCA 08-06-2022
      if (!pattern.test(this.config.colorBoton)) {
        this.tenant.datosImplementacion.configuracionCorreo.colorBoton = '';
        this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
      }
    }
  }

  preview(event: any) {
    let files = event.target.files;
    if (files) {
      for (let file of files) {
        let reader = new FileReader();
        reader.onload = (e: any) => {
          let img: IPreviewImgs = {
            url: e.target.result
          }
          this.urlImagenLogoPortada = img;
        }
        reader.readAsDataURL(file);
      }
      this.defaultImageLogoPortada = event.target.files
    }

  }

  onChange(event: any) {
    this.defaultImageLogoPortada = event.srcElement.files;
  }

  deletePreviewImg() {
    this.urlImagenLogoPortada = null;
    this.defaultImageLogoPortada = null;
    this.clearInput(document.getElementById('fileImageLogoPortada'));
  }

  clearInput(input) {
    try {
      input.value = null;
    } catch (ex) { }
    if (input.value) {
      input.parentNode.replaceChild(input.cloneNode(true), input);
    }
  }


  MostrarSeccionConfigDiseno(seccion: number) {

    switch (seccion) {
      case 1:
        if (this.inicioDeSesion == 0) {
          this.inicioDeSesion = 1;
        } else {
          this.inicioDeSesion = 0;
        }
        break;

      case 2:
        if (this.sidebar == 0) {
          this.sidebar = 1;
        } else {
          this.sidebar = 0;
        }
        break;

      case 3:
        if (this.pagoRapido == 0) {
          this.pagoRapido = 1;
        } else {
          this.pagoRapido = 0;
        }
        break;

      case 4:
        if (this.dashboardCliente == 0) {
          this.dashboardCliente = 1;
        } else {
          this.dashboardCliente = 0;
        }
        break;

      case 5:
        if (this.misCompras == 0) {
          this.misCompras = 1;
        } else {
          this.misCompras = 0;
        }
        break;

      case 6:
        if (this.miPerfil == 0) {
          this.miPerfil = 1;
        } else {
          this.miPerfil = 0;
        }
        break;
    }
  }

  verificaColorConfigDiseno(color: number) {
    let pattern = /^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$/

    switch (color) {
      case 1:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoPortada = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;
      case 2:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagoRapido = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 3:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonInicioSesion = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 4:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonPagar = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 5:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoDocumentos = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 6:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes != '' && this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorTextoPendientes = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;
      case 7:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoVencidos = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;
      case 8:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos != '' && this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorTextoVencidos = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 9:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoPorVencer = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 10:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer != '' && this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorTextoPorVencer = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }

      case 11:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos != '' && this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorSeleccionDocumentos = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 12:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoUltimasCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 13:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonUltimasCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 14:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonUltimasCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 15:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorTextoBotonUltimasCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 16:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoMisCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 17:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoPendientesMisCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 18:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoProductosMisCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 19:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorFondoGuiasMisCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 20:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras != '' && this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorIconosMisCompras = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 21:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonBuscar = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 22:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonModificarPerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 23:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonClavePerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 24:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonEstadoPerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonEstadoPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonEstadoPerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonEstadoPerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 25:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorHoverBotonesPerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 26:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonCancelarModalPerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;

      case 27:
        if (this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil != '' && this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil)) {
            this.tenant.datosImplementacion.configuracionDiseno.colorBotonGuardarModalPerfil = '';
            this.notificationService.warning('Debe ingresar un color válido en formato hexadecimal.', '', true);
          }
        }
        break;
    }



  }

  onChangeConfigDiseno(event: any, imagen: number) {
    switch (imagen) {
      case 1:
        this.defaultImageLogoPortadaDiseno = event.srcElement.files;
        break;
      case 2:
        this.defaultImagenPortada = event.srcElement.files;
        break;
      case 3:
        this.defaultImageLogoPrincipalSidebar = event.srcElement.files;
        break;
      case 4:
        this.defaultImageLogoSecundarioSidebar = event.srcElement.files;
        break;
      case 5:
        this.defaultImageBannerPagoRapido = event.srcElement.files;
        break;
      case 6:
        this.defaultImageUltimasCompras = event.srcElement.files;
        break;
      case 7:
        this.defaultImageMisCompras = event.srcElement.files;
        break;
      case 8:
        this.defaultImageBannerMisCompras = event.srcElement.files;
        break;
      case 9:
        this.defaultImagenUsuario = event.srcElement.files;
        break;
      case 10:
        this.defaultImageBannerPerfil = event.srcElement.files;
        break;
      case 11:
        this.urlImagenIconoContactos = event.srcElement.files;
        break;
      case 12:
        this.defaultImageIconoClavePerfil = event.srcElement.files;
        break;
      case 13:
        this.defaultImageIconoEditarPerfil = event.srcElement.files;
        break;
      case 14:
        this.defaultImageIconoEstadoPerfil = event.srcElement.files;
        break;
      case 15:
        this.defaultImageFavicon = event.srcElement.files;
        break;
    }

  }

  previewConfigDiseno(event: any, imagen: number) {
    let files = event.target.files;
    switch (imagen) {
      case 1:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenLogoPortadaDiseno = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageLogoPortadaDiseno = event.target.files
        }
        break;

      case 2:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenPortada = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImagenPortada = event.target.files
        }
        break;

      case 3:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenLogoPrincipalSidebar = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageLogoPrincipalSidebar = event.target.files
        }
        break;

      case 4:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenLogoSecundarioSidebar = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageLogoSecundarioSidebar = event.target.files
        }
        break;

      case 5:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenBannerPagoRapido = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageBannerPagoRapido = event.target.files
        }
        break;

      case 6:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenUltimasCompras = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageUltimasCompras = event.target.files
        }
        break;

      case 7:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenMisCompras = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageMisCompras = event.target.files
        }
        break;

      case 8:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenBannerMisCompras = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageBannerMisCompras = event.target.files
        }
        break;

      case 9:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenUsuario = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImagenUsuario = event.target.files
        }
        break;

      case 10:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenBannerPerfil = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageBannerPerfil = event.target.files
        }
        break;

      case 11:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenIconoContactos = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageIconoContactos = event.target.files
        }
        break;

      case 12:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenIconoClavePerfil = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageIconoClavePerfil = event.target.files
        }
        break;

      case 13:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenIconoEditarPerfil = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageIconoEditarPerfil = event.target.files
        }
        break;

      case 14:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }
              this.urlImagenIconoEstadoPerfil = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageIconoEstadoPerfil = event.target.files
        }
        break;

      case 15:
        if (files) {
          for (let file of files) {
            let reader = new FileReader();
            reader.onload = (e: any) => {
              let img: IPreviewImgs = {
                url: e.target.result
              }

              this.urlImagenFavicon = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageFavicon = event.target.files
        }
        break;
    }
  }

  deletePreviewImgConfigDiseno(imagen: number) {
    switch (imagen) {
      case 1:
        this.urlImagenLogoPortadaDiseno = null;
        this.defaultImageLogoPortadaDiseno = null;
        this.clearInput(document.getElementById('fileImageLogoPortada2'));
        break;
      case 2:
        this.urlImagenPortada = null;
        this.defaultImagenPortada = null;
        this.clearInput(document.getElementById('fileImagePortada'));
        break;
      case 3:
        this.urlImagenLogoPrincipalSidebar = null;
        this.defaultImageLogoPrincipalSidebar = null;
        this.clearInput(document.getElementById('fileImageLogoPrincipalSidebar'));
        break;
      case 4:
        this.urlImagenLogoSecundarioSidebar = null;
        this.defaultImageLogoSecundarioSidebar = null;
        this.clearInput(document.getElementById('fileImageLogoSecundarioSidebar'));
        break;
      case 5:
        this.urlImagenBannerPagoRapido = null;
        this.defaultImageBannerPagoRapido = null;
        this.clearInput(document.getElementById('fileImageBannerPagoRapido'));
        break;
      case 6:
        this.urlImagenUltimasCompras = null;
        this.defaultImageUltimasCompras = null;
        this.clearInput(document.getElementById('fileImageUltimasCompras'));
        break;
      case 7:
        this.urlImagenMisCompras = null;
        this.defaultImageMisCompras = null;
        this.clearInput(document.getElementById('fileImageIconoMisCompras'));
        break;
      case 8:
        this.urlImagenBannerMisCompras = null;
        this.defaultImageBannerMisCompras = null;
        this.clearInput(document.getElementById('fileImageBannerMisCompras'));
        break;
      case 9:
        this.urlImagenUsuario = null;
        this.defaultImagenUsuario = null;
        this.clearInput(document.getElementById('fileImageUsuario'));
        break;
      case 10:
        this.urlImagenBannerPerfil = null;
        this.defaultImageBannerPerfil = null;
        this.clearInput(document.getElementById('fileImageBannerMiPerfil'));
        break;
      case 11:
        this.urlImagenIconoContactos = null;
        this.defaultImageIconoContactos = null;
        this.clearInput(document.getElementById('fileImageIconoContactos'));
        break;
      case 12:
        this.urlImagenIconoClavePerfil = null;
        this.defaultImageIconoClavePerfil = null;
        this.clearInput(document.getElementById('fileImageIconoClavePerfil'));
        break;
      case 13:
        this.urlImagenIconoEditarPerfil = null;
        this.defaultImageIconoEditarPerfil = null;
        this.clearInput(document.getElementById('fileImageIconoEditarPerfil'));
        break;
      case 14:
        this.urlImagenIconoEstadoPerfil = null;
        this.defaultImageIconoEstadoPerfil = null;
        this.clearInput(document.getElementById('fileImageIconoEstadoPerfil'));
        break;
      case 15:
        this.urlImagenFavicon = null;
        this.defaultImageFavicon = null;
        this.clearInput(document.getElementById('fileImageFavicon'));
        break;
    }
  }

  llenaDatosTenant(content: any) {
    this.spinner.show();
    this.implementacionService.obtieneDatosTenant(this.tenant).subscribe((res: any) => {
      this.tenant = res;

      const c1: string[] = this.tenant.datosImplementacion.configuracionCorreo.correoAvisoPago != null ? this.tenant.datosImplementacion.configuracionCorreo.correoAvisoPago.split(';') : [];

      c1.forEach(element => {
        if (element && element.length > 2) {
          this.correo1.push({ value: element, display: element });
        }
      });


      if (this.tenant.datosImplementacion.configuracionDiseno.logoPortada != null && this.tenant.datosImplementacion.configuracionDiseno.logoPortada != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.logoPortada
        }
        this.urlImagenLogoPortadaDiseno = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.imagenPortada != null && this.tenant.datosImplementacion.configuracionDiseno.imagenPortada != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.imagenPortada
        }
        this.urlImagenPortada = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.logoSidebar != null && this.tenant.datosImplementacion.configuracionDiseno.logoSidebar != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.logoSidebar
        }
        this.urlImagenLogoPrincipalSidebar = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.logoMinimalistaSidebar != null && this.tenant.datosImplementacion.configuracionDiseno.logoMinimalistaSidebar != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.logoMinimalistaSidebar
        }
        this.urlImagenLogoSecundarioSidebar = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.bannerPagoRapido != null && this.tenant.datosImplementacion.configuracionDiseno.bannerPagoRapido != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.bannerPagoRapido
        }
        this.urlImagenBannerPagoRapido = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.imagenUltimasCompras != null && this.tenant.datosImplementacion.configuracionDiseno.imagenUltimasCompras != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.imagenUltimasCompras
        }
        this.urlImagenUltimasCompras = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.iconoMisCompras != null && this.tenant.datosImplementacion.configuracionDiseno.iconoMisCompras != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.iconoMisCompras
        }
        this.urlImagenMisCompras = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.bannerMisCompras != null && this.tenant.datosImplementacion.configuracionDiseno.bannerMisCompras != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.bannerMisCompras
        }
        this.urlImagenBannerMisCompras = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.imagenUsuario != null && this.tenant.datosImplementacion.configuracionDiseno.imagenUsuario != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.imagenUsuario
        }
        this.urlImagenUsuario = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.bannerPortal != null && this.tenant.datosImplementacion.configuracionDiseno.bannerPortal != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.bannerPortal
        }
        this.urlImagenBannerPerfil = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.iconoContactos != null && this.tenant.datosImplementacion.configuracionDiseno.iconoContactos != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.iconoContactos
        }
        this.urlImagenIconoContactos = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.iconoClavePerfil != null && this.tenant.datosImplementacion.configuracionDiseno.iconoClavePerfil != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.iconoClavePerfil
        }
        this.urlImagenIconoClavePerfil = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.iconoEditarPerfil != null && this.tenant.datosImplementacion.configuracionDiseno.iconoEditarPerfil != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.iconoEditarPerfil
        }
        this.urlImagenIconoEditarPerfil = preview;
      }

      if (this.tenant.datosImplementacion.configuracionDiseno.iconoEstadoPerfil != null && this.tenant.datosImplementacion.configuracionDiseno.iconoEstadoPerfil != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionDiseno.iconoEstadoPerfil
        }
        this.urlImagenIconoEstadoPerfil = preview;
      }

      if (this.tenant.datosImplementacion.configuracionCorreo.logoCorreo != null && this.tenant.datosImplementacion.configuracionCorreo.logoCorreo != '') {
        let preview: IPreviewImgs = {
          url: this.tenant.datosImplementacion.configuracionCorreo.logoCorreo
        }
        this.urlImagenLogoPortada = preview;
      }


      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', windowClass: 'modal-xl' });
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener datos', '', true); });
  }

  cambiaAmbienteTbk() {
    if (this.tenant.datosImplementacion.ambienteTransbank == 0) {
      this.tenant.datosImplementacion.codigoComercioTransbank = '597055555532'
      this.tenant.datosImplementacion.apiKeyTransbank = '579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C'
    } else {
      this.tenant.datosImplementacion.codigoComercioTransbank = ''
      this.tenant.datosImplementacion.apiKeyTransbank = ''
    }
  }

  onComplete() {
    this.spinner.show();

    if (this.defaultImageLogoPortadaDiseno && this.defaultImageLogoPortadaDiseno.length > 0) {
      this.tenant.logoPortada = this.defaultImageLogoPortadaDiseno[0]
    }

    if (this.defaultImagenPortada && this.defaultImagenPortada.length > 0) {
      this.tenant.imagenPortada = this.defaultImagenPortada[0]
    }

    if (this.defaultImageLogoPrincipalSidebar && this.defaultImageLogoPrincipalSidebar.length > 0) {
      this.tenant.logoSidebar = this.defaultImageLogoPrincipalSidebar[0]
    }

    if (this.defaultImageLogoSecundarioSidebar && this.defaultImageLogoSecundarioSidebar.length > 0) {
      this.tenant.logoMinimalistaSidebar = this.defaultImageLogoSecundarioSidebar[0]
    }

    if (this.defaultImageBannerPagoRapido && this.defaultImageBannerPagoRapido.length > 0) {
      this.tenant.bannerPagoRapido = this.defaultImageBannerPagoRapido[0]
    }

    if (this.defaultImageUltimasCompras && this.defaultImageUltimasCompras.length > 0) {
      this.tenant.imagenUltimasCompras = this.defaultImageUltimasCompras[0]
    }

    if (this.defaultImageMisCompras && this.defaultImageMisCompras.length > 0) {
      this.tenant.iconoMisCompras = this.defaultImageMisCompras[0]
    }

    if (this.defaultImageBannerMisCompras && this.defaultImageBannerMisCompras.length > 0) {
      this.tenant.bannerMisCompras = this.defaultImageBannerMisCompras[0]
    }

    if (this.defaultImagenUsuario && this.defaultImagenUsuario.length > 0) {
      this.tenant.imagenUsuario = this.defaultImagenUsuario[0]
    }

    if (this.defaultImageBannerPerfil && this.defaultImageBannerPerfil.length > 0) {
      this.tenant.bannerPortal = this.defaultImageBannerPerfil[0]
    }

    if (this.defaultImageIconoContactos && this.defaultImageIconoContactos.length > 0) {
      this.tenant.iconoContactos = this.defaultImageIconoContactos[0]
    }

    if (this.defaultImageIconoClavePerfil && this.defaultImageIconoClavePerfil.length > 0) {
      this.tenant.iconoClavePerfil = this.defaultImageIconoClavePerfil[0]
    }

    if (this.defaultImageIconoEditarPerfil && this.defaultImageIconoEditarPerfil.length > 0) {
      this.tenant.iconoEditarPerfil = this.defaultImageIconoEditarPerfil[0]
    }

    if (this.defaultImageIconoEstadoPerfil && this.defaultImageIconoEstadoPerfil.length > 0) {
      this.tenant.iconoEstadoPerfil = this.defaultImageIconoEstadoPerfil[0]
    }

    if (this.defaultImageLogoPortada && this.defaultImageLogoPortada.length > 0) {
      this.tenant.logoCorreo = this.defaultImageLogoPortada[0]
    }


    let c1: string = '';

    this.correo1.forEach(element => {
      c1 += `${element.value};`;
    });

    this.config.correoAvisoPago = c1.length > 0 ? c1.substring(0, c1.length - 1) : '';
    this.tenant.estado = 3
    this.tenant.datosImplementacion.utilizaTransbank =  this.tenant.datosImplementacion.utilizaTransbank ? 1 : 0
    this.tenant.datosImplementacion.utilizaVirtualPos =  this.tenant.datosImplementacion.utilizaVirtualPos ? 1 : 0
    this.tenant.datosImplementacion.configuracionCorreo.ssl = this.tenant.datosImplementacion.configuracionCorreo.ssl ? 1 : 0
    this.tenant.datosImplementacion.configuracionPortal.muestraContactosPerfil = this.tenant.datosImplementacion.configuracionPortal.muestraContactosPerfil ? 1 : 0
    this.tenant.datosImplementacion.configuracionPortal.permiteExportarExcel = this.tenant.datosImplementacion.configuracionPortal.permiteExportarExcel ? 1 : 0
    this.tenant.datosImplementacion.configuracionPortal.habilitaPagoRapido = this.tenant.datosImplementacion.configuracionPortal.habilitaPagoRapido ? 1 : 0
    this.tenant.datosImplementacion.configuracionPortal.utilizaDocumentoPagoRapido = this.tenant.datosImplementacion.configuracionPortal.utilizaDocumentoPagoRapido ? 1 : 0
    this.tenant.datosImplementacion.configuracionPortal.permiteAbonoParcial = this.tenant.datosImplementacion.configuracionPortal.permiteAbonoParcial ? 1 : 0

    let cuentasContables = null;
    if (this.selectedCuentasContables != null) {
      cuentasContables = this.selectedCuentasContables.length > 0 ? this.selectedCuentasContables.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null;
    }

    let tiposDocumentos = null;
    if (this.selectedTiposDocumentos != null) {
      tiposDocumentos = this.selectedTiposDocumentos.length > 0 ? this.selectedTiposDocumentos.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null;
    }

    this.tenant.datosImplementacion.configuracionPagoCliente.cuentasContablesDeuda = cuentasContables
    this.tenant.datosImplementacion.configuracionPagoCliente.tiposDocumentosDeuda = tiposDocumentos

    this.implementacionService.subirTenantYArchivos(this.tenant).then((res: any) => {
      this.getEmpresas();
      this.modalService.dismissAll();
      this.notificationService.success('Datos guardados correctamente', '', true);
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un al guardar datos', '', true); });
  }

  public onAdd(tag: AutoCompleteModel, type: number) {
    switch (type) {
      case 1:
        this.removeFromArrayIfMailIsInvalid(tag.value, this.correo1);
        this.toLowerMails(this.correo1);
        break;
    }

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
