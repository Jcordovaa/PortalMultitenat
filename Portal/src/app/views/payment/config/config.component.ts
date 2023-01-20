import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionPagoClientesService } from '../../../shared/services/configuracionpagoclientes.service';
import { ConfiguracionPagoCliente, ConfiguracionPortal } from '../../../shared/models/configuracioncobranza.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { number } from 'ngx-custom-validators/src/app/number/validator';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

interface AutoCompleteModel {
  value: any;
  display: string;
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
  cantidadDiasPorVencer: number = 0;

  constructor(private configuracionPagoClientesService: ConfiguracionPagoClientesService,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private configuracionSoftlandService: ConfiguracionSoftlandService, private modalService: NgbModal,) { }

  ngOnInit(): void {
    this.getConfigPagoClientes();
    this.getConfigPortal();
    this.getCentrosCostos();
    this.getAreasNegocio();
  }

  getConfigPagoClientes() {
    this.spinner.show();

    this.configuracionPagoClientesService.getConfigPagoClientes().subscribe((res: any) => {
      if (res) {
        // this.configAll = res;

        this.getMonedas();
        this.configPagoClientes = res;
        this.cantidadDiasPorVencer = this.configPagoClientes.diasPorVencer;

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
    this.configuracionSoftlandService.getMonedas().subscribe((res: any) => {
      this.monedas = res;
    }, err => {
      this.notificationService.error('Ocurrio un error al obtener codigo de monedas.', '', true);
    });
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
    this.spinner.show();
    let cuentasContables = this.selectedCuen.length > 0 ? this.selectedCuen.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let tiposDocumentos = this.tiposDctosSelected.length > 0 ? this.tiposDctosSelected.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    this.configPagoClientes.cuentasContablesDeuda = cuentasContables;
    this.configPagoClientes.tiposDocumentosDeuda = tiposDocumentos;

    this.configuracionPagoClientesService.edit(this.configPagoClientes).subscribe(res => {

      this.notificationService.success('Configuración de pagos actualizada correctamente', '', true);
      this.spinner.hide();
    }, err => {
      this.notificationService.error('Error al actualizar  Configuración de pagos', '', true);
      this.spinner.hide();
    });
  }

  saveParametros() {
    this.spinner.show();
    this.configPortal.muestraEstadoBloqueo = this.configPortal.muestraEstadoBloqueo ? 1 : 0;
    this.configPortal.muestraEstadoSobregiro = this.configPortal.muestraEstadoSobregiro ? 1 : 0;
    //this.configPortal.muestraContactoComercial = this.configPortal.muestraContactoComercial ? 1 : 0;
    this.configPortal.muestraContactosPerfil = this.configPortal.muestraContactosPerfil ? 1 : 0;
    this.configPortal.habilitaPagoRapido = this.configPortal.habilitaPagoRapido ? 1 : 0;
    this.configPortal.permiteExportarExcel = this.configPortal.permiteExportarExcel ? 1 : 0;
    this.configPortal.permiteAbonoParcial = this.configPortal.permiteAbonoParcial ? 1 : 0;
    this.configPortal.utilizaDocumentoPagoRapido = this.configPortal.utilizaDocumentoPagoRapido ? 1 : 0;
    this.configuracionPagoClientesService.editPortal(this.configPortal, this.cantidadDiasPorVencer).subscribe(res => {
      this.configPagoClientes.diasPorVencer = this.cantidadDiasPorVencer;
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
        this.modalContent = 'Al habilitar esta opcion se mostrara el estado "Desbloqueado" o "Bloqueado" en el dashboard del cliente.';
        this.modalImage = 'assets/images/config/muestra_estado_cliente.jpg';
        break;
      case 2:
        this.modalTitle = 'Mostrar Contactos en Perfil';
        this.modalContent = 'Al habilitar esta opción permitira al cliente ver en la pagina "Mi Perfil" los contactos asociados a su cuenta.';
        this.modalImage = 'assets/images/config/contactos.jpg';
        break;
      case 3:
        this.modalTitle = 'Exportar a Excel';
        this.modalContent = 'Al habilitar esta opcion permitira exportar a excel en las siguientes pantallas: Dashboard Cliente (Documentos pendientes, vencidos y por vencer), ' +
          'Mis compras (Compras facturadas, notas de venta pendientes de facturar y Productos) y Estado de cuenta ';
        this.modalImage = 'assets/images/config/exportar_excel.jpg';
        break;
      case 4:
        this.modalTitle = 'Cantidad Compras Dashboard';
        this.modalContent = 'Cantidad de ultimas compras a mostrar en dashboard cliente';
        this.modalImage = 'assets/images/config/cantidad_compras.jpg';
        break;
      case 5:
        this.modalTitle = 'Mostrar Estado Sobregiro';
        this.modalContent = 'Al habilitar esta opcion mostrara si el cliente se encuentra sobregirado en el dashboard cliente.';
        this.modalImage = 'assets/images/config/muestra_sobregiro_cliente.jpg';
        break;
      case 6:
        this.modalTitle = 'Habilitar Pago Rapido';
        this.modalContent = 'Habilita la opcion de realizar un pago rapido en la pagina de inicio.';
        this.modalImage = 'assets/images/config/pago_rapido.jpg';
        break;
      case 7:
        this.modalTitle = 'Permite Abono Parcial';
        this.modalContent = 'Si esta habilitado aparece columna adicional en estado cuenta que permite al cliente realizar un pago parcial.';
        this.modalImage = 'assets/images/config/abono_parcial.jpg';
        break;
      case 8:
        this.modalTitle = 'Habilitar Busqueda por Documento Pago Rapido';
        this.modalContent = 'Al habilitar esta opción se permitira el pago rapido por número de documento. Para modificar esta opción debe estar habilitado el Pago rapido';
        this.modalImage = 'assets/images/config/doc_pago_rapido.jpg';
        break;
      case 9:
        this.modalTitle = 'Cantidad Días Documentos Por Vencer';
        this.modalContent = 'Cantidad de días antes de su vencimiento desde los cuales los documentos se consideraran "Por Vencer", tanto en el dhasboard administrador como en el dashboard cliente.';
        this.modalImage = 'assets/images/config/dias_por_vencer.jpg';
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

}
