import { Component, OnInit, Input, Output, EventEmitter  } from '@angular/core';
import { TipoPagoService } from '../../../../../shared/services/tipopago.service';
import { Pagos } from '../../../../../shared/models/pagos.model';
import { TipoPago } from '../../../../../shared/models/tipopago.model';
import { Bancos } from '../../../../../shared/models/bancos.model';
import { NgxSpinnerService } from "ngx-spinner";
import { NotificationService } from '../../../../../shared/services/notificacion.service';
import { BancosService } from '../../../../../shared/services/bancos.service';
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker, NgbDate } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-step2',
  templateUrl: './step2.component.html'
})
export class Step2Component implements OnInit {

  @Input() totalPagar: number;
  @Output() propagar = new EventEmitter<any>();

  public selectedTipoPago: any = null;
  public tipoPagos: TipoPago[] = [];
  public tipoPagosCobranza: TipoPago[] = [];
  public pagos: any = [];
  public totalPagando: number = 0;
  public nuevoPago: Pagos = new Pagos();
  public selectedBanco: any = null;
  public bancos: Bancos[] = [];
  public fechaDocumento: NgbDateStruct;

  public muestraComprobante: number = 0;
  public muestraCantidad: number = 0;
  public muestraSerie: number = 0;
  public muestraFecha: number = 0;
  public muestraMonto: number = 0;
  public muestraBanco: number = 0;

  constructor(
    private tipoPagoService: TipoPagoService,
    private spinner: NgxSpinnerService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private bancosService: BancosService) { }

  ngOnInit(): void {
    this.spinner.show();

    this.tipoPagoService.getAll().subscribe((res: TipoPago[]) => {
      this.tipoPagos = Object.assign([], res);
      this.tipoPagosCobranza = res;
      
      this.bancosService.getAll().subscribe((res: Bancos[]) => {
        this.bancos = res;
        this.spinner.hide();
      }, err => { this.spinner.hide(); });

    }, err => { this.spinner.hide(); });    
  }

  onChangeTipoPago(content) {
    if (this.pagos.length == 2) {  
      this.notificationService.info('Solo se permite el ingreso de 2 pagos distintos', '', true); 
      return;  
    }
    
    if (this.selectedTipoPago != null || this.selectedTipoPago != 0) {
      //valida si pago ya fue ingresado
      for (let i = 0; i <= this.pagos.length - 1; i++) {
        if (this.pagos[i].idTipoPago == this.selectedTipoPago) {
          this.notificationService.info('Medio de pago ya ingresado, para modificar debe ser eliminado', '', true);
            return;
        }
      }

      if ((this.totalPagar - this.totalPagando) > 0) {
        this.nuevoPago = new Pagos();
        this.nuevoPago.idTipoPago = this.selectedTipoPago;

        let selectTIPO = this.tipoPagos.filter(x => x.idTipoPago == this.selectedTipoPago);
        let pago = selectTIPO[0];

        this.nuevoPago.nombre = pago.nombre;
        this.nuevoPago.cuentaContable = pago.cuentaContable;
        this.nuevoPago.tipoDocumento = pago.tipoDocumento;
  
        this.muestraMonto = pago.muestraMonto;
        this.muestraBanco = pago.muestraBanco;
        this.muestraSerie = pago.muestraSerie;
        this.muestraFecha = pago.muestraFecha;
        this.muestraComprobante = pago.muestraComprobante;
        this.muestraCantidad = pago.muestraCantidad;
  
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
      } 
      else { this.notificationService.info('Ya ha completado el monto total del pago.', '', true); }
    }   
  }

  eliminarpago(p: any) {
    for (let i = 0; i <= this.pagos.length - 1; i++) {
      if (this.pagos[i].idTipoPago == p.idTipoPago && this.pagos[i].montoPago == p.montoPago 
          && this.pagos[i].serie == p.serie && this.pagos[i].banco == p.banco ) {
        this.pagos.splice(i, 1);
        this.pagos = [...this.pagos]
        this.totalPagando = this.totalPagando - p.montoPago;
        this.onPropagar();
        break;
      }
    }
  }

  agregarPago() {
    //Valida campos
    if (this.nuevoPago.montoPago > (this.totalPagar - this.totalPagando)) {
      this.notificationService.warning('El monto ingresado es mayor al pendiente de pago', '', true);
      this.nuevoPago.montoPago = 0;
      return;
    }

    if (this.nuevoPago.montoPago == 0) {this.notificationService.info('Debe ingresar el monto de pago', '', true); return;}
    if (this.muestraBanco == 1 && this.selectedBanco == 0) {this.notificationService.info('Debe seleccionar el banco', '', true); return;}
    if (this.muestraSerie == 1 && this.nuevoPago.serie == "") {this.notificationService.info('Debe ingresar el numero de serie', '', true); return;}
    if (this.muestraFecha == 1 && this.fechaDocumento  == null) {this.notificationService.info('Debe ingresar la fecha del primer vencimiento', '', true); return;}
    if (this.muestraComprobante == 1 && this.nuevoPago.comprobante  == "") {this.notificationService.info('Debe ingresar el numero de comprobante', '', true); return;}
    if (this.muestraCantidad == 1 && this.nuevoPago.cantidad  == 0) {this.notificationService.info('Debe ingresar la cantidad de documentos', '', true); return;}
    
    if (this.nuevoPago.montoPago > 0) {

      if (this.selectedBanco != 0 && this.selectedBanco != null) {
        var selectBA = this.bancos.filter(x => x.idBanco == this.selectedBanco);
        var banc = selectBA[0];
        this.nuevoPago.codBanco = banc.codErp;
        this.nuevoPago.banco = banc.nombre;
        this.nuevoPago.idBanco = this.selectedBanco;
      }
      
      if (this.fechaDocumento != null) {
        const fecha = new Date(this.fechaDocumento.year, this.fechaDocumento.month -1, this.fechaDocumento.day, 0, 0, 0);
        this.nuevoPago.fecha = fecha.toLocaleDateString();
        this.nuevoPago.fechaFom = fecha;
      }

      this.nuevoPago.folio = 0;

      this.totalPagando = this.totalPagando + this.nuevoPago.montoPago;
      let rowData = { ...this.nuevoPago };
      this.pagos.splice(0, 0, this.nuevoPago);
      this.pagos = [...this.pagos]
      this.modalService.dismissAll();
      this.selectedTipoPago = null;

      this.onPropagar();
    } 
  }

  onPropagar() {
    this.propagar.emit({
      pagos: this.pagos,
      totalPagando: this.totalPagando,
      tiposDocs: this.tipoPagos
    });
    // this.propagar.emit(this.totalPagando);
  }

}
