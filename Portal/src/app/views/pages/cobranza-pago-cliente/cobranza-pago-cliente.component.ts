import { Component, OnInit, ViewChild } from '@angular/core';
import { TiposDocumentoService } from '../../../shared/services/tipodocumento.service';
import { CondicionVentaService } from '../../../shared/services/condicionventa.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { ConfiguracionSoftlandService } from '../../../shared/services/configuracionsoftland.service';
import { ConfiguracionPagoClientesService } from '../../../shared/services/configuracionpagoclientes.service';
import { ConfiguracionPagoCliente, ConfiguracionCargos, ConfiguracionTiposDocumentos } from '../../../shared/models/configuracioncobranza.model';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-cobranza-pago-cliente',
  templateUrl: './cobranza-pago-cliente.component.html',
  styleUrls: ['./cobranza-pago-cliente.component.scss']
})
export class CobranzaPagoClienteComponent implements OnInit {

  @ViewChild('myFormDctos') formDctos;
  @ViewChild('myFormCar') formCargos; 
 
  tipoDocs: any = [];
  cuentasContables: any = [];
  cuentasContablesCP: any = [];
  cargos: any = [];
  clientes: any = [];
  condVentas: any = [];

  selectedDosc: any = [];
  selectedCuen: any = [];
  selectedCuenContrapartida: any = [];
  selectedCarg: any = [];

  enviaNotifContactosSinCargo: boolean = false;
  enviaNotifClientesSinContactos: boolean = false;

  tiposDctosSelected: any = [];
  cargosSelected: any = [];
  btnLoading: boolean = false;
  selectedCargos: any = null;
  configPagoClientes: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
  configAll: any = null;

  constructor(
    private tiposDocumentoService: TiposDocumentoService, 
    private condicionVentaService: CondicionVentaService,
    private clientesService: ClientesService,
    private configuracionSoftlandService: ConfiguracionSoftlandService,
    private configuracionPagoClientesService: ConfiguracionPagoClientesService,
    private notificationService: NotificationService,
    private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.getConfigPagoClientes();    
  }

  getTipoDocs () {
    this.configuracionSoftlandService.getAllTipoDocSoftland().subscribe((res: []) => {
      this.tipoDocs = res;

      this.configAll.configuracionTiposDocumentos.forEach(element => {
        let tDoc = res.find((x: any) => x.codDoc == element.codErp)
        if (tDoc) {
          this.tiposDctosSelected.push(tDoc)
        }        
      });

      this.getCuentasContables();
    }, err => { this.spinner.hide(); });
  }

  getCuentasContables () {
    this.configuracionSoftlandService.getAllCuentasContablesSoftland().subscribe(res => {
      this.cuentasContables = res;
      this.cuentasContablesCP = Object.assign([], res) //{ ...this.cuentasContables }

      let ccs = []
      const cuentasContables = this.configPagoClientes.cuentasContablesDeuda ? this.configPagoClientes.cuentasContablesDeuda.split(';') : [];

      cuentasContables.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          ccs.push(element)
        }        
      });

      this.selectedCuen = ccs.length > 0 ? ccs : null;  

      // let ccsCp = []
      // const cuentasContablesCP = this.configPagoClientes.cuentaContablePago ? this.configPagoClientes.cuentaContablePago.split(';') : [];

      // cuentasContablesCP.forEach(element => {
      //   if (element && element.trim().length > 0 && element.trim() !== ';') {
      //     ccsCp.push(element)
      //   }        
      // });
 
      // this.selectedCuenContrapartida = ccsCp.length > 0 ? ccsCp : null;

      this.getCargos();
    }, err => { this.spinner.hide(); });
  }

  getCargos () {
    this.clientesService.getCargos().subscribe(res => {
      this.cargos = res;
      
      this.configAll.configuracionCargos.forEach(element => {
        let cargo = res.find((x: any) => x.codCargoSoftland == element.codErp)
        if (cargo) {
          this.cargosSelected.push(cargo)
        }        
      });

      this.spinner.hide();

    }, err => { this.spinner.hide(); });
  }

  onSelectTipoDoc(val: any) {
    const td = this.tiposDctosSelected.find(x => x.codDoc == val.codDoc)
    if (td == null) {
      this.tiposDctosSelected.push(val)
    }
    this.selectedDosc = null;
    this.formDctos.resetForm();
  }

  onSelectCargos(val: any) {
    const cargo = this.cargosSelected.find(x => x.idCargo == val.idCargo)
    if (cargo == null) {
      this.cargosSelected.push(val)
    }
    this.selectedCargos = null;
    this.formCargos.resetForm();
  }

  deleteTipoDoc(item: any) {
    for (let i: number = 0; i <= this.tiposDctosSelected.length -1; i++) {
      if (item.codDoc == this.tiposDctosSelected[i].codDoc) {
        this.tiposDctosSelected.splice(i, 1);
        break;
      }
    }
  }

  deleteCargo(item: any) {
    for (let i: number = 0; i <= this.cargosSelected.length -1; i++) {
      if (item.idCargo == this.cargosSelected[i].idCargo) {
        this.cargosSelected.splice(i, 1);
        break;
      }
    }
  }

  getConfigPagoClientes() {
    this.spinner.show();

    this.configuracionPagoClientesService.getConfigPagoClientes().subscribe((res: any) => {
      if (res) {
        this.configAll = res;
        this.configPagoClientes = res.configuracionPagoCliente.length > 0 ? res.configuracionPagoCliente[0] : new ConfiguracionPagoCliente();
        this.enviaNotifClientesSinContactos = res.configuracionPagoCliente.length > 0 ? (res.configuracionPagoCliente[0].enviaNotificacionSinContacto == 1 ? true : false) : false;
        this.enviaNotifContactosSinCargo = res.configuracionPagoCliente.length > 0 ? (res.configuracionPagoCliente[0].enviaNotificacionSinCargo == 1 ? true: false) : false;
      } else {
        this.configPagoClientes = new ConfiguracionPagoCliente();
      }
      this.getTipoDocs();
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrió un error al obtener la configuración.', '', true);
    });
  }

  save() {
    this.btnLoading = true;

    let cuentasContables = this.selectedCuen ? this.selectedCuen.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let cuentasContablesContra = this.selectedCuenContrapartida ? this.selectedCuenContrapartida.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    const data: ConfiguracionPagoCliente = {
      idConfiguracionPago: this.configPagoClientes.idConfiguracionPago,
      cuentasContablesDeuda: cuentasContables,
      // cuentaContablePago: cuentasContablesContra,
      // enviaNotificacionSinCargo: this.enviaNotifContactosSinCargo ? 1 : 0,
      // enviaNotificacionSinContacto: this.enviaNotifClientesSinContactos ? 1 : 0,
      // rutaCorreo: ''
    };
    
    if (this.configPagoClientes.idConfiguracionPago === 0) {
      this.configuracionPagoClientesService.save(data).subscribe((res: ConfiguracionPagoCliente) => {
        
        if ((this.cargosSelected && this.cargosSelected.length > 0) || (this.tiposDctosSelected && this.tiposDctosSelected.length > 0)) {
          this.saveConfigCargos(res.idConfiguracionPago);
        } else {
          this.configPagoClientes.idConfiguracionPago = res.idConfiguracionPago;
          this.notificationService.success('Correcto', '', true);
          this.btnLoading = false;
        }
        
      }, err => {
        this.btnLoading = false;
      });
    } else {
      this.configuracionPagoClientesService.edit(data).subscribe(res => {
        
        if ((this.cargosSelected && this.cargosSelected.length > 0) || (this.tiposDctosSelected && this.tiposDctosSelected.length > 0)) {
          this.saveConfigCargos(this.configPagoClientes.idConfiguracionPago);
        } else {
          this.notificationService.success('Correcto', '', true);
          this.btnLoading = false;
        }

      }, err => {
        this.btnLoading = false;
      });
    }

  }

  saveConfigCargos(id: number) {
    if (this.cargosSelected && this.cargosSelected.length > 0) {

      let configuracionCargos: ConfiguracionCargos[] = [];

      this.cargosSelected.forEach(element => {
        configuracionCargos.push({
          idCargosConfig: 0,
          idConfiguracion: id,
          nombre: element.nombre,
          codErp: element.codCargoSoftland,
          tipoConfiguracion: 2
        });
      });

      // this.configuracionPagoClientesService.saveConfigCargos(configuracionCargos, 2).subscribe(res => {

      //   if (this.tiposDctosSelected && this.tiposDctosSelected.length > 0) {
      //     this.saveTipoDctos(id);
      //   } else {
      //     this.notificationService.success('Correcto', '', true);
      //     this.btnLoading = false;
      //   }

      // }, err => {
      //   this.btnLoading = false;
      // });

    } else {
      this.saveTipoDctos(id);
    }    
  }

  saveTipoDctos(id: number) {
    if (this.tiposDctosSelected && this.tiposDctosSelected.length > 0) {

      let configuracionTiposDctos: ConfiguracionTiposDocumentos[] = [];
      
      this.tiposDctosSelected.forEach(element => {
        configuracionTiposDctos.push({
          idTipoDocConfig: 0,
          idConfiguracion: id,
          nombre: element.desDoc,
          codErp: element.codDoc
        });
      });

      this.configuracionPagoClientesService.saveConfigTiposDocs(configuracionTiposDctos).subscribe(res => {
        this.notificationService.success('Correcto', '', true);
        this.btnLoading = false;        
      }, err => {
        this.btnLoading = false;
      });

    } else {
      this.notificationService.success('Correcto', '', true);
      this.btnLoading = false;
    }  
  }

}
