import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionPagoClientesService } from '../../../shared/services/configuracionpagoclientes.service';
import { ConfiguracionPagoCliente, ConfiguracionPortal } from '../../../shared/models/configuracioncobranza.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { number } from 'ngx-custom-validators/src/app/number/validator';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';
import { LocalStoreService } from 'src/app/shared/services/local-store.service';

interface AutoCompleteModel {
  value: any;
  display: string;
}

export interface IPreviewImgs {
  url?: string;
}

@Component({
  selector: 'app-config',
  templateUrl: './config.component.html',
  styleUrls: ['./config.component.scss']
})
export class ConfigComponent implements OnInit {

  configAll: any = null;
  configPagoClientes: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
  configPortal: ConfiguracionPortal = new ConfiguracionPortal();
  tipoDocs: any = [];
  cuentasContables: any = [];
  cuentasContablesCP: any = [];
  selectedCuen: any = [];
  tiposDctosSelected: any = [];
  anioContable: any;
  monedas: any = [];
  selectedMoneda: string = '';
  selectedSegundaMoneda: string = '';
  centrosCostos: any = [];
  areasNegocio: any = [];
  public modalTitle: string = '';
  public modalContent: string = '';
  public modalImage: string = '';
  cantidadDiasPorVencer: any = 0;
  configuracionDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();
  inicioDeSesion: number = 0;
  sidebar: number = 0;
  pagoRapido: number = 0;
  dashboardCliente: number = 0;
  misCompras: number = 0;
  miPerfil: number = 0;

  anioTributario: any = null;

  public defaultImageLogoPortada: FileList = null;
  public urlImagenLogoPortada: IPreviewImgs = null;
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

  constructor(private configuracionPagoClientesService: ConfiguracionPagoClientesService, private _sanitizer: DomSanitizer,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService, private configuracionDisenoService: ConfiguracionDisenoService,
    private configuracionSoftlandService: ConfiguracionSoftlandService, private modalService: NgbModal,  private ls: LocalStoreService,  private configuracionService: ConfiguracionPagoClientesService) { }

  ngOnInit(): void {
    this.getConfigPagoClientes();
    this.getConfigPortal();
    //this.getCentrosCostos();
    // this.getAreasNegocio();
    this.getConfigDiseno();
  }

  getConfigDiseno() {
    this.configuracionDisenoService.getConfigDiseno().subscribe((res: any) => {
      this.configuracionDiseno = res;

      if (this.configuracionDiseno.logoPortada != null && this.configuracionDiseno.logoPortada != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.logoPortada
        }
        this.urlImagenLogoPortada = preview;
      }

      if (this.configuracionDiseno.imagenPortada != null && this.configuracionDiseno.imagenPortada != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.imagenPortada
        }
        this.urlImagenPortada = preview;
      }

      if (this.configuracionDiseno.logoSidebar != null && this.configuracionDiseno.logoSidebar != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.logoSidebar
        }
        this.urlImagenLogoPrincipalSidebar = preview;
      }

      if (this.configuracionDiseno.logoMinimalistaSidebar != null && this.configuracionDiseno.logoMinimalistaSidebar != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.logoMinimalistaSidebar
        }
        this.urlImagenLogoSecundarioSidebar = preview;
      }

      if (this.configuracionDiseno.bannerPagoRapido != null && this.configuracionDiseno.bannerPagoRapido != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.bannerPagoRapido
        }
        this.urlImagenBannerPagoRapido = preview;
      }

      if (this.configuracionDiseno.imagenUltimasCompras != null && this.configuracionDiseno.imagenUltimasCompras != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.imagenUltimasCompras
        }
        this.urlImagenUltimasCompras = preview;
      }

      if (this.configuracionDiseno.iconoMisCompras != null && this.configuracionDiseno.iconoMisCompras != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.iconoMisCompras
        }
        this.urlImagenMisCompras = preview;
      }

      if (this.configuracionDiseno.bannerMisCompras != null && this.configuracionDiseno.bannerMisCompras != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.bannerMisCompras
        }
        this.urlImagenBannerMisCompras = preview;
      }

      if (this.configuracionDiseno.imagenUsuario != null && this.configuracionDiseno.imagenUsuario != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.imagenUsuario
        }
        this.urlImagenUsuario = preview;
      }

      if (this.configuracionDiseno.bannerPortal != null && this.configuracionDiseno.bannerPortal != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.bannerPortal
        }
        this.urlImagenBannerPerfil = preview;
      }

      if (this.configuracionDiseno.iconoContactos != null && this.configuracionDiseno.iconoContactos != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.iconoContactos
        }
        this.urlImagenIconoContactos = preview;
      }

      if (this.configuracionDiseno.iconoClavePerfil != null && this.configuracionDiseno.iconoClavePerfil != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.iconoClavePerfil
        }
        this.urlImagenIconoClavePerfil = preview;
      }

      if (this.configuracionDiseno.iconoEditarPerfil != null && this.configuracionDiseno.iconoEditarPerfil != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.iconoEditarPerfil
        }
        this.urlImagenIconoEditarPerfil = preview;
      }

      if (this.configuracionDiseno.iconoEstadoPerfil != null && this.configuracionDiseno.iconoEstadoPerfil != '') {
        let preview: IPreviewImgs = {
          url: this.configuracionDiseno.iconoEstadoPerfil
        }
        this.urlImagenIconoEstadoPerfil = preview;
      }
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrio un error al obtener la configuración.', '', true);
    });
  }

  getConfigPagoClientes() {
    this.spinner.show();

    this.configuracionPagoClientesService.getConfigPagoClientes().subscribe((res: any) => {
      if (res) {
        // this.configAll = res;

        this.getMonedas();
        this.configPagoClientes = res;
        this.cantidadDiasPorVencer = this.configPagoClientes.diasPorVencer == null ? 0 : this.configPagoClientes.diasPorVencer;
        this.anioTributario = this.configPagoClientes.anioTributario;

      } else {
        this.configPagoClientes = new ConfiguracionPagoCliente();
      }
      this.getTipoDocs();

    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrio un error al obtener la configuración.', '', true);
    });
  }

  getMonedas() {
    // this.configuracionSoftlandService.getMonedas().subscribe((res: any) => {
    //   this.monedas = res;
    // }, err => {
    //   this.notificationService.error('Ocurrio un error al obtener codigo de monedas.', '', true);
    // });
  }

  getCentrosCostos() {
    this.configuracionSoftlandService.getCentrosCostosActivos().subscribe((res: any) => {
      this.centrosCostos = res;
    }, err => {
      this.notificationService.error('Ocurrio un error al obtener centros de costos.', '', true);
    });
  }

  getAreasNegocio() {
    this.configuracionSoftlandService.getAreasNegocio().subscribe((res: any) => {
      this.areasNegocio = res;
    }, err => {
      this.notificationService.error('Ocurrio un error al obtener areas de negocio.', '', true);
    });
  }


  getConfigPortal() {
    this.spinner.show();
    this.configuracionPagoClientesService.getConfigPortal().subscribe((res: any) => {
      if (res) {
        this.configPortal = res;
      } else {
        this.configPortal = new ConfiguracionPortal();
      }
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrio un error al obtener la configuración.', '', true);
    });
  }

  getTipoDocs() {
    this.configuracionSoftlandService.getAllTipoDocSoftland().subscribe((res: []) => {
      this.tipoDocs = res;
      this.tipoDocs.forEach(element => {
        element.desDoc = element.codDoc + ' - ' + element.desDoc;
      });
      let ccs = []
      const documentos = this.configPagoClientes.tiposDocumentosDeuda ? this.configPagoClientes.tiposDocumentosDeuda.split(';') : [];

      documentos.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          ccs.push(element)
        }
      });

      this.tiposDctosSelected = ccs.length > 0 ? ccs : null;

      this.getCuentasContables();
    }, err => { this.spinner.hide(); });
  }

  getCuentasContables() {
    this.configuracionSoftlandService.getAllCuentasContablesSoftland().subscribe(res => {

      this.cuentasContables = res;
      this.cuentasContables.forEach(element => {
        element.nombre = element.codigo + ' - ' + element.nombre;
      });
      let ccs = []
      const cuentasContables = this.configPagoClientes.cuentasContablesDeuda ? this.configPagoClientes.cuentasContablesDeuda.split(';') : [];

      cuentasContables.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          ccs.push(element)
        }
      });

      this.selectedCuen = ccs.length > 0 ? ccs : null;
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
  }

  savePagos() {


    let cuentasContables = null;
    if (this.selectedCuen != null) {
      cuentasContables = this.selectedCuen.length > 0 ? this.selectedCuen.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null;
    }

    let tiposDocumentos = null;
    if (this.tiposDctosSelected != null) {
      tiposDocumentos = this.tiposDctosSelected.length > 0 ? this.tiposDctosSelected.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null;
    }


    if (this.anioTributario == null || this.anioTributario == '') {
      this.notificationService.warning('El año tributario es obligatorio, debe ingresarlo.', '', true);
      return;
    }

    // if (this.configPagoClientes.monedaUtilizada == null || this.configPagoClientes.monedaUtilizada == '') {
    //   this.notificationService.warning('Debe ingresar Moneda Nacional', '', true);
    //   return;
    // }

    if (cuentasContables == '' || cuentasContables == null) {
      this.notificationService.warning('Debe ingresar almenos una cuenta contable', '', true);
      return;
    }

    // if(this.configPagoClientes.centroCosto == null || this.configPagoClientes.centroCosto == ''){
    //   this.notificationService.warning('Debe ingresar Centro de Costo', '', true);
    //   return;
    // }

    this.spinner.show();

    this.configPagoClientes.cuentasContablesDeuda = cuentasContables;
    this.configPagoClientes.tiposDocumentosDeuda = tiposDocumentos;
    this.configPagoClientes.anioTributario = this.anioTributario;

    this.configuracionPagoClientesService.edit(this.configPagoClientes).subscribe(res => {

      const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
      if (configuracionCompletaPortal != null) {
        configuracionCompletaPortal.configuracionPagoCliente = this.configPagoClientes;
        this.ls.setItem("configuracionCompletaPortal", configuracionCompletaPortal);
      }

      this.notificationService.success('Configuración de pagos actualizada correctamente', '', true);
      this.spinner.hide();
    }, err => {
      this.notificationService.error('Error al actualizar  Configuración de pagos', '', true);
      this.spinner.hide();
    });
  }

  saveParametros() {
    this.spinner.show();
    // this.configPortal.muestraEstadoBloqueo = this.configPortal.muestraEstadoBloqueo ? 1 : 0;
    // this.configPortal.muestraEstadoSobregiro = this.configPortal.muestraEstadoSobregiro ? 1 : 0;
    //this.configPortal.muestraContactoComercial = this.configPortal.muestraContactoComercial ? 1 : 0;
    this.configPortal.muestraContactosPerfil = this.configPortal.muestraContactosPerfil ? 1 : 0;
    this.configPortal.habilitaPagoRapido = this.configPortal.habilitaPagoRapido ? 1 : 0;
    this.configPortal.permiteExportarExcel = this.configPortal.permiteExportarExcel ? 1 : 0;
    this.configPortal.permiteAbonoParcial = this.configPortal.permiteAbonoParcial ? 1 : 0;
    this.configPortal.utilizaDocumentoPagoRapido = this.configPortal.utilizaDocumentoPagoRapido ? 1 : 0;
    this.cantidadDiasPorVencer = (this.cantidadDiasPorVencer == null || this.cantidadDiasPorVencer == '') ? 0 : this.cantidadDiasPorVencer;
    this.configuracionPagoClientesService.editPortal(this.configPortal, this.cantidadDiasPorVencer).subscribe(res => {
      this.configPagoClientes.diasPorVencer = this.cantidadDiasPorVencer;
      const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
      if (configuracionCompletaPortal != null) {
        configuracionCompletaPortal.configuracionPortal = this.configPortal;
        configuracionCompletaPortal.configuracionPagoCliente.diasPorVencer = this.cantidadDiasPorVencer;
        this.ls.setItem("configuracionCompletaPortal", configuracionCompletaPortal);
      }
      this.notificationService.success('Configuración actualizada correctamente', '', true);
      this.spinner.hide();
    }, err => {
      this.notificationService.error('Error al actualizar parametros', '', true);
      this.spinner.hide();
    });
  }

  openInfoModal(content: any, type: number) {
    switch (type) {
      case 1:
        this.modalTitle = 'Mostrar Estado Cliente';
        this.modalContent = 'Al habilitar esta opción se mostrará el estado "Desbloqueado" o "Bloqueado" en el dashboard del cliente.';
        this.modalImage = 'assets/images/config/muestra_estado_cliente.jpg';
        break;
      case 2:
        this.modalTitle = 'Mostrar Contactos en Perfil';
        this.modalContent = 'Al habilitar esta opción permitirá al cliente ver en la página "Mi Perfil" los contactos asociados a su cuenta.';
        this.modalImage = 'assets/images/config/contactos.jpg';
        break;
      case 3:
        this.modalTitle = 'Exportar a Excel';
        this.modalContent = 'Al habilitar esta opción permitirá exportar a Excel en las siguientes pantallas: Dashboard Cliente (Documentos pendientes, vencidos y por vencer), ' +
          'Mis compras (Compras facturadas, notas de venta pendientes de facturar y Productos) y Estado de cuenta ';
        this.modalImage = 'assets/images/config/exportar_excel.jpg';
        break;
      case 4:
        this.modalTitle = 'Cantidad Compras Dashboard';
        this.modalContent = 'Cantidad de últimas compras a mostrar en Dashboard cliente.';
        this.modalImage = 'assets/images/config/cantidad_compras.jpg';
        break;
      case 5:
        this.modalTitle = 'Mostrar Estado Sobregiro';
        this.modalContent = 'Al habilitar esta opción mostrará si el cliente se encuentra sobregirado en el dashboard cliente.';
        this.modalImage = 'assets/images/config/muestra_sobregiro_cliente.jpg';
        break;
      case 6:
        this.modalTitle = 'Habilitar Paga Tú Cuenta';
        this.modalContent = 'Habilita la opción  de realizar un Paga Tú Cuenta en la página  de inicio.';
        this.modalImage = 'assets/images/config/pago_rapido.jpg';
        break;
      case 7:
        this.modalTitle = 'Permite Abono Parcial';
        this.modalContent = 'Si está habilitado, aparece columna adicional en estado cuenta que permite al cliente realizar un pago parcial.';
        this.modalImage = 'assets/images/config/abono_parcial.jpg';
        break;
      case 8:
        this.modalTitle = 'Habilitar Búsqueda por Documento Paga Tú Cuenta';
        this.modalContent = 'Al habilitar esta opción se permitirá Paga Tú Cuenta por número de documento. Para modificar esta opción debe estar habilitado Paga Tú Cuenta';
        this.modalImage = 'assets/images/config/doc_pago_rapido.jpg';
        break;
      case 9:
        this.modalTitle = 'Cantidad Días Documentos Por Vencer';
        this.modalContent = 'Cantidad de días antes del vencimiento desde los cuales los documentos se considerarán "Por Vencer", tanto en el Dashboard administrador como en el Dashboard cliente (Si no se indica dias o se deja en 0 se considerarán todos los documentos que esten en estado "Pendiente").';
        this.modalImage = 'assets/images/config/dias_por_vencer.jpg';
        break;
      case 10:
        this.modalTitle = 'Color Fondo';
        this.modalContent = 'Color del fondo de la pantalla de inicio del portal en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorFondoPortada.jpg';
        break;
      case 11:
        this.modalTitle = 'Color Botón Paga Tú Cuenta';
        this.modalContent = 'Color del botón "PAGA TU CUENTA" de la pantalla de inicio del portal en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonPagoRapido.jpg';
        break;
      case 12:
        this.modalTitle = 'Color Botón Inicio de Sesión';
        this.modalContent = 'Color del botón "INICIAR SESIÓN" de la pantalla de inicio del portal en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonInicioSesion.jpg';
        break;
      case 13:
        this.modalTitle = 'Color Botón Pagar';
        this.modalContent = 'Color del botón "PAGAR" de la pantalla de inicio del portal en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonPagar.jpg';
        break;
      case 14:
        this.modalTitle = 'Logo';
        this.modalContent = 'Logo mostrado en la pantalla de inicio del portal.';
        this.modalImage = 'assets/images/config/logoPortada.jpg';
        break;
      case 15:
        this.modalTitle = 'Imagen Portada';
        this.modalContent = 'Imagen mostrada en la pantalla de inicio del portal.';
        this.modalImage = 'assets/images/config/imagenFondoInicio.jpg';
        break;
      case 16:
        this.modalTitle = 'Favicon';
        this.modalContent = 'Icono mostrado como favicon del portal, este debe ser en formato .ico.';
        this.modalImage = 'assets/images/config/faviconInicio.jpg';
        break;
      case 17:
        this.modalTitle = 'Logo Principal';
        this.modalContent = 'Logo principal mostrado en el menú lateral del portal.';
        this.modalImage = 'assets/images/config/logoSidebar.jpg';
        break;
      case 18:
        this.modalTitle = 'Logo Secundario';
        this.modalContent = 'Logo secundario mostrado en el menú lateral del portal.';
        this.modalImage = 'assets/images/config/logoSecundarioSideBar.jpg';
        break;
      case 19:
        this.modalTitle = 'Banner';
        this.modalContent = 'Imagen mostrada en el banner de la pantalla de Paga Tú Cuenta.';
        this.modalImage = 'assets/images/config/bannerPagoRapido.jpg';
        break;
      case 20:
        this.modalTitle = 'Título Documentos Pendientes';
        this.modalContent = 'Título que se mostrará en el recuadro de documentos pendientes en la pantalla Mi Dashboard del cliente.';
        this.modalImage = 'assets/images/config/tituloDocumentosPendientes.jpg';
        break;
      case 21:
        this.modalTitle = 'Título Documentos Vencidos';
        this.modalContent = 'Título que se mostrará en el recuadro de documentos vencidos  en la pantalla Mi Dashboard del cliente.';
        this.modalImage = 'assets/images/config/tituloDocumentosVencidos.jpg';
        break;
      case 22:
        this.modalTitle = 'Título Documentos Por Vencer';
        this.modalContent = 'Título que se mostrará en el recuadro de documentos en la pantalla Mi Dashboard del cliente.';
        this.modalImage = 'assets/images/config/tituloDocumentosPorVencer.jpg';
        break;
      case 23:
        this.modalTitle = 'Título Últimas Compras';
        this.modalContent = 'Título que se mostrará en la sección "Últimas Compras" en la pantalla Mi Dashboard del cliente.';
        this.modalImage = 'assets/images/config/tituloUltimasCompras.jpg';
        break;
      case 24:
        this.modalTitle = 'Título Moneda Principal';
        this.modalContent = 'Título que se mostrará en la pestaña del detalle de documentos pendientes, vencidos y por vencer  en la pantalla Mi Dashboard del cliente, generados en la moneda seleccionada como Moneda Nacional en la configuración del portal.';
        this.modalImage = 'assets/images/config/tituloMonedaPrincipal.jpg';
        break;
      case 25:
        this.modalTitle = 'Título Segunda Moneda';
        this.modalContent = 'Título que se mostrará en la pestaña del detalle de documentos pendientes, vencidos y por vencer  en la pantalla Mi Dashboard del cliente, generados en la moneda seleccionada como Segunda Moneda en la configuración del portal.';
        this.modalImage = 'assets/images/config/tituloSegundaMoneda.jpg';
        break;
      case 26:
        this.modalTitle = 'Texto Aviso';
        this.modalContent = 'Texto de notificación que se mostrará en el dashboard del cliente y en la pantalla de Paga Tú cuenta.';
        this.modalImage = 'assets/images/config/textoAviso.jpg';
        break;
      case 27:
        this.modalTitle = 'Color Recuadro Documentos Pendientes';
        this.modalContent = 'Color del recuadro de Documentos Pendientes de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosPendientes.jpg';
        break;
      case 28:
        this.modalTitle = 'Color Texto Documentos Pendientes';
        this.modalContent = 'Color del texto del recuadro de Documentos Pendientes de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosPendientes.jpg';
        break;
      case 29:
        this.modalTitle = 'Color Recuadro Documentos Vencidos';
        this.modalContent = 'Color del recuadro de Documentos Vencidos de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosVencidos.jpg';
        break;
      case 30:
        this.modalTitle = 'Color Texto Documentos Vencidos';
        this.modalContent = 'Color del texto del recuadro de Documentos Vencidos de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosVencidos.jpg';
        break;
      case 31:
        this.modalTitle = 'Color Recuadro Documentos Por Vencer';
        this.modalContent = 'Color del recuadro de Documentos Por Vencer de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosPorVencer.jpg';
        break;
      case 32:
        this.modalTitle = 'Color Texto Documentos Por Vencer';
        this.modalContent = 'Color del texto del recuadro de Documentos Vencidos de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorDocumentosPorVencer.jpg';
        break;
      case 33:
        this.modalTitle = 'Color Selección Recuadro Documentos';
        this.modalContent = 'Color del recuadro de Documentos pendientes, vencidos y por vencer al ser seleccionados para ver el detalle en la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorSeleccionDocumentos.jpg';
        break;
      case 34:
        this.modalTitle = 'Color Fondo Últimas Compras';
        this.modalContent = 'Color del fondo para el título de la sección "Últimas Compras" de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorFondoUltimasCompras.jpg';
        break;
      case 35:
        this.modalTitle = 'Color Botón Detalle Últimas Compras';
        this.modalContent = 'Color del Botón "Ver Detalle"  de la sección Últimas Compras de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonDetalleUltimasCompras.jpg';
        break;
      case 36:
        this.modalTitle = 'Color Selección Botón Detalle Últimas Compras';
        this.modalContent = 'Color del botón "Ver Detalle"  de la sección Últimas Compras de la pantalla Mi Dashboard del cliente al pasar el cursor sobre el, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorSeleccionBotonDetalle.jpg';
        break;
      case 37:
        this.modalTitle = 'Color Texto Botón Detalle Últimas Compras';
        this.modalContent = 'Color del texto del botón "Ver Detalle"  de la sección Últimas Compras de la pantalla Mi Dashboard del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonDetalleUltimasCompras.jpg';
        break;
      case 38:
        this.modalTitle = 'Imagen Últimas Compras';
        this.modalContent = 'Imagen o Icono mostrado en la sección Últimas Compras de la pantalla Mi Dashboard del cliente.';
        this.modalImage = 'assets/images/config/imagenUltimasCompras.jpg';
        break;
      case 39:
        this.modalTitle = 'Título Mis Compras';
        this.modalContent = 'Título mostrado en la pantalla Mis Compras.';
        this.modalImage = 'assets/images/config/tituloMisCompras.jpg';
        break;
      case 40:
        this.modalTitle = 'Título Compras Facturadas';
        this.modalContent = 'Título que se mostrará en el recuadro de Compras Facturadas en la pantalla Mis Compras del cliente.';
        this.modalImage = 'assets/images/config/tituloComprasFacturadas.jpg';
        break;
      case 41:
        this.modalTitle = 'Título Compras Pendientes Por Facturar';
        this.modalContent = 'Título que se mostrará en el recuadro de Compras Pendientes Por Facturar en la pantalla Mis Compras del cliente.';
        this.modalImage = 'assets/images/config/tituloComprasPendientes.jpg';
        break;
      case 42:
        this.modalTitle = 'Título Productos Comprados';
        this.modalContent = 'Título que se mostrará en el recuadro de Productos Comprados en la pantalla Mis Compras del cliente.';
        this.modalImage = 'assets/images/config/tituloProductosComprados.jpg';
        break;
      case 43:
        this.modalTitle = 'Título Guias Pendientes de Facturar';
        this.modalContent = 'Título que se mostrará en el recuadro Guias Pendientes de Facturar en la pantalla Mis Compras del cliente.';
        this.modalImage = 'assets/images/config/tituloGuiasPendientes.jpg';
        break;
      case 44:
        this.modalTitle = 'Color Recuadro Compras Facturadas';
        this.modalContent = 'Color del recuadro de Compras Facturadas de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/fondoComprasFacturadas.jpg';
        break;
      case 45:
        this.modalTitle = 'Color Recuadro Compras Pendientes Por Facturar';
        this.modalContent = 'Color del recuadro de Compras Pendientes de Facturar de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/fondoComprasPendientes.jpg';
        break;
      case 46:
        this.modalTitle = 'Color Recuadro Productos Comprados';
        this.modalContent = 'Color del recuadro Productos Comprados de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/fondoProductosComprados.jpg';
        break;
      case 47:
        this.modalTitle = 'Color Recuadro Guias Pendientes de Facturar';
        this.modalContent = 'Color del recuadro Guias Pendientes de Facturar de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorGuiasPendientes.jpg';
        break;
      case 48:
        this.modalTitle = 'Color Texto Recuadros Mis Compras';
        this.modalContent = 'Color del texto en los recuadros de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorTextoMisCompras.jpg';
        break;
      case 49:
        this.modalTitle = 'Color Botón Buscar';
        this.modalContent = 'Color del botón "Buscar" en el detalle de la pantalla Mis Compras del cliente, en formato hexadecimal.';
        this.modalImage = 'assets/images/config/colorBotonBuscar.jpg';
        break;
      case 50:
        this.modalTitle = 'Icono Mis Compras';
        this.modalContent = 'Icono mostrado en la pantalla Mis Compras.';
        this.modalImage = 'assets/images/config/iconoMisCompras.jpg';
        break;
      case 51:
        this.modalTitle = 'Banner Mis Compras';
        this.modalContent = 'Banner mostrado en la pantalla Mis Compras.';
        this.modalImage = 'assets/images/config/bannerMisCompras.jpg';
        break;
      case 52:
        this.modalTitle = 'Color Botón Modificar Datos';
        this.modalContent = 'Color del botón "Modificar Datos" de la pantalla Mi Perfil del cliente.';
        this.modalImage = 'assets/images/config/botonModificarDatos.jpg';
        break;
      case 53:
        this.modalTitle = 'Color Botón Cambio de Clave';
        this.modalContent = 'Color del botón "Cambio de Clave" de la pantalla Mi Perfil del cliente.';
        this.modalImage = 'assets/images/config/botonCambioClave.jpg';
        break;
      case 54:
        this.modalTitle = 'Color Botón Estado de Cuenta';
        this.modalContent = 'Color del botón "Estado de Cuenta" de la pantalla Mi Perfil del cliente.';
        this.modalImage = 'assets/images/config/botonEstadoCuenta.jpg';
        break;
      case 55:
        this.modalTitle = 'Color Seleccion Botón';
        this.modalContent = 'Color de los botón "Moficar Datos", "Cambio de Clave" y "Estado de Cuenta" de la pantalla Mi Perfil del cliente al pasar el cursor por encima.';
        this.modalImage = 'assets/images/config/botonSeleccionPerfil.jpg';
        break;
      case 56:
        this.modalTitle = 'Color Botón Cancelar pop up';
        this.modalContent = 'Color del botón "Cancelar" en la ventana emergente al seleccionar Modificar Datos.';
        this.modalImage = 'assets/images/config/colorBotonCancelar.jpg';
        break;
      case 57:
        this.modalTitle = 'Color Botón Guardar pop up';
        this.modalContent = 'Color del botón "Guardar" en la ventana emergente al seleccionar Modificar Datos o Cambio de Clave.';
        this.modalImage = 'assets/images/config/colorBotonGuardar.jpg';
        break;
      case 58:
        this.modalTitle = 'Icono Mi Perfil';
        this.modalContent = 'Icono mostrado en la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/iconoMiPerfil.jpg';
        break;
      case 59:
        this.modalTitle = 'Banner Mi Perfil';
        this.modalContent = 'Banner mostrado en la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/bannerMiPerfil.jpg';
        break;
      case 60:
        this.modalTitle = 'Icono Contactos';
        this.modalContent = 'Icono mostrado en pestaña Contactos de la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/iconoContactos.jpg';
        break;
      case 61:
        this.modalTitle = 'Icono Cambio de Clave';
        this.modalContent = 'Icono mostrado sobre el botón Cambio de Clave Datos en la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/iconoCambioClave.jpg';
        break;
      case 62:
        this.modalTitle = 'Icono Modificar Datos';
        this.modalContent = 'Icono mostrado sobre el botón Modificar Datos en la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/iconoModificarDatos.jpg';
        break;
      case 63:
        this.modalTitle = 'Icono Estado de Cuenta';
        this.modalContent = 'Icono mostrado sobre el botón Estado de Cuenta en la pantalla Mi Perfil.';
        this.modalImage = 'assets/images/config/iconoEstadoCuenta.jpg';
        break;
    }
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  openImage() {
    if (this.modalImage.length > 0) {
      var win = window.open(`${window.location.origin}/${this.modalImage}`, '_blank');
      win.focus();
    }
  }

  MostrarSeccion(seccion: number) {

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

  verificaColor(color: number) {
    let pattern = /^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$/

    switch (color) {
      case 1:
        if (this.configuracionDiseno.colorFondoPortada != '' && this.configuracionDiseno.colorFondoPortada != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoPortada)) {
            this.configuracionDiseno.colorFondoPortada = '';
          }
        }
        break;
      case 2:
        if (this.configuracionDiseno.colorBotonPagoRapido != '' && this.configuracionDiseno.colorBotonPagoRapido != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonPagoRapido)) {
            this.configuracionDiseno.colorBotonPagoRapido = '';
          }
        }
        break;

      case 3:
        if (this.configuracionDiseno.colorBotonInicioSesion != '' && this.configuracionDiseno.colorBotonInicioSesion != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonInicioSesion)) {
            this.configuracionDiseno.colorBotonInicioSesion = '';
          }
        }
        break;

      case 4:
        if (this.configuracionDiseno.colorBotonPagar != '' && this.configuracionDiseno.colorBotonPagar != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonPagar)) {
            this.configuracionDiseno.colorBotonPagar = '';
          }
        }
        break;

      case 5:
        if (this.configuracionDiseno.colorFondoDocumentos != '' && this.configuracionDiseno.colorFondoDocumentos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoDocumentos)) {
            this.configuracionDiseno.colorFondoDocumentos = '';
          }
        }
        break;

      case 6:
        if (this.configuracionDiseno.colorTextoPendientes != '' && this.configuracionDiseno.colorTextoPendientes != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorTextoPendientes)) {
            this.configuracionDiseno.colorTextoPendientes = '';
          }
        }
        break;
      case 7:
        if (this.configuracionDiseno.colorFondoVencidos != '' && this.configuracionDiseno.colorFondoVencidos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoVencidos)) {
            this.configuracionDiseno.colorFondoVencidos = '';
          }
        }
        break;
      case 8:
        if (this.configuracionDiseno.colorTextoVencidos != '' && this.configuracionDiseno.colorTextoVencidos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorTextoVencidos)) {
            this.configuracionDiseno.colorTextoVencidos = '';
          }
        }
        break;

      case 9:
        if (this.configuracionDiseno.colorFondoPorVencer != '' && this.configuracionDiseno.colorFondoPorVencer != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoPorVencer)) {
            this.configuracionDiseno.colorFondoPorVencer = '';
          }
        }
        break;

      case 10:
        if (this.configuracionDiseno.colorTextoPorVencer != '' && this.configuracionDiseno.colorTextoPorVencer != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorTextoPorVencer)) {
            this.configuracionDiseno.colorTextoPorVencer = '';
          }
        }

      case 11:
        if (this.configuracionDiseno.colorSeleccionDocumentos != '' && this.configuracionDiseno.colorSeleccionDocumentos != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorSeleccionDocumentos)) {
            this.configuracionDiseno.colorSeleccionDocumentos = '';
          }
        }
        break;

      case 12:
        if (this.configuracionDiseno.colorFondoUltimasCompras != '' && this.configuracionDiseno.colorFondoUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoUltimasCompras)) {
            this.configuracionDiseno.colorFondoUltimasCompras = '';
          }
        }
        break;

      case 13:
        if (this.configuracionDiseno.colorBotonUltimasCompras != '' && this.configuracionDiseno.colorBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonUltimasCompras)) {
            this.configuracionDiseno.colorBotonUltimasCompras = '';
          }
        }
        break;

      case 14:
        if (this.configuracionDiseno.colorHoverBotonUltimasCompras != '' && this.configuracionDiseno.colorHoverBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorHoverBotonUltimasCompras)) {
            this.configuracionDiseno.colorHoverBotonUltimasCompras = '';
          }
        }
        break;

      case 15:
        if (this.configuracionDiseno.colorTextoBotonUltimasCompras != '' && this.configuracionDiseno.colorTextoBotonUltimasCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorTextoBotonUltimasCompras)) {
            this.configuracionDiseno.colorTextoBotonUltimasCompras = '';
          }
        }
        break;

      case 16:
        if (this.configuracionDiseno.colorFondoMisCompras != '' && this.configuracionDiseno.colorFondoMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoMisCompras)) {
            this.configuracionDiseno.colorFondoMisCompras = '';
          }
        }
        break;

      case 17:
        if (this.configuracionDiseno.colorFondoPendientesMisCompras != '' && this.configuracionDiseno.colorFondoPendientesMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoPendientesMisCompras)) {
            this.configuracionDiseno.colorFondoPendientesMisCompras = '';
          }
        }
        break;

      case 18:
        if (this.configuracionDiseno.colorFondoProductosMisCompras != '' && this.configuracionDiseno.colorFondoProductosMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoProductosMisCompras)) {
            this.configuracionDiseno.colorFondoProductosMisCompras = '';
          }
        }
        break;

      case 19:
        if (this.configuracionDiseno.colorFondoGuiasMisCompras != '' && this.configuracionDiseno.colorFondoGuiasMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorFondoGuiasMisCompras)) {
            this.configuracionDiseno.colorFondoGuiasMisCompras = '';
          }
        }
        break;

      case 20:
        if (this.configuracionDiseno.colorIconosMisCompras != '' && this.configuracionDiseno.colorIconosMisCompras != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorIconosMisCompras)) {
            this.configuracionDiseno.colorIconosMisCompras = '';
          }
        }
        break;

      case 21:
        if (this.configuracionDiseno.colorBotonBuscar != '' && this.configuracionDiseno.colorBotonBuscar != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonBuscar)) {
            this.configuracionDiseno.colorBotonBuscar = '';
          }
        }
        break;

      case 22:
        if (this.configuracionDiseno.colorBotonModificarPerfil != '' && this.configuracionDiseno.colorBotonModificarPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonModificarPerfil)) {
            this.configuracionDiseno.colorBotonModificarPerfil = '';
          }
        }
        break;

      case 23:
        if (this.configuracionDiseno.colorBotonClavePerfil != '' && this.configuracionDiseno.colorBotonClavePerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonClavePerfil)) {
            this.configuracionDiseno.colorBotonClavePerfil = '';
          }
        }
        break;

      case 24:
        if (this.configuracionDiseno.colorBotonEstadoPerfil != '' && this.configuracionDiseno.colorBotonEstadoPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonEstadoPerfil)) {
            this.configuracionDiseno.colorBotonEstadoPerfil = '';
          }
        }
        break;

      case 25:
        if (this.configuracionDiseno.colorHoverBotonesPerfil != '' && this.configuracionDiseno.colorHoverBotonesPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorHoverBotonesPerfil)) {
            this.configuracionDiseno.colorHoverBotonesPerfil = '';
          }
        }
        break;

      case 26:
        if (this.configuracionDiseno.colorBotonCancelarModalPerfil != '' && this.configuracionDiseno.colorBotonCancelarModalPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonCancelarModalPerfil)) {
            this.configuracionDiseno.colorBotonCancelarModalPerfil = '';
          }
        }
        break;

      case 27:
        if (this.configuracionDiseno.colorBotonGuardarModalPerfil != '' && this.configuracionDiseno.colorBotonGuardarModalPerfil != null) {  //FCA 08-06-2022
          if (!pattern.test(this.configuracionDiseno.colorBotonGuardarModalPerfil)) {
            this.configuracionDiseno.colorBotonGuardarModalPerfil = '';
          }
        }
        break;
    }



  }

  onChange(event: any, imagen: number) {
    switch (imagen) {
      case 1:
        this.defaultImageLogoPortada = event.srcElement.files;
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

  preview(event: any, imagen: number) {
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
              this.urlImagenLogoPortada = img;
            }
            reader.readAsDataURL(file);
          }
          this.defaultImageLogoPortada = event.target.files
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

  deletePreviewImg(imagen: number) {
    switch (imagen) {
      case 1:
        this.urlImagenLogoPortada = null;
        this.defaultImageLogoPortada = null;
        this.clearInput(document.getElementById('fileImageLogoPortada'));
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

  clearInput(input) {
    try {
      input.value = null;
    } catch (ex) { }
    if (input.value) {
      input.parentNode.replaceChild(input.cloneNode(true), input);
    }
  }

  saveConfigDiseno(seccion: number) {
    switch (seccion) {
      case 1:
        if (this.configuracionDiseno.colorFondoPortada == '' || this.configuracionDiseno.colorFondoPortada == null) {
          this.notificationService.warning('Debe seleccionar color de fondo', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonPagoRapido == '' || this.configuracionDiseno.colorBotonPagoRapido == null) {
          this.notificationService.warning('Debe seleccionar color para el boton de Paga Tú Cuenta', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonInicioSesion == '' || this.configuracionDiseno.colorBotonInicioSesion == null) {
          this.notificationService.warning('Debe seleccionar color para el boton de inicio de sesión', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonPagar == '' || this.configuracionDiseno.colorBotonPagar == null) {
          this.notificationService.warning('Debe seleccionar color para el boton pagar', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonPagar == '' || this.configuracionDiseno.colorBotonPagar == null) {
          this.notificationService.warning('Debe seleccionar color para el boton pagar', '', true);
          return;
        }

        if (this.defaultImageLogoPortada == null && (this.configuracionDiseno.logoPortada == null || this.configuracionDiseno.logoPortada == '')) {
          this.notificationService.warning('Debe seleccionar un Logo', '', true);
          return;
        }

        if (this.defaultImagenPortada == null && (this.configuracionDiseno.imagenPortada == null || this.configuracionDiseno.imagenPortada == '')) {
          this.notificationService.warning('Debe seleccionar una imagen de portada', '', true);
          return;
        }

        // if (this.defaultImageFavicon == null && (this.configuracionDiseno.favicon == null || this.configuracionDiseno.favicon == '')) {
        //   this.notificationService.warning('Debe seleccionar una imagen de portada', '', true);
        //   return;
        // }

        this.spinner.show();
        this.configuracionDisenoService.saveConfigDiseno(this.configuracionDiseno, seccion).subscribe(res => {
          this.uploadImage(seccion);
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al guardar datos', '', true); });
        break;

      case 2:
        if (this.defaultImageLogoPrincipalSidebar == null && (this.configuracionDiseno.logoSidebar == null || this.configuracionDiseno.logoSidebar == '')) {
          this.notificationService.warning('Debe seleccionar un Logo Principal', '', true);
          return;
        }
        if (this.defaultImageLogoSecundarioSidebar == null && (this.configuracionDiseno.logoMinimalistaSidebar == null || this.configuracionDiseno.logoMinimalistaSidebar == '')) {
          this.notificationService.warning('Debe seleccionar un Logo Secundario', '', true);
          return;
        }
        this.uploadImage(seccion);
        break;

      case 3:
        if (this.defaultImageBannerPagoRapido == null && (this.configuracionDiseno.bannerPagoRapido == null || this.configuracionDiseno.bannerPagoRapido == '')) {
          this.notificationService.warning('Debe seleccionar un Banner', '', true);
          return;
        }
        if (this.defaultImageBannerPagoRapido != null) {
          this.uploadImage(seccion);
        } else {
          this.notificationService.success('Datos guardados correctamente', '', true);
        }
        break;

      case 4:
        if (this.configuracionDiseno.tituloPendientesDashboard == '' || this.configuracionDiseno.tituloPendientesDashboard == null) {
          this.notificationService.warning('Debe seleccionar título para los documentos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloVencidosDashboard == '' || this.configuracionDiseno.tituloVencidosDashboard == null) {
          this.notificationService.warning('Debe seleccionar título para los documentos vencidos', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloPorVencerDashboard == '' || this.configuracionDiseno.tituloPorVencerDashboard == null) {
          this.notificationService.warning('Debe seleccionar título para los documentos por vencer', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloUltimasCompras == '' || this.configuracionDiseno.tituloUltimasCompras == null) {
          this.notificationService.warning('Debe seleccionar título para las ultimas compras', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloMonedaPeso == '' || this.configuracionDiseno.tituloMonedaPeso == null) {
          this.notificationService.warning('Debe seleccionar título para la moneda principal', '', true);
          return;
        }

        if ((this.configPagoClientes.segundaMonedaUtilizada != '' && this.configPagoClientes.segundaMonedaUtilizada != null) && this.configuracionDiseno.tituloOtraMoneda == '' || this.configuracionDiseno.tituloOtraMoneda == null) {
          this.notificationService.warning('Debe seleccionar título para la moneda secundaria', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoDocumentos == '' || this.configuracionDiseno.colorFondoDocumentos == null) {
          this.notificationService.warning('Debe seleccionar color para recuadro documentos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorTextoPendientes == '' || this.configuracionDiseno.colorTextoPendientes == null) {
          this.notificationService.warning('Debe seleccionar color para texto docuementos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoVencidos == '' || this.configuracionDiseno.colorFondoVencidos == null) {
          this.notificationService.warning('Debe seleccionar color para texto docuementos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorTextoVencidos == '' || this.configuracionDiseno.colorTextoVencidos == null) {
          this.notificationService.warning('Debe seleccionar color para texto docuementos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoPorVencer == '' || this.configuracionDiseno.colorFondoPorVencer == null) {
          this.notificationService.warning('Debe seleccionar color para texto docuementos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorTextoPorVencer == '' || this.configuracionDiseno.colorTextoPorVencer == null) {
          this.notificationService.warning('Debe seleccionar color para texto docuementos pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorSeleccionDocumentos == '' || this.configuracionDiseno.colorSeleccionDocumentos == null) {
          this.notificationService.warning('Debe seleccionar color para seleccion recuadro documentos', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoUltimasCompras == '' || this.configuracionDiseno.colorFondoUltimasCompras == null) {
          this.notificationService.warning('Debe seleccionar color fondo ultimas compras', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonUltimasCompras == '' || this.configuracionDiseno.colorBotonUltimasCompras == null) {
          this.notificationService.warning('Debe seleccionar color boton detalle ultimas compras', '', true);
          return;
        }

        if (this.configuracionDiseno.colorHoverBotonUltimasCompras == '' || this.configuracionDiseno.colorHoverBotonUltimasCompras == null) {
          this.notificationService.warning('Debe seleccionar color para selección boton ultimas compras', '', true);
          return;
        }

        if (this.configuracionDiseno.colorTextoBotonUltimasCompras == '' || this.configuracionDiseno.colorTextoBotonUltimasCompras == null) {
          this.notificationService.warning('Debe seleccionar color para texto boton ultimas compras', '', true);
          return;
        }

        if (this.defaultImageUltimasCompras == null && (this.configuracionDiseno.imagenUltimasCompras == null || this.configuracionDiseno.imagenUltimasCompras == '')) {
          this.notificationService.warning('Debe seleccionar una imagen para ultimas compras', '', true);
          return;
        }

        this.spinner.show();
        this.configuracionDisenoService.saveConfigDiseno(this.configuracionDiseno, seccion).subscribe(res => {
          if (this.defaultImageUltimasCompras != null || this.defaultImageUltimasCompras != null) {
            this.uploadImage(seccion);
          } else {
            this.spinner.hide();
            this.notificationService.success('Datos guardados correctamente', '', true);
          }
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al guardar datos', '', true); });
        break;

      case 5:
        if (this.configuracionDiseno.tituloMisCompras == '' || this.configuracionDiseno.tituloMisCompras == null) {
          this.notificationService.warning('Debe seleccionar título mis compras', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloComprasFacturadas == '' || this.configuracionDiseno.tituloComprasFacturadas == null) {
          this.notificationService.warning('Debe seleccionar título recuadro compras facturadas', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloPendientesFacturar == '' || this.configuracionDiseno.tituloPendientesFacturar == null) {
          this.notificationService.warning('Debe seleccionar título recuadro Compras Pendientes de Facturar', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloProductos == '' || this.configuracionDiseno.tituloProductos == null) {
          this.notificationService.warning('Debe seleccionar título recuadro Productos', '', true);
          return;
        }

        if (this.configuracionDiseno.tituloGuiasPendientes == '' || this.configuracionDiseno.tituloGuiasPendientes == null) {
          this.notificationService.warning('Debe seleccionar título recuadro Guias Pendientes', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoMisCompras == '' || this.configuracionDiseno.colorFondoMisCompras == null) {
          this.notificationService.warning('Debe seleccionar color para recuadro Compras Facturadas', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoPendientesMisCompras == '' || this.configuracionDiseno.colorFondoPendientesMisCompras == null) {
          this.notificationService.warning('Debe seleccionar color para recuadro Compras Pendientes Facturadas', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoProductosMisCompras == '' || this.configuracionDiseno.colorFondoProductosMisCompras == null) {
          this.notificationService.warning('Debe seleccionar color para recuadro Productos', '', true);
          return;
        }

        if (this.configuracionDiseno.colorFondoGuiasMisCompras == '' || this.configuracionDiseno.colorFondoGuiasMisCompras == null) {
          this.notificationService.warning('Debe seleccionar color para recuadro Guias Pendientes de Facturar', '', true);
          return;
        }

        if (this.configuracionDiseno.colorIconosMisCompras == '' || this.configuracionDiseno.colorIconosMisCompras == null) {
          this.notificationService.warning('Debe seleccionar color para Texto Recuadros Mis Compras', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonBuscar == '' || this.configuracionDiseno.colorBotonBuscar == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Buscar', '', true);
          return;
        }

        if (this.defaultImageMisCompras == null && (this.configuracionDiseno.iconoMisCompras == null || this.configuracionDiseno.iconoMisCompras == '')) {
          this.notificationService.warning('Debe seleccionar un icono para Mis Compras', '', true);
          return;
        }

        if (this.defaultImageBannerMisCompras == null && (this.configuracionDiseno.bannerMisCompras == null || this.configuracionDiseno.bannerMisCompras == '')) {
          this.notificationService.warning('Debe seleccionar un banner para Mis Compras', '', true);
          return;
        }

        this.spinner.show();
        this.configuracionDisenoService.saveConfigDiseno(this.configuracionDiseno, seccion).subscribe(res => {
          if (this.defaultImageBannerMisCompras != null || this.defaultImageMisCompras != null) {
            this.uploadImage(seccion);
          } else {
            this.spinner.hide();
            this.notificationService.success('Datos guardados correctamente', '', true);
          }
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al guardar datos', '', true); });
        break;

      case 6:
        if (this.configuracionDiseno.colorBotonModificarPerfil == '' || this.configuracionDiseno.colorBotonModificarPerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Modificar Datos', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonClavePerfil == '' || this.configuracionDiseno.colorBotonClavePerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Cambio de Clave', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonEstadoPerfil == '' || this.configuracionDiseno.colorBotonEstadoPerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Estado de Cuenta', '', true);
          return;
        }

        if (this.configuracionDiseno.colorHoverBotonesPerfil == '' || this.configuracionDiseno.colorHoverBotonesPerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Seleccion Botón', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonCancelarModalPerfil == '' || this.configuracionDiseno.colorBotonCancelarModalPerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Cancelar pop up', '', true);
          return;
        }

        if (this.configuracionDiseno.colorBotonGuardarModalPerfil == '' || this.configuracionDiseno.colorBotonGuardarModalPerfil == null) {
          this.notificationService.warning('Debe seleccionar color para Boton Guardar pop up', '', true);
          return;
        }

        if (this.defaultImagenUsuario == null && (this.configuracionDiseno.imagenUsuario == null || this.configuracionDiseno.imagenUsuario == '')) {
          this.notificationService.warning('Debe seleccionar Imagen Mi Perfil', '', true);
          return;
        }

        if (this.defaultImageBannerPerfil == null && (this.configuracionDiseno.bannerPortal == null || this.configuracionDiseno.bannerPortal == '')) {
          this.notificationService.warning('Debe seleccionar Banner Mi Perfil', '', true);
          return;
        }

        if (this.defaultImageIconoContactos == null && (this.configuracionDiseno.iconoContactos == null || this.configuracionDiseno.iconoContactos == '')) {
          this.notificationService.warning('Debe seleccionar Icono Contactos', '', true);
          return;
        }

        if (this.defaultImageIconoClavePerfil == null && (this.configuracionDiseno.iconoClavePerfil == null || this.configuracionDiseno.iconoClavePerfil == '')) {
          this.notificationService.warning('Debe seleccionar Icono Cambio de Clave', '', true);
          return;
        }

        if (this.defaultImageIconoEditarPerfil == null && (this.configuracionDiseno.iconoEditarPerfil == null || this.configuracionDiseno.iconoEditarPerfil == '')) {
          this.notificationService.warning('Debe seleccionar Icono Modificar Datos', '', true);
          return;
        }

        if (this.defaultImageIconoEstadoPerfil == null && (this.configuracionDiseno.iconoEstadoPerfil == null || this.configuracionDiseno.iconoEstadoPerfil == '')) {
          this.notificationService.warning('Debe seleccionar Icono Estado de Cuenta', '', true);
          return;
        }

        this.spinner.show();
        this.configuracionDisenoService.saveConfigDiseno(this.configuracionDiseno, seccion).subscribe(res => {
          this.uploadImage(seccion);
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al guardar datos', '', true); });
        break;
    }


  }


  uploadImage(seccion: number) {
    this.spinner.show();
    switch (seccion) {
      case 1:
        this.configuracionDisenoService.subirImagen(this.defaultImageLogoPortada, 1).then(res => {
          this.defaultImageLogoPortada = null;
          this.configuracionDisenoService.subirImagen(this.defaultImagenPortada, 2).then(res => {
            this.configuracionDisenoService.subirImagen(this.defaultImageFavicon, 15).then(res => {
              this.notificationService.success('Datos guardados correctamente', '', true);
              this.getConfigDiseno();
              this.spinner.hide();
              this.defaultImageFavicon = null;
            }).catch(err => {
              this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Favicon', '', true);
            });
            this.defaultImagenPortada = null;
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Imagen Portada', '', true);
          });
        }).catch(err => {
          this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir  Logo', '', true);
        });
        break;

      case 2:
        if (this.defaultImageLogoPrincipalSidebar != null && this.defaultImageLogoSecundarioSidebar != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageLogoPrincipalSidebar, 3).then(res => {
            this.defaultImageLogoPrincipalSidebar = null;
            this.configuracionDisenoService.subirImagen(this.defaultImageLogoSecundarioSidebar, 4).then(res => {
              this.defaultImageLogoSecundarioSidebar = null;
              this.getConfigDiseno();
              this.notificationService.success('Datos guardados correctamente', '', true);
              this.spinner.hide();
            }).catch(err => {
              this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Logo Secundario', '', true);
            });
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Logo Principal', '', true);
          });
        } else if (this.defaultImageLogoPrincipalSidebar != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageLogoPrincipalSidebar, 3).then(res => {
            this.notificationService.success('Datos guardados correctamente', '', true);
            this.getConfigDiseno();
            this.spinner.hide();
            this.defaultImageLogoPrincipalSidebar = null;
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Logo Principal', '', true);
          });
        } else if (this.defaultImageLogoSecundarioSidebar != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageLogoSecundarioSidebar, 4).then(res => {
            this.notificationService.success('Datos guardados correctamente', '', true);
            this.getConfigDiseno();
            this.spinner.hide();
            this.defaultImageLogoSecundarioSidebar = null;
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Logo Secundario', '', true);
          });
        } else {
          this.spinner.hide(); this.notificationService.success('Datos guardados correctamente', '', true);
        }
        break;

      case 3:
        this.configuracionDisenoService.subirImagen(this.defaultImageBannerPagoRapido, 5).then(res => {
          this.notificationService.success('Datos guardados correctamente', '', true);
          this.getConfigDiseno();
          this.spinner.hide();
          this.defaultImageBannerPagoRapido = null;
        }).catch(err => {
          this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir la imagen', '', true);
        });
        break;

      case 4:
        this.configuracionDisenoService.subirImagen(this.defaultImageUltimasCompras, 6).then(res => {
          this.notificationService.success('Datos guardados correctamente', '', true);
          this.getConfigDiseno();
          this.spinner.hide();
          this.defaultImageUltimasCompras = null;
        }).catch(err => {
          this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir la imagen', '', true);
        });
        break;

      case 5:
        if (this.defaultImageBannerMisCompras != null && this.defaultImageMisCompras != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageMisCompras, 7).then(res => {
            this.defaultImageMisCompras = null;
            this.configuracionDisenoService.subirImagen(this.defaultImageBannerMisCompras, 8).then(res => {
              this.defaultImageBannerMisCompras = null;
              this.getConfigDiseno();
              this.notificationService.success('Datos guardados correctamente', '', true);
              this.spinner.hide();
            }).catch(err => {
              this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir banner Mis Compras', '', true);
            });
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al Icono Mis Compras', '', true);
          });
        } else if (this.defaultImageMisCompras != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageMisCompras, 7).then(res => {
            this.notificationService.success('Datos guardados correctamente', '', true);
            this.getConfigDiseno();
            this.spinner.hide();
            this.defaultImageMisCompras = null;
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Icono Mis Compras', '', true);
          });
        } else if (this.defaultImageBannerMisCompras != null) {
          this.configuracionDisenoService.subirImagen(this.defaultImageBannerMisCompras, 8).then(res => {
            this.notificationService.success('Datos guardados correctamente', '', true);
            this.getConfigDiseno();
            this.spinner.hide();
            this.defaultImageBannerMisCompras = null;
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir banner Mis Compras', '', true);
          });
        } else {
          this.spinner.hide(); this.notificationService.success('Datos guardados correctamente', '', true);
        }
        break;

      case 6:
        this.configuracionDisenoService.subirImagen(this.defaultImagenUsuario, 9).then(res => {
          this.defaultImagenUsuario = null;
          this.configuracionDisenoService.subirImagen(this.defaultImageBannerPerfil, 10).then(res => {
            this.defaultImageBannerPerfil = null;
            this.configuracionDisenoService.subirImagen(this.defaultImageIconoContactos, 11).then(res => {
              this.defaultImageIconoContactos = null;
              this.configuracionDisenoService.subirImagen(this.defaultImageIconoClavePerfil, 12).then(res => {
                this.defaultImageIconoClavePerfil = null;
                this.configuracionDisenoService.subirImagen(this.defaultImageIconoEditarPerfil, 13).then(res => {
                  this.defaultImageIconoEditarPerfil = null;
                  this.configuracionDisenoService.subirImagen(this.defaultImageIconoEstadoPerfil, 14).then(res => {
                    this.defaultImageIconoEstadoPerfil = null;
                    this.spinner.hide();
                    this.notificationService.success('Datos guardados correctamente', '', true);
                    this.getConfigDiseno();
                  }).catch(err => {
                    this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Icono Estado de Cuenta', '', true);
                  });
                }).catch(err => {
                  this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Icono Modificar Datos', '', true);
                });
              }).catch(err => {
                this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Icono Cambio de Clave', '', true);
              });
            }).catch(err => {
              this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Icono Contactos', '', true);
            });
          }).catch(err => {
            this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Banner Mi Perfil', '', true);
          });
        }).catch(err => {
          this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir Imagen Mi Perfil', '', true);
        });
        break;
    }
  }

  public getSantizeUrl(url: string) {
    return this._sanitizer.bypassSecurityTrustUrl(url);
  }

  validaAnio() {
    const currentDate = new Date();
    var anioActual = currentDate.getFullYear();

    if (this.anioTributario > anioActual) {
      this.anioTributario = anioActual;
    }
  }
}

