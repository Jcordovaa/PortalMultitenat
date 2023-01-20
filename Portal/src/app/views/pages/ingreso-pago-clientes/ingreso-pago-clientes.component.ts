import { Component, OnInit } from '@angular/core';
import { Utils } from '../../../shared/utils';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ClientesService } from '../../../shared/services/clientes.service';
import { WizardData } from '../../../shared/models/wizarddata.model';
import { PagoCobranza, PagosCabecera, PagosDetalle, PagosDocumentos } from '../../../shared/models/pagos.model';
import { PagosCobranzaService } from '../../../shared/services/pagocobranza.service';
import { AuthService } from '../../../shared/services/auth.service';

@Component({
  selector: 'app-ingreso-pago-clientes',
  templateUrl: './ingreso-pago-clientes.component.html',
  styleUrls: ['./ingreso-pago-clientes.component.scss']
})
export class IngresoPagoClientesComponent implements OnInit {

  public wzData: WizardData;
  public compras: any = [];
  public tipoDocumentos: any = [];
  public totalPagar: number = 0;
  public totalPagando: number = 0;
  public rut: string = '';
  public razonSocial: string = '';
  public showDetail: boolean = false;
  public paso: number = 0;
  public pagos: any = [];
  public bloqueado: boolean = false;
  public idUsuario: number = 0;
  public sfTipoDocs: any = [];

  constructor(
    private spinner: NgxSpinnerService,
    private notificationService: NotificationService,
    private utils: Utils,
    private clientesService: ClientesService,
    private pagosCobranzaService: PagosCobranzaService,
    private authService: AuthService
  ) { }

  ngOnInit() {
  }

  onStep1Next(e) {
    this.paso = 1;
  }
  
  onStep2Next(e) {
    this.paso = 2;
  }

  onStep3Next(e) {
    this.paso = 3;
  }

  onComplete(e) {
    window.location.reload();
  }

  search() {
    if (this.utils.isValidRUT(this.rut)) {   
      
      this.rut = this.utils.estandarizaRut(this.rut)

      this.spinner.show();
      this.showDetail = false;
  
      const rut: string[] = this.rut.split('-');
      const rut2: string = rut[0].replace('.', '').replace('.', '')
      const model = { nombre: rut2 }

      this.clientesService.getClienteComprasFromSoftland(model).subscribe((res: any) => {
        this.compras = res;

        this.wzData = {
          rut: this.rut,
          razonSocial: this.razonSocial,
          documentos: [],
          totalPagando: 0,
          totalPagar: 0
        };

        let tipoDocs = [];
  
        res.forEach((element, index) => {
          const doc = tipoDocs.find(x => x.nombre === element.documento);
          if (!doc) {
            tipoDocs.push({
              id: index + 1,
              nombre: element.documento
            })
          }        
        });
  
        this.tipoDocumentos = Object.assign([], tipoDocs);  
  
        if (this.compras.length > 0) {
          this.showDetail = true;
          const model: any = { Email: '', Rut: this.rut };

          this.clientesService.getClientFromSoftland(model).subscribe((res: any) => {
            if (res && res.length > 0) {
              this.bloqueado = res[0].bloqueado == "S" ? true : false;
              this.razonSocial = res[0].nombre;
              this.wzData.razonSocial = res[0].nombre;
            }
          }, err => {});
          
        } else {
          this.showDetail = false;
          this.razonSocial = '';
          this.notificationService.info('No se encontró información para el rut ingresado.', '', true);
        }
  
        //this.limpiarFiltros();
        this.spinner.hide();
      }, err => { this.spinner.hide(); });

    } else {
      this.notificationService.warning('Rut ingresado no es válido', '', true);
      this.showDetail = false;
    }
  }

  procesaPropagar(event: any) {
    this.pagos = event.pagos;
    this.wzData.totalPagando = parseInt(event.totalPagando);
    this.totalPagando = parseInt(event.totalPagando);
    this.sfTipoDocs = event.tiposDocs;
  }

  procesaPropagarDocs(event: any) {
    this.wzData.documentos = event.documentos;
    this.wzData.totalPagar = event.totalPagar;
    this.totalPagar = event.totalPagar;
  }

  async onStepChangedl(e, s, s2, s3) {    
    if (this.paso === 3) {
      const response = await this.notificationService.confirmation('Ingresar pagos','¿Confirma generar los pagos para los documentos ingresados?');
      if (response.isConfirmed) {
        this.paso = 3;
        e.goToStep(e.wizardSteps._results[3]);

        document.getElementById('btnAtras').style.display = 'none'; //inline-block
        //document.getElementById('btnTerminar').style.display = 'none';

        this.save();
      } else {
        this.paso = 2;
        e.goToStep(s3);
        return;
      }
    }    
  }

  save() {
    this.spinner.show();

    const cabecera: PagosCabecera = {
      idPago: 0,
      rutCliente: this.rut,
      fechaPago: new Date(),
      horaPago: `${new Date().getHours()}:${new Date().getMinutes()}`,
      montoPago: this.totalPagar,
      comprobanteContable: '',
      fechaEnvioCobranza: new Date(),
      horaEnvioCobranza: `${new Date().getHours()}:${new Date().getMinutes()}`,
      idEstadoEnvioCobranza: 1
    };

    let detalles: PagosDetalle[] = [];
    let documentos: PagosDetalle[] = [];

    this.pagos.forEach(element => {
  
      let detalle: PagosDetalle = {
        idPagoDetalle: 0,
        idPago: 0,
        idTipoPago: element.idTipoPago,
        montoPago: element.montoPago,
        idBanco: (!element.idBanco || element.idBanco === 0) ? null : element.idBanco,
        serie: (!element.serie || element.serie === '') ? null : element.serie,
        fechaEmisionDocumento: new Date(),
        fechaVencimientoDocumento: new Date(),
        cantidadDocumentos: (!element.cantidad || element.cantidad === 0) ? null : element.cantidad,
        nroComprobante: (!element.comprobante || element.comprobante === '') ? null : element.comprobante,
        cuentaContable: element.cuentaContable,
        tipoDoc: element.tipoDocumento,
        fechaTransaccion: new Date(),
        horaTransaccion: `${new Date().getHours()}:${new Date().getMinutes()}`,
        idUsuario: null//this.idUsuario,
      };

      detalles.push(detalle);
    });

    this.wzData.documentos.forEach(element => {
      let tipDoc = this.tipoDocumentos.find(x => x.nombre == element.documento)

      let documento: PagosDocumentos = {
        idPagoDocumento: 0,
        idPago: 0,
        idTipoDocumento: tipDoc ? tipDoc.id : 0,
        folioDocumento: element.nro,
        tipoDocumento: element.movtipdocref,
        estadoPago: 1,
        fechaEnvioCobranza: new Date(),
        horaEnvioCobranza: `${new Date().getHours()}:${new Date().getMinutes()}`,
        correosEnvio: ''
      };

      documentos.push(documento);
    });    

    const data: PagoCobranza = {
      pagosCabecera: cabecera,
      pagosDetalle: detalles,
      pagosDocumentos: documentos
    };

    this.pagosCobranzaService.save(data).subscribe(res => {
      this.spinner.hide();
      this.notificationService.success('Correcto', '', true);
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar.', '', true); });
  }

  async desbloquearCliente() {
    const response = await this.notificationService.confirmation('Desbloquear cliente', `¿Confirma desbloquear al cliente ${this.razonSocial}?`);
    if (response.isConfirmed) {
      this.spinner.show();

      const rut: string[] = this.rut.split('-');
      const rutCodAux: string = rut[0].replace('.', '').replace('.', '')

      this.clientesService.desbloquearClienteSoftland({ codAux: rutCodAux }).subscribe(res => {
        this.spinner.hide();
        this.bloqueado = false;
        this.notificationService.success('Correcto', '', true);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al desbloquear el cliente.', '', true); });
    }    
  }

}
