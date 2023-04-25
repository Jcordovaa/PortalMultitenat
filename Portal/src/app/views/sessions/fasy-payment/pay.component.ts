import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from "../../../shared/services/auth.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { VentasService } from '../../../shared/services/ventas.service';
import { Utils } from '../../../shared/utils';
import { TbkRedirect } from '../../../shared/enums/TbkRedirect';
import { NgxSpinnerService } from "ngx-spinner";
import { MontoPipe } from 'src/app/shared/pipes/monto.pipe';
import { PasarelaPagoService } from 'src/app/shared/services/pasarelaPago.service';
import { ThrowStmt } from '@angular/compiler';
import { ConfiguracionPagoCliente, ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { formatDate } from '@angular/common';
import * as FileSaver from "file-saver";
import * as XLSX from 'xlsx';
import { Cliente } from 'src/app/shared/models/clientes.model';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';
;


const I18N_VALUES = {
  en: {
    weekdays: ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'],
    months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  },
  es: {
    weekdays: ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'],
    months: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
  }
};

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

declare var $: any;
@Component({
  selector: 'app-pay',
  templateUrl: './pay.component.html',
  styleUrls: ['./pay.component.scss']
})
export class PayComponent implements OnInit {


  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();
  @ViewChild('modalClientes') modalClientes: any
  clienteSeleccionado: any = null;
  cliente: any;
  clientes: any[] = [];
  loading: boolean = false;
  compras: any = [];
  comprasResp: any = [];
  dateDesde: NgbDateStruct;
  dateHasta: NgbDateStruct;
  selected: any = [];
  tiposDocs: any = [];
  estados: any = [];
  selectedEstado: any = null;
  selectedTipoDoc: any = null;
  folio: number = null;
  total: string = "$ 0";
  totalPagar: number = 0;
  selectedDosc: any = [];
  showModalPaymentResult: boolean = true;
  paymentResultState: number = 1;
  logPasarela: any = null;
  pasarelas: any = [];
  selectedPasarela: number = 0;
  tipoFecha: number = 1;
  checkAll: boolean = false
  page = 1;
  pageSize = 2;
  collectionSize = 8;
  configuracion: ConfiguracionPortal = new ConfiguracionPortal();
  codAuxCliente: string = '';
  correoPago: string = '';
  rutPago: string = '';
  nombrePago: string = '';
  nombreCliente: string = '';
  rutCliente: string = '';
  configuracionPagos: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
  existDocSegundaMoneda: boolean = false;
  apellidoPago: string = '';
  esPagoRut: boolean = false;
  foliosErrorComprobate: string = '';
  bloqueaBotonPago: boolean = false;
  load: boolean = false;
  fechaActual = new Date();
  valorUfActrual: number = 0;
  idCobranza: number = 0;
  automatizacion: string = null;
  varloUfOrigen: number = 0;
  fechaEmision: any = null;

  constructor(private ngbDatepickerConfig: NgbDatepickerConfig , private disenoSerivce: ConfiguracionDisenoService,
    private clientesService: ClientesService,
    private authService: AuthService,
    private activatedRoute: ActivatedRoute,
    private notificationService: NotificationService,
    private ngbDatepickerI18n: NgbDatepickerI18n,
    private utils: Utils,
    private spinner: NgxSpinnerService,
    private modalService: NgbModal,
    private router: Router,
    private ventasService: VentasService,
    private montoPipe: MontoPipe,
    private pasarelaService: PasarelaPagoService, private configuracionService: ConfiguracionPagoClientesService) {

    this.ngbDatepickerConfig.firstDayOfWeek = 1;

    this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
      return I18N_VALUES['es'].weekdays[weekday - 1];
    };

    this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
      return I18N_VALUES['es'].months[months - 1];
    };

  }

  ngOnInit(): void {
    this.spinner.show();
    this.configuracionService.getConfigPortal().subscribe(res => {
      this.configuracion = res;
      this.configuracionService.getConfigPagoClientes().subscribe(res => {     
        this.configuracionPagos = res;
        this.getConfigDiseno();
        this.activatedRoute.queryParams.subscribe(params => {
          if (params['state'] != null) {
    
            var state = atob(params['state']);
    
            this.paymentResultState = parseInt(state.split(';')[0]);
            this.showModalPaymentResult = true;
    
    
            if (this.paymentResultState == 4) {
              this.foliosErrorComprobate = state.split(';')[2];
            }
    
            this.pasarelaService.getLogPasarela(parseInt(state.split(';')[1])).subscribe(res => {
    
              this.logPasarela = res;
              var btn = document.getElementById('btnModal')
              btn.click();
              this.spinner.hide();
            }, err => {
              this.logPasarela = null;
              var btn = document.getElementById('btnModal')
              btn.click();
              this.spinner.hide();
            });
          }
        });
    
        if (this.activatedRoute.snapshot.params['rut'] != null && this.activatedRoute.snapshot.params['rut'] != '' && this.activatedRoute.snapshot.params['rut'] != '0') {
    
          this.esPagoRut = true;
          this.rutCliente = window.atob(this.activatedRoute.snapshot.params['rut'])
          var codAux: string = '';
      
    
          const data: any = {
            correo: '',
            rut: this.rutCliente,
            codaux: ''
          };
    
          this.clientesService.getClienteByMailAndRut(data).subscribe((res: Cliente) => {
            this.cliente = res;
            codAux = this.cliente.codAux;
            this.nombreCliente = this.cliente.nombre;
            this.getCompras(codAux);
          }, err => { this.spinner.hide(); });
    
        }else{
          this.getCompras(codAux);
        }
    
      
      }, err => { this.spinner.hide(); });
    }, err => { this.spinner.hide(); });

  }

  getCompras(codAux: string){
    let numDoc = 0;
    if (this.activatedRoute.snapshot.params['numDoc'] != null && this.activatedRoute.snapshot.params['numDoc'] != '' && this.activatedRoute.snapshot.params['numDoc'] != '0') {
      numDoc = this.activatedRoute.snapshot.params['numDoc']
      this.esPagoRut = false;
    }

    if (this.activatedRoute.snapshot.params['idCobranza'] != null && this.activatedRoute.snapshot.params['idCobranza'] != '' && this.activatedRoute.snapshot.params['idCobranza'] != '0') {
      this.idCobranza = this.activatedRoute.snapshot.params['idCobranza']
    }

    debugger
    if (this.activatedRoute.snapshot.params['automatizacion'] != null && this.activatedRoute.snapshot.params['automatizacion'] != '' && this.activatedRoute.snapshot.params['automatizacion'] != '0') {
      this.automatizacion = this.activatedRoute.snapshot.params['automatizacion']
    }


    this.activatedRoute.queryParams.subscribe(params => {
      if (params['state'] != null) {
        var state = atob(params['state']);

        let buscaDocumentos = parseInt(state.split(';')[2]);



        if (buscaDocumentos == 0) {
          const model = {rut: this.rutCliente,  codAux: codAux, folio: numDoc, idCobranza : this.idCobranza, automatizacionJson: this.automatizacion }
          this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any) => {


            this.comprasResp = res;
            this.comprasResp.forEach(element => {
              element.bloqueadoPago = false;
              if(element.codMon != this.configuracionPagos.monedaUtilizada && element.equivalenciaMoneda == 0){
                element.bloqueadoPago = true;
              }
              if (this.paymentResultState == 4 && this.foliosErrorComprobate != null && this.foliosErrorComprobate != '') {
                let folios = this.foliosErrorComprobate.split('-');
                folios.forEach(folio => {
                  if (element.nro == folio) {
                    element.bloqueadoPago = true;
                  }
                });
              }
            });
            this.compras = this.comprasResp;
            let docsSegundaMoneda = this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
            if (docsSegundaMoneda.length > 0) {
              this.existDocSegundaMoneda = true;
            }
            this.estados = [];
            this.tiposDocs = [];
            this.estados.push({ id: 0, nombre: 'TODOS' });
            this.tiposDocs.push({ id: 0, nombre: 'TODOS' });
            this.selectedTipoDoc = this.tiposDocs[0];
            this.selectedEstado = this.estados[0];
            res.forEach((element, index) => {

              let auxElement = element;
              if (auxElement.estado == 'V') { auxElement.estado = 'VENCIDO'; }
              if (auxElement.estado == 'P') { auxElement.estado = 'PENDIENTE'; }
              const ex1 = this.estados.find(x => x.nombre == auxElement.estado);
              const ex2 = this.tiposDocs.find(x => x.nombre == auxElement.documento);

              if (ex1 == null) {
                this.estados.push({
                  id: index + 1,
                  nombre: auxElement.estado
                });
              }
              if (ex2 == null) {
                this.tiposDocs.push({
                  id: index + 1,
                  nombre: auxElement.documento
                });
              }
            });
            this.load = true;
            this.spinner.hide();
          }, err => { this.spinner.hide(); });
        }
      } else {
        const model = { rut: this.rutCliente, codAux: codAux, folio: numDoc, idCobranza : this.idCobranza, automatizacionJson: this.automatizacion }
        this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any) => {
          this.comprasResp = res;
          if(this.comprasResp.length == 0){
            this.notificationService.warning('No se pudo encontrar el documento o este ya fue pagado.', '', true);
          }
          if (numDoc != 0) {
            let listaCodigos = [];
            this.comprasResp.forEach((elemento) => {                 
              let exist = listaCodigos.filter(x => x.codAux == elemento.codAux);
              if(exist.length == 0){
                listaCodigos.push({ codAux: elemento.codAux });
              }
            });
            if(listaCodigos.length > 0){
              this.clientesService.getClientesByCodAux(listaCodigos).subscribe(
                (res: any) => {
                  this.clientes = res;
                  this.modalService.open(this.modalClientes, { keyboard:false,backdrop:'static', ariaLabelledBy: 'modal-basic-title'});
                  this.spinner.hide();
                },
                err => { this.spinner.hide(); }
              );
            }else{
              this.spinner.hide();
            }
          
          } else {
            this.comprasResp.forEach(element => {
              element.bloqueadoPago = false;
              if(element.codMon != this.configuracionPagos.monedaUtilizada && element.equivalenciaMoneda == 0){
                element.bloqueadoPago = true;
              }
              if (this.paymentResultState == 4 && this.foliosErrorComprobate != null && this.foliosErrorComprobate != '') {
                let folios = this.foliosErrorComprobate.split('-');
                folios.forEach(folio => {
                  if (element.nro == folio) {
                    element.bloqueadoPago = true;
                  }
                });
              }
            });

            this.compras = this.comprasResp;
            let docsSegundaMoneda = this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
            if (docsSegundaMoneda.length > 0) {
              this.existDocSegundaMoneda = true;
            }
            this.estados = [];
            this.tiposDocs = [];
            this.estados.push({ id: 0, nombre: 'TODOS' });
            this.tiposDocs.push({ id: 0, nombre: 'TODOS' });
            this.selectedTipoDoc = this.tiposDocs[0];
            this.selectedEstado = this.estados[0];
            res.forEach((element, index) => {

              let auxElement = element;
              if (auxElement.estado == 'V') { auxElement.estado = 'VENCIDO'; }
              if (auxElement.estado == 'P') { auxElement.estado = 'PENDIENTE'; }
              const ex1 = this.estados.find(x => x.nombre == auxElement.estado);
              const ex2 = this.tiposDocs.find(x => x.nombre == auxElement.documento);

              if (ex1 == null) {
                this.estados.push({
                  id: index + 1,
                  nombre: auxElement.estado
                });
              }
              if (ex2 == null) {
                this.tiposDocs.push({
                  id: index + 1,
                  nombre: auxElement.documento
                });
              }
            });
            this.load = true;
            this.spinner.hide();
          }

        }, err => { this.spinner.hide(); });
      }
    });
  }

  filter() {
    let data: any = Object.assign([], this.comprasResp);

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.selectedEstado.id != 0) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.selectedTipoDoc.id != 0) {
      data = data.filter(x => x.documento == this.selectedTipoDoc.nombre)
    }
    if (this.tipoFecha != 0) {
      if (this.tipoFecha == 1) {
        if (this.dateDesde != null) {
          const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
          data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
        }
        if (this.dateHasta != null) {
          const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
          data = data.filter(x => new Date(x.fechaEmision) <= fHasta)
        }
      }

      if (this.tipoFecha == 2) {
        if (this.dateDesde != null) {
          const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
          data = data.filter(x => new Date(x.fechaVcto) >= fDesde)
        }
        if (this.dateHasta != null) {
          const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
          data = data.filter(x => new Date(x.fechaVcto) <= fHasta)
        }
      }
    }

    this.compras = data;
  }



  export() {
    let data: any = Object.assign([], this.comprasResp);

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.selectedEstado.id != 0) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.selectedTipoDoc.id != 0) {
      data = data.filter(x => x.documento == this.selectedTipoDoc.nombre)
    }
    if (this.tipoFecha != 0) {
      if (this.tipoFecha == 1) {
        if (this.dateDesde != null) {
          const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
          data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
        }
        if (this.dateHasta != null) {
          const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
          data = data.filter(x => new Date(x.fechaEmision) <= fHasta)
        }
      }

      if (this.tipoFecha == 2) {
        if (this.dateDesde != null) {
          const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
          data = data.filter(x => new Date(x.fechaVcto) >= fDesde)
        }
        if (this.dateHasta != null) {
          const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
          data = data.filter(x => new Date(x.fechaVcto) <= fHasta)
        }
      }
    }

    var listaExportacion = [];
    data.forEach((element, index) => {
      var fem = formatDate(element.fechaEmision, 'yyyy-MM-dd', 'en-US');
      var fve = formatDate(element.fechaVcto, 'yyyy-MM-dd', 'en-US');
      const model = { Tipo: element.documento, Folio: element.nro, Emision: fem, Vencimiento: fve, Monto: element.debe, Saldo: element.saldo, Estado: element.estado };
      listaExportacion.push(model);
    });

    this.exportAsExcelFile(listaExportacion, "Estado de cuenta");
  }

  public exportAsExcelFile(rows: any[], excelFileName: string): void {
    if (rows.length > 0) {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
      const workbook: XLSX.WorkBook = { Sheets: { 'Estado de cuenta': worksheet }, SheetNames: ['Estado de cuenta'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    } else {
      this.notificationService.warning('Aucune ligne à exporter...', '', true);
    }
  }

  private saveAsExcelFile(buffer: any, baseFileName: string): void {
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    FileSaver.saveAs(data, baseFileName + EXCEL_EXTENSION);
  }


  SeleccionarPago(content) {

    this.pasarelaService.getPasarelasPago().subscribe(
      res => {
        
        if (res.length > 0) {
          this.pasarelas = res;
        }
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
      },
      err => { this.spinner.hide(); this.notificationService.error('Error al obtener pasarelas.', '', true); }
    );



  }


  validaRut() {
    if (this.utils.isValidRUT(this.rutPago)) {
      this.rutPago = this.utils.checkRut(this.rutPago);
    }
  }

  pagar() {
    this.bloqueaBotonPago = true;
    if (this.selectedPasarela == 0) {
      this.notificationService.warning('Debe seleccionar la forma de pago.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }
    if (this.correoPago == '') {
      this.notificationService.warning('Debe ingresar un correo.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }

    if (this.rutPago == '') {
      this.notificationService.warning('Debe ingresar un rut.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }

    if (this.nombrePago == '') {
      this.notificationService.warning('Debe ingresar nombre.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }

    const emailRegex = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;
    if (!emailRegex.test(this.correoPago)) {
      this.notificationService.warning('Debe un correo valido.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }



    let detalle = [];


    for (let i = 0; i <= this.selected.length - 1; i++) {
      detalle.push({
        IdPagoDetalle: 0,
        IdPago: 0,
        Folio: this.selected[i].nro,
        TipoDocumento: this.selected[i].tipoDoc,
        CuentaContableDocumento: this.selected[i].cuentaContable,
        FechaEmision: this.selected[i].fechaEmision,
        FechaVencimiento: this.selected[i].fechaVcto,
        Total: this.selected[i].montoBase,
        Saldo: this.selected[i].saldoBase,
        APagar: this.selected[i].aPagar

      });
    }


    var codAux: string = '';
    if (this.esPagoRut) {
      let str = this.rutCliente.split('');
      for (let a of str) {
        if (a != '.') {
          if (a == '-') {
            break;
          }
          codAux = codAux + a;
        }
      }
    } else {
      
      codAux = this.comprasResp[0].codAux;
    }


    let pago = {
      IdPago: 0,
      IdCliente: 0,
      FechaPago: new Date(),
      HoraPago: '',
      MontoPago: this.totalPagar,
      ComprobanteContable: '',
      IdPagoEstado: 1,
      Rut: this.rutPago,
      CodAux: codAux,
      Nombre: this.nombrePago,
      Correo: this.correoPago,
      IdPasarela: this.selectedPasarela,
      EsPagoRapido: 1,
      PagosDetalle: detalle
    }


    var datosPago = window.btoa(encodeURIComponent(this.nombrePago) + ';' + encodeURIComponent(this.apellidoPago) + ';' + this.rutPago + ';' + this.correoPago);
    let rutEncriptado = window.btoa(this.rutCliente);
    let tenant = btoa(window.location.hostname );
    this.spinner.show();

    this.clientesService.getEstadoConexionSoftland().subscribe(
      resVal => {
        if (resVal) {
          this.clientesService.postSavePago(pago).subscribe(
            (res: any) => {
              this.spinner.hide();
              //LLama a procesador de pago que se encargara de levantar la pasarela correspondiente

              this.pasarelaService.generaPagoElectronico(res,this.selectedPasarela,rutEncriptado,0,datosPago,TbkRedirect.Front,tenant).subscribe(
                (res: any) => {
                  this.spinner.hide();
                  //LLama a procesador de pago que se encargara de levantar la pasarela correspondiente
                  if (res.estado == 1)
                  {
                    window.location.href = res.enlacePago;
                  }
                },
                err => { this.spinner.hide(); this.bloqueaBotonPago = false; }
              );

            },
            err => { this.spinner.hide(); this.bloqueaBotonPago = false; }
          );
        } else {
          this.spinner.hide(); this.bloqueaBotonPago = false; this.notificationService.warning('No es posible realizar el pago en estos momentos, favor vuelva a intentar más tarde.', '', true);
        }
      },
      err => { this.spinner.hide(); this.bloqueaBotonPago = false; this.notificationService.error('Error al verificar conexión.', '', true); }
    );

  }

  openModal(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  closeModal() {
    if (this.esPagoRut) {
      var rutEncriptado = window.btoa(this.rutCliente);
      this.modalService.dismissAll();
      this.bloqueaBotonPago = false;
      let aut = (this.automatizacion == null || this.automatizacion == '') ? '0' : this.automatizacion;
      window.location.href = window.location.origin + '#/sessions/pay/' + rutEncriptado + '/0/' + this.idCobranza + '/' + aut
    } else {
      this.modalService.dismissAll();
      this.activatedRoute.queryParams.subscribe(params => {
        if (params['state'] != null) {
          window.location.href = window.location.origin + '#/sessions/signin';
        }
      });   
    }

  }

  limpiarFiltros() {
    this.selectedEstado = this.estados[0];
    this.selectedTipoDoc = this.tiposDocs[0];
    this.selected = [];
    this.compras = this.comprasResp;
    this.folio = null;
    this.dateDesde = null;
    this.dateHasta = null;
    this.calcularTotalCuentas();

    // var x = document.getElementById("tbDetalle");

    this.filter();
  }


  onSel(val: any, c: any) {
    const added = this.selected.find(x => x.nro == c.nro && x.tipoDoc == c.tipoDoc);
    if (added != null) {
      //remueve
      for (let i = 0; i <= this.selected.length - 1; i++) {
        if (this.selected[i].nro == c.nro && this.selected[i].tipoDoc == c.tipoDoc) {
          this.selected.splice(i, 1);
          c.aPagar = c.saldoBase;
          break;
        }
      }
    } else {
      this.selected.push(c);
    }

    let valor: number = 0;

    this.selected.forEach(element => {
      valor += element.aPagar;
    });

    this.total = `$ ${valor}`;
    this.totalPagar = valor;

    if (this.selected.length == 0) {
      this.checkAll = false;
    }
  }

  onSelAll(val: any) {
    let valor: number = 0;
    this.selected = [];

    this.compras.forEach(element => {
      if(!element.bloqueadoPago){
        element.checked = val.target.checked
        if(!element.checked){
          element.aPagar = element.saldoBase;
        }
        if (val.target.checked) {
          this.selected.push(element);
          valor += element.aPagar;
        }
      }
    });

    this.total = `$ ${valor}`;
    this.totalPagar = valor;
  }

  onChangeAPagar(val: any, data: any, content) {

    if (val == "" || val == 0) {
      content.target.value = this.montoPipe.transform2(data.saldoBase);
    } else {
      val = val.replace('.', '')
      val = val.replace(',', '.')
      if (val > data.saldoBase) {
        this.notificationService.warning('El monto ingresado es mayor al saldo pendiente, favor volver a ingresar', '', true);
        content.target.value = this.montoPipe.transform2(data.saldoBase);

        this.selected.forEach(element => {
          if (element.nro === data.nro && element.tipoDoc == data.tipoDoc) {
            if (element.aPagar != parseFloat(val)) {
              element.aPagar = data.saldoBase;
            }
          }
        });

      } else {
        

        this.selected.forEach(element => {
          if (element.nro === data.nro && element.tipoDoc == data.tipoDoc) {
            if (element.aPagar != parseFloat(val)) {
              element.aPagar = parseFloat(val);
            }
          }
        });

        this.compras.forEach(element => {
          if (element.nro === data.nro && element.tipoDoc == data.tipoDoc) {
            if (element.aPagar != parseFloat(val)) {
              element.aPagar = parseFloat(val);
            }
          }
        });

        content.target.value = this.montoPipe.transform2(data.aPagar);

        if(val < data.saldoBase && this.esPagoRut){
          const added = this.selected.find(x => x.nro == data.nro && x.tipoDoc == data.tipoDoc);
          if (added != null) {
            //remueve
            for (let i=0; i <= this.selected.length -1; i++) {
              if (this.selected[i].nro == data.nro && this.selected[i].tipoDoc == data.tipoDoc &&  this.selected[i].aPagar != val) {
                this.selected.splice(i, 1);
                data.checked = false;
                break;
              }
            }
          } else {
            data.checked = true;
            this.selected.push(data);
          }
      
          let valor: number = 0; 
      
          this.selected.forEach(element => {
            valor += element.aPagar;  
          });
      
          this.total = `$ ${valor}`;
          this.totalPagar = valor;
      
          if (this.selected.length == 0) {
            this.checkAll = false;
          }
        }
      }

      this.calcularTotalCuentas();
    }

  }

  calcularTotalCuentas() {
    let total: number = 0;
    this.selected.forEach(element => {
      total += parseFloat(element.aPagar);
    });
    this.totalPagar = total;
  }

  changePasarela(p: any, e: any) {
    if (e.srcElement.checked) {
      this.selectedPasarela = p.idPasarela;

      this.pasarelas.forEach(element => {
        if (element.idPasarela != p.idPasarela) {
          let input = document.getElementById("cb" + element.idPasarela) as HTMLInputElement;
          input.checked = false;
        }
      });

    } else {
      this.selectedPasarela = 0;
    }
  }

  downloadPDF(numComprobante: any) {
    this.spinner.show();
    this.clientesService.getPDFPago(btoa(numComprobante)).subscribe(
      (res: any) => {
        if (res != '') {
          let a = document.createElement("a");
          a.href = "data:application/octet-stream;base64," + res;
          a.download = numComprobante + ".pdf"
          a.click();
        }
        this.spinner.hide();
      },
      err => { this.spinner.hide(); }
    );
  }

  volver() {
    this.modalService.dismissAll();
    window.location.href = window.location.origin + '#/sessions/signin';

  }


  onChange(item: any) {
    
    this.clienteSeleccionado = item;
  }


  seleccionarCliente() {
    if (this.clienteSeleccionado == null) {
      this.notificationService.warning('Debe seleccionar un cliente', '', true);
      return;
    } else {
      this.spinner.show();
      this.comprasResp = this.comprasResp.filter(x => x.codAux == this.clienteSeleccionado.codAux);
      this.comprasResp.forEach(element => {
        element.bloqueadoPago = false;
        if(element.codMon != this.configuracionPagos.monedaUtilizada && element.equivalenciaMoneda == 0){
          element.bloqueadoPago = true;
        }
        if (this.paymentResultState == 4 && this.foliosErrorComprobate != null && this.foliosErrorComprobate != '') {
          let folios = this.foliosErrorComprobate.split('-');
          folios.forEach(folio => {
            if (element.nro == folio) {
              element.bloqueadoPago = true;
            }
          });
        }
      });
      this.compras = this.comprasResp;
      let docsSegundaMoneda = this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
      if (docsSegundaMoneda.length > 0) {
        this.existDocSegundaMoneda = true;
      }

      let valor: number = 0;
      this.selected = [];
  
      this.compras.forEach(element => {
        if(!element.bloqueadoPago){
          element.checked = true;
          if (element.checked) {
            this.selected.push(element);
            valor += element.aPagar;
          }
        }
       
      });
  
      this.total = `$ ${valor}`;
      this.totalPagar = valor;
      this.spinner.hide();
      this.load = true;
      this.modalService.dismissAll();
    }
  }

  private getConfigDiseno() {
    this.disenoSerivce.getConfigDiseno().subscribe((res: ConfiguracionDiseno) => {
        this.configDiseno = res;
    }, err => { this.spinner.hide();});
}

}
