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

@Component({
  selector: 'app-account-state',
  templateUrl: './account-state.component.html',
  styleUrls: ['./account-state.component.scss']
})
export class AccountStateComponent implements OnInit {

  loading: boolean = false;
  estadosCuenta: any = [];
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
  logTbk: any = null;
  rut: string = '';
  razonSocial: string = '';

  checkAll: boolean = false
  page = 1;
  pageSize = 2;
  collectionSize = 8;

  constructor(private ngbDatepickerConfig: NgbDatepickerConfig, private clientesService: ClientesService, private authService: AuthService, private activatedRoute: ActivatedRoute,
    private notificationService: NotificationService, private ngbDatepickerI18n: NgbDatepickerI18n, private utils: Utils, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private router: Router, private ventasService: VentasService) {

    this.ngbDatepickerConfig.firstDayOfWeek = 1;

    this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
      return I18N_VALUES['es'].weekdays[weekday - 1];
    };

    this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
      return I18N_VALUES['es'].months[months - 1];
    };

  }

  ngOnInit(): void {

  }

  search() {
    this.spinner.show();

    const rut: string[] = this.rut.split('-');
    const rut2: string = rut[0].replace('.', '').replace('.', '')
    const model = { nombre: rut2 }

    this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res1: any) => {
      this.estadosCuenta = res1;

      if (res1 && res1.length > 0) {
        this.razonSocial = res1[0].razonSocial
      } else {
        this.razonSocial = '';
      }

      this.clientesService.getClienteComprasFromSoftland(model).subscribe((res: any) => {
        this.compras = res;
        this.comprasResp = res;
        this.estados = [];
        this.tiposDocs = [];

        res.forEach((element, index) => {
          const ex1 = this.estados.find(x => x.nombre == element.estado);
          const ex2 = this.tiposDocs.find(x => x.nombre == element.documento);

          if (ex1 == null) {
            this.estados.push({
              id: index + 1,
              nombre: element.estado
            });
          }
          if (ex2 == null) {
            this.tiposDocs.push({
              id: index + 1,
              nombre: element.documento
            });
          }
        });

        if (res1.length === 0 && res.length === 0) {
          this.notificationService.info('No se encontró información para el rut ingresado.', '', true);
        }

        this.spinner.hide();
      }, err => { this.spinner.hide(); });

    }, err => { this.spinner.hide(); });

  }

  filter() {
    let data: any = Object.assign([], this.comprasResp);

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.selectedEstado != null) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.selectedTipoDoc != null) {
      data = data.filter(x => x.documento == this.selectedTipoDoc.nombre)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fechaEmision) <= fHasta)
    }

    this.compras = data;
  }

  onSelect(val: any) {
    this.selectedDosc = val.selected;

    if (val.selected.length > 0) {
      let valor: number = 0;
      val.selected.forEach(element => {
        valor += element.debe
      });
      this.total = `$ ${valor}`;
      this.totalPagar = valor;
    } else {
      this.total = "$ 0";
      this.totalPagar = 0;
    }
  }

  pagar() {
    const user = this.authService.getUserPortal();
    if (user) {
      let detalle = [];
      const codAux = user.rut.replace('.', '').replace('.', '').split('-')[0]

      for (let i = 0; i <= this.selectedDosc.length - 1; i++) {
        detalle.push({
          IdPago: 0,
          Documento: this.selectedDosc[i].documento,
          Folio: this.selectedDosc[i].nro,
          Monto: this.selectedDosc[i].saldo,
          TipoDoc: this.selectedDosc[i].tipoDoc,
        });
      }

      let pago = {
        IdPago: 0,
        CodAux: codAux,
        MontoTotal: this.totalPagar,
        Detalles: detalle,
        Email: user.email
      }

      this.spinner.show();

      this.clientesService.getEstadoConexionSoftland().subscribe(
        resVal => {
          if (resVal) {
            this.clientesService.postSavePago(pago).subscribe(
              (res: any) => {
                this.spinner.hide();
                window.location.href = `${this.utils.Server}/pagotbk?idVenta=${res}&isDocumentPayment=1&redirectTo=${TbkRedirect.PortalInside}&idCobranza=0`;
              },
              err => { this.spinner.hide(); }
            );
          } else {
            this.spinner.hide(); this.notificationService.warning('No es posible realizar el pago en estos momentos, favor vuelva a intentar más tarde.', '', true);
          }
        },
        err => { this.spinner.hide(); this.notificationService.error('Error al verificar conexión.', '', true); }
      );

    } else {
      this.authService.signoutExpiredToken();
    }

  }

  openModal(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  closeModal() {
    this.modalService.dismissAll();
    window.location.href = window.location.origin + '/#/payment/accounts-state'
    //this.router.navigate(['/payment/accounts-state']);
  }

  limpiarFiltros() {
    this.selectedEstado = null;
    this.selectedTipoDoc = null;
    this.folio = null;
    this.dateDesde = null;
    this.dateHasta = null;

    this.filter();
  }

  refreshCountries() {

  }

  onSel(val: any, c: any) {
    const added = this.selectedDosc.find(x => x.nro == c.nro);
    if (added != null) {
      //remueve
      for (let i = 0; i <= this.selectedDosc.length - 1; i++) {
        if (this.selectedDosc[i].nro == c.nro) {
          this.selectedDosc.splice(i, 1);
          break;
        }
      }
    } else {
      this.selectedDosc.push(c);
    }

    let valor: number = 0;

    this.selectedDosc.forEach(element => {
      valor += element.debe;
    });

    this.total = `$ ${valor}`;
    this.totalPagar = valor;

    if (this.selectedDosc.length == 0) {
      this.checkAll = false;
    }
  }

  onSelAll(val: any) {
    let valor: number = 0;
    this.selectedDosc = [];

    this.compras.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.selectedDosc.push(element);
        valor += element.debe;
      }
    });

    this.total = `$ ${valor}`;
    this.totalPagar = valor;
  }

}
