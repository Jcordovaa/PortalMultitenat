import { Component, OnInit } from '@angular/core';
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
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';


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
  selector: 'app-accounts-state',
  templateUrl: './accounts-state.component.html',
  styleUrls: ['./accounts-state.component.scss']
})
export class AccountsStateComponent implements OnInit {

  loading: boolean = false;
  compras: any = [];
  abonos: any = [];
  totalDocumentoAbono: number = 0;
  saldoPendiente: number = 0;
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
  existDocSegundaMoneda: boolean = false;
  configuracionPagos: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
  page = 1;
  pageSize = 2;
  collectionSize = 8;
  configuracion: ConfiguracionPortal = new ConfiguracionPortal();
  valorUfActrual: number = 0;
  fechaActual = new Date();
  foliosErrorComprobate: string = '';
  bloqueaBotonPago: boolean = false;
  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();
  varloUfOrigen: number = 0;
  load: boolean = false;
  noResultText: string = 'No existen resultados para la consulta'

  constructor(private ngbDatepickerConfig: NgbDatepickerConfig, private disenoSerivce: ConfiguracionDisenoService,
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
    const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
    if (configuracionCompletaPortal != null) {
      this.configDiseno = configuracionCompletaPortal.configuracionDiseno;
      this.configuracion = configuracionCompletaPortal.configuracionPortal;
      this.configuracionPagos = configuracionCompletaPortal.configuracionPagoCliente;
    }


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
        }, err => {
          this.logPasarela = null;
          var btn = document.getElementById('btnModal')
          btn.click();
        });
      }
    });

    const user = this.authService.getuser();
    if (user) {
      this.spinner.show();


      const model = { codAux: user.codAux, folio: 0 }

      this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any) => {

        this.comprasResp = res;
        this.comprasResp.forEach(element => {
          element.bloqueadoPago = false;
          if (element.codMon != this.configuracionPagos.monedaUtilizada && element.equivalenciaMoneda == 0) {
            
            //element.bloqueadoPago = true;
          }

          if (this.paymentResultState == 4 && this.foliosErrorComprobate != null && this.foliosErrorComprobate != '') {
            let folios = this.foliosErrorComprobate.split('-');
            folios.forEach(folio => {
              if (element.nro == folio) {
                //element.bloqueadoPago = true;
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
        debugger
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

    this.selected = [];
    this.compras.forEach(element => {
      element.checked =  false;
  });
  this.calcularTotalCuentas();
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
      if (this.existDocSegundaMoneda) {
        const model = { Tipo: element.documento, Folio: element.nro, Emision: fem, Vencimiento: fve, Moneda: element.desMon, Monto_Moneda_Origen: element.debe, Monto_Moneda_Nacional_Emision: element.montoOriginalBase, Monto_Mda_Nacional_Actualizado: element.montoBase, Saldo_En_Mda_Nacional: element.saldoBase, Estado: element.estado };
        listaExportacion.push(model);
      } else {
        const model = { Tipo: element.documento, Folio: element.nro, Emision: fem, Vencimiento: fve, Moneda: element.desMon, Monto: element.debe, Saldo: element.saldo, Estado: element.estado };
        listaExportacion.push(model);
      }


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

  pagar() {
    this.bloqueaBotonPago = true;
    if (this.selectedPasarela == 0) {
      this.notificationService.warning('Debe seleccionar la forma de pago.', '', true);
      this.bloqueaBotonPago = false;
      return;
    }

    const user = this.authService.getuser();
    if (user) {
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

      let pago = {
        IdPago: 0,
        IdCliente: 0,
        FechaPago: new Date(),
        HoraPago: '',
        MontoPago: this.totalPagar,
        ComprobanteContable: '',
        IdPagoEstado: 1,
        Rut: user.rut,
        CodAux: user.codAux,
        Nombre: '',
        Correo: user.email,
        IdPasarela: this.selectedPasarela,
        EsPagoRapido: 0,
        PagosDetalle: detalle
      }

      this.spinner.show();

      let rutCliente = btoa(user.rut);
      let tenant = btoa(window.location.hostname );
      let datosPago = window.btoa(encodeURIComponent(user.nombre) + ';' + encodeURIComponent(user.nombre)+ ';' + user.rut + ';' + user.email);
      this.clientesService.getEstadoConexionSoftland().subscribe(
        resVal => {
          if (resVal) {
            this.clientesService.postSavePago(pago).subscribe(
              (res: any) => {
                this.spinner.hide();
                //LLama a procesador de pago que se encargara de levantar la pasarela correspondiente

                this.pasarelaService.generaPagoElectronico(res,this.selectedPasarela,rutCliente,0,datosPago,TbkRedirect.Front,tenant).subscribe(
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

    } else { this.notificationService.error('Ocurrió un error al obtener usuario, favor vuelva a iniciar sesión.', '', true); this.bloqueaBotonPago = false; }
  }

  openModal(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  closeModal() {
    this.modalService.dismissAll();
    this.bloqueaBotonPago = false;
    window.location.href = window.location.origin + '/#/payment/accounts-state'
    //this.router.navigate(['/payment/accounts-state']);
  }

  limpiarFiltros() {
    this.selectedEstado = this.estados[0];
    this.selectedTipoDoc = this.tiposDocs[0];
    this.selected = [];
    this.folio = null;
    this.dateDesde = null;
    this.dateHasta = null;
    

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
      if (!element.bloqueadoPago) {
        element.checked = val.target.checked
        if (!element.checked) {
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
debugger
    if (val == "" || val == 0) {
      val = data.saldoBase;
      val = val.replace(new RegExp('\\.', 'g'), '')
       val = val.replace(new RegExp(',', 'g'), '\\.')
      content.target.value = this.montoPipe.transform2(val);
    } else {
      val = val.replace(new RegExp('\\.', 'g'), '')
      val = val.replace(new RegExp(',', 'g'), '\\.')
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

        if (val < data.saldoBase) {
          const added = this.selected.find(x => x.nro == data.nro && x.tipoDoc == data.tipoDoc);
          if (added != null) {
            //remueve
            for (let i = 0; i <= this.selected.length - 1; i++) {
              if (this.selected[i].nro == data.nro && this.selected[i].tipoDoc == data.tipoDoc && this.selected[i].aPagar != val) {
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

  openInfoModal(content, compra: any) {

    this.spinner.show();
    const model = { codAux: compra.codAux, folio: compra.nro, TipoDoc: compra.tipoDoc }

    this.clientesService.getPagosDocumento(model).subscribe((res: any) => {

      this.abonos = [];
      this.abonos = res;

      if (this.abonos.length <= 0) {
        this.spinner.hide();
        this.notificationService.warning('El documento no posee abonos asociados', '', true);
        return;
      }
      this.totalDocumentoAbono = compra.debe;
      this.saldoPendiente = compra.saldoBase;
      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
  }



}
