import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-step1',
  templateUrl: './step1.component.html'
})
export class Step1Component implements OnInit {

  @Input() compras: any;
  @Input() comprasResp: any;
  @Input() tipoDocumentos: any;
  @Output() propagar = new EventEmitter<any>();

  public selectedDosc: any = [];
  public selected: any = [];
  public selectedTipoDcto: any = null;
  public folio: number = 0;
  public dateFilter: NgbDateStruct;

  total: string = "$ 0";
  totalPagar: number = 0;
  totalPagando: number = 0;

  constructor() { }

  ngOnInit(): void {
  }

  onPropagar() {
    this.propagar.emit({
      totalPagar: this.totalPagar,
      documentos: this.selectedDosc
    });
  }

  onSelect(val: any) {    
    if (val.selected) {
      this.selectedDosc = val.selected;
      if (val.selected.length > 0) {
        let valor: number = 0;
        val.selected.forEach(element => {
            valor += element.aPagar
        });
        this.totalPagar = valor;
      } else {
        this.totalPagar = 0;
      }
      this.onPropagar()
    }    
  }

  calcularTotalCuentas() {
    let total: number = 0;
    this.selected.forEach(element => {
      total += parseFloat(element.aPagar);
    });
    this.totalPagar = total;
    this.onPropagar()
  }

  onChangeAPagar(val: any, data: any) {
    // let monto = val.replace(/\./g,'')
    // debugger
    // const newMonto: number = parseInt(monto)
  
    this.compras.forEach(element => {
      if (element.nro === data.nro) {
        if (element.aPagar != parseFloat(val)) {
          element.aPagar = parseFloat(val);
          this.calcularTotalCuentas(); 
        } 
      }
    });
    
    this.selectedDosc.forEach(element => {
      if (element.nro === data.nro) {
        if (element.aPagar != parseFloat(val)) {
          element.aPagar = parseFloat(val);
          this.calcularTotalCuentas(); 
        } 
      }
    }); 

    this.selected.forEach(element => {
      if (element.nro === data.nro) {
        if (element.aPagar != parseFloat(val)) {
          element.aPagar = parseFloat(val);
          this.calcularTotalCuentas(); 
        } 
      }
    });    
    
  }

  filter() {
    let data: any = Object.assign([], this.comprasResp);
    
    if (this.selectedTipoDcto != null) {
      const tp = this.tipoDocumentos.find(x => x.id === this.selectedTipoDcto)
      data = data.filter(x => x.documento == tp.nombre)
    }
    
    if (this.folio != null && this.folio != 0) {
        data = data.filter(x => x.nro == this.folio)
    }
    if (this.dateFilter !=  null) {
        const fDesde = new Date(this.dateFilter.year, this.dateFilter.month -1, this.dateFilter.day, 0, 0, 0);
        const fHasta = new Date(this.dateFilter.year, this.dateFilter.month -1, this.dateFilter.day, 23, 59, 59);
        data = data.filter(x => new Date(x.fvencimiento) >= fDesde && new Date(x.fvencimiento) <= fHasta);
    }

    this.compras = data;
  }

  cleanFilters() {
    this.dateFilter = null;
    this.folio = null;
    this.selectedTipoDcto = null;
    this.compras = Object.assign([], this.comprasResp);
  }

}
