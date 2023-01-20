import { Component, OnInit, ViewChild } from '@angular/core';
import { TiposDocumentoService } from '../../../shared/services/tipodocumento.service';
import { CondicionVentaService } from '../../../shared/services/condicionventa.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { ConfiguracionCobranzaService } from '../../../shared/services/configuracioncobranzas.service';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgSelectComponent, NgOption } from '@ng-select/ng-select';
import { ConfiguracionCobranzas, ConfiguracionCargos, ListaClientesExcluidos } from '../../../shared/models/configuracioncobranza.model';
import { NgxSpinnerService } from "ngx-spinner";

interface AutoCompleteModel {
  value: any;
  display: string;
}

@Component({
  selector: 'app-cobranza-config',
  templateUrl: './cobranza-config.component.html',
  styleUrls: ['./cobranza-config.component.scss']
})
export class CobranzaConfigComponent implements OnInit {

  @ViewChild('myFormCli') formCliente;
  @ViewChild('myFormCar') formCargos; 

  checkAllTipoDocs: boolean = false;
  checkAllCondVnta: boolean = false;
  checkAllClientes: boolean = false;
  
  tipoDocs: any = [];
  condVentas: any = [];
  clientes: any = [];
  cargos: any = [];
  diasCobro: any = [];

  selectedDosc: any = [];
  selectedCond: any = [];
  selectedClie: any = [];

  selectedTipoDcto: any = null;
  selectedCondVenta: any = null;
  selectedClientes: any = null;
  selectedDiasCobro: any = null;
  selectedCargos: any = null;
  clientesSelected: any = [];
  cargosSelected: any = [];
  public tiposDocs: AutoCompleteModel[] = [];

  enviaNotifContactosSinCargo: boolean = false;
  enviaNotifClientesSinContactos: boolean = false;
  enviaCobranza: boolean = false;
  enviaPreCobranza: boolean = false;

  cantDiasVcto: number = 1;
  cantDiasPreVcto: number = 1;
  btnLoading: boolean = false;
  configCobranza: ConfiguracionCobranzas = new ConfiguracionCobranzas();
  configAll: any = null;

  constructor(
    private tiposDocumentoService: TiposDocumentoService, 
    private condicionVentaService: CondicionVentaService,
    private clientesService: ClientesService,
    private notificationService: NotificationService,
    private configuracionCobranzaService: ConfiguracionCobranzaService,
    private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.getConfigCobranza();    
    this.diasCobro = [
      { id: 1, nombre: 'Lunes' },
      { id: 2, nombre: 'Martes' },
      { id: 3, nombre: 'Miercoles' },
      { id: 4, nombre: 'Jueves' },
      { id: 5, nombre: 'Viernes' },
      { id: 6, nombre: 'Sabado' },
      { id: 7, nombre: 'Domingo' }
    ];
  }

  onSelTipoDocs(val: any, c: any) {
    const added = this.selectedDosc.find(x => x.idTipoDocumento == c.idTipoDocumento);
    if (added != null) {
      //remueve
      for (let i=0; i <= this.selectedDosc.length -1; i++) {
        if (this.selectedDosc[i].nro == c.nro) {
          this.selectedDosc.splice(i, 1);
          break;
        }
      }
    } else {
      this.selectedDosc.push(c);
    }

    if (this.selectedDosc.length == 0) {
      this.checkAllTipoDocs = false;
    }
  }

  onSelAllTipoDocs(val: any) {   
    this.selectedDosc = [];

    this.tipoDocs.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.selectedDosc.push(element);    
      }
    });

  }

  onSelCondVenta(val: any, c: any) {
    const added = this.selectedCond.find(x => x.idCondicionDeVenta == c.idCondicionDeVenta);
    if (added != null) {
      //remueve
      for (let i=0; i <= this.selectedCond.length -1; i++) {
        if (this.selectedCond[i].nro == c.nro) {
          this.selectedCond.splice(i, 1);
          break;
        }
      }
    } else {
      this.selectedCond.push(c);
    }

    if (this.selectedCond.length == 0) {
      this.checkAllCondVnta = false;
    }
  }

  onSelAllCondVenta(val: any) {   
    this.selectedCond = [];

    this.condVentas.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.selectedCond.push(element);    
      }
    });

  }

  onSelCientes(val: any, c: any) {
    const added = this.selectedClie.find(x => x.idCliente == c.idCliente);
    if (added != null) {
      //remueve
      for (let i=0; i <= this.selectedClie.length -1; i++) {
        if (this.selectedClie[i].nro == c.nro) {
          this.selectedClie.splice(i, 1);
          break;
        }
      }
    } else {
      this.selectedClie.push(c);
    }

    if (this.selectedClie.length == 0) {
      this.checkAllClientes = false;
    }
  }

  onSelAllClientes(val: any) {   
    this.selectedClie = [];

    this.clientes.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.selectedClie.push(element);    
      }
    });

  }

  getTipoDocs () {
    this.tiposDocumentoService.getAll().subscribe(res => {
      this.tipoDocs = res;

      let tds = []
      const tipos = this.configCobranza.tipoDocumentoCobranza ? this.configCobranza.tipoDocumentoCobranza.split(';') : [];

      tipos.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          tds.push(parseInt(element))
        }        
      });

      this.selectedTipoDcto = tds.length > 0 ? tds : null;  

      this.getCondVta();
    }, err => { this.spinner.hide(); });
  }

  getCondVta () {
    this.condicionVentaService.getAll().subscribe(res => {
      this.condVentas = res;

      let condVtas = []
      const cond = this.configCobranza.condicionesCredito ? this.configCobranza.condicionesCredito.split(';') : [];

      cond.forEach(element => {
        if (element && element.trim().length > 0 && element.trim() !== ';') {
          condVtas.push(parseInt(element))
        }        
      });

      this.selectedCondVenta = condVtas.length > 0 ? condVtas : null; 

      this.getClientes();
    }, err => { this.spinner.hide(); });
  }

  getClientes () {
    this.clientesService.getClientes().subscribe(res => {
      this.clientes = res;

      this.configAll.listaClientesExcluidos.forEach(element => {
        let cliente = res.find(x => x.rut == element.rut)
        if (cliente) {
          this.clientesSelected.push(cliente)
        }        
      });

      this.getCargos();

    }, err => { this.spinner.hide(); });
  }

  getCargos () {
    this.clientesService.getCargos().subscribe(res => {
      this.cargos = res;

      const cargos = this.configAll.configuracionCargos.filter(x => x.tipoConfiguracion === 1);

      cargos.forEach(element => {
        let cargo = res.find(x => x.codCargoSoftland == element.codErp)
        if (cargo) {
          this.cargosSelected.push(cargo)
        }        
      });

      this.spinner.hide();
      
    }, err => { this.spinner.hide(); });
  }

  onSelectCliente(val) {
    if (val) {
      const cliente = this.clientesSelected.find(x => x.idCliente == val.idCliente)
      if (cliente == null) {
        this.clientesSelected.push(val)
      }
      this.selectedClientes = null;
      this.formCliente.resetForm();
    }    
  }

  onSelectCargos(val) {
    const cargo = this.cargosSelected.find(x => x.idCargo == val.idCargo)
    if (cargo == null) {
      this.cargosSelected.push(val)
    }
    this.selectedCargos = null;
    this.formCargos.resetForm();
  }

  deleteCliente(item: any) {
    for (let i: number = 0; i <= this.clientesSelected.length -1; i++) {
      if (item.idCliente == this.clientesSelected[i].idCliente) {
        this.clientesSelected.splice(i, 1);
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

  getConfigCobranza() {
    this.spinner.show();
    this.configuracionCobranzaService.getConfigCObranzas().subscribe((res: any) => {
      if (res) {
        this.configAll = res;
        this.configCobranza = res.configracionCobranzas.length > 0 ? res.configracionCobranzas[0] : new ConfiguracionCobranzas();
        this.enviaCobranza = this.configCobranza.enviaCobranza ? true : false;
        this.enviaPreCobranza = this.configCobranza.enviaPreCobranza ? true : false;
        this.cantDiasVcto = this.configCobranza.enviaCobranza ? this.configCobranza.cantidadDiasVencimiento : 1;
        this.cantDiasPreVcto = this.configCobranza.enviaPreCobranza ? this.configCobranza.cantidadDiasPrevios : 1;
      } else {
        this.configCobranza = new ConfiguracionCobranzas();
      }
      this.getTipoDocs();
    }, err => {
      this.spinner.hide();
      this.notificationService.error('Ocurrio un error al obtener la configuración.', '', true);
    });
  }

  save() {
    this.btnLoading = true;

    let tiposDocs = this.selectedTipoDcto ? this.selectedTipoDcto.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    let condVenta = this.selectedCondVenta ? this.selectedCondVenta.reduce((accumulator, item) => {
      return `${accumulator};${item}`;
    }) : null;

    const data: ConfiguracionCobranzas = {
      idConfigCobranza: this.configCobranza.idConfigCobranza,
      tipoDocumentoCobranza: tiposDocs,
      condicionesCredito: condVenta,
      enviaCobranza: this.enviaCobranza ? 1 : 0,
      cantidadDiasVencimiento: this.cantDiasVcto,
      idFrecuenciaEnvioCob: null,
      enviaPreCobranza: this.enviaPreCobranza ? 1 : 0,
      cantidadDiasPrevios: this.cantDiasPreVcto,
      idFrecuenciaEnvioPre: null
    };

    if (this.configCobranza.idConfigCobranza === 0) {
      this.configuracionCobranzaService.save(data).subscribe((res: ConfiguracionCobranzas) => {
        
        if ((this.cargosSelected && this.cargosSelected.length > 0) || (this.clientesSelected && this.clientesSelected.length > 0)) {
          this.saveConfigCargos(res.idConfigCobranza);
        } else {
          this.configCobranza.idConfigCobranza = res.idConfigCobranza;
          this.notificationService.success('Correcto', '', true);
          this.btnLoading = false;
        }
        
      }, err => {
        this.btnLoading = false;
      });
    } else {
      this.configuracionCobranzaService.edit(data).subscribe(res => {
        
        if ((this.cargosSelected && this.cargosSelected.length > 0) || (this.clientesSelected && this.clientesSelected.length > 0)) {
          this.saveConfigCargos(this.configCobranza.idConfigCobranza);
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
          tipoConfiguracion: 1
        });
      });

      this.configuracionCobranzaService.saveConfigCargos(configuracionCargos, 1).subscribe(res => {

        if (this.clientesSelected && this.clientesSelected.length > 0) {
          this.saveClientes(id);
        } else {
          this.notificationService.success('Correcto', '', true);
          this.btnLoading = false;
        }

      }, err => {
        this.btnLoading = false;
      });

    } else {
      this.saveClientes(id);
    }    
  }

  saveClientes(id: number) {
    if (this.clientesSelected && this.clientesSelected.length > 0) {

      let configuracionClientes: ListaClientesExcluidos[] = [];
      
      this.clientesSelected.forEach(element => {
        configuracionClientes.push({
          idListaClientes: 0,
          idConfiguracion: id,
          rut: element.rut,
          razonSocial: element.nombre
        });
      });

      this.configuracionCobranzaService.saveConfigClientes(configuracionClientes).subscribe(res => {
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
