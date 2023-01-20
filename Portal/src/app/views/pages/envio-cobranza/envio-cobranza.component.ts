import { Component, OnInit  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from "../../../shared/services/auth.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { VentasService } from '../../../shared/services/ventas.service';
import { Utils } from '../../../shared/utils';
import { TbkRedirect } from '../../../shared/enums/TbkRedirect';
import { NgxSpinnerService } from "ngx-spinner";
import { EnviaCobranza } from 'src/app/shared/models/enviacobranza.model';

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
  selector: 'app-envio-cobranza',
  templateUrl: './envio-cobranza.component.html',
  styleUrls: ['./envio-cobranza.component.scss']
})

export class EnvioCobranzaComponent implements OnInit {

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
  showDetail: boolean = false;
  searchRut: string = '';
  searchRazonSocial: string = '';
  contactosCliente: any = [];
  contactosSeleccionados: any = [];
  otroCorreo: string = '';
  checkAll: boolean = false

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
    if (this.dateDesde !=  null) {
        const fDesde = new Date(this.dateDesde.year, this.dateHasta.month -1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
    }
    if (this.dateHasta !=  null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month -1, this.dateHasta.day, 23, 59, 59);
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

  limpiarFiltros() {
    this.selectedEstado = null;
    this.selectedTipoDoc = null;
    this.folio = null;
    this.dateDesde = null;
    this.dateHasta = null;
    
    this.filter();
  }

  validaRut() {
    if (this.utils.isValidRUT(this.searchRut)) {
      this.searchRut = this.utils.checkRut(this.searchRut);
    }
  }

  search() {
    if (this.utils.isValidRUT(this.searchRut)) {      

      this.spinner.show();
      this.showDetail = false;
  
      const rut: string[] = this.searchRut.split('-');
      const rut2: string = rut[0].replace('.', '').replace('.', '')
      const model = { nombre: rut2 }
  
      this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any) => {
        this.compras = res;
        this.comprasResp = res;
  
        this.estados = [];
        this.tiposDocs = [];
        this.dateDesde = null;
        this.dateHasta = null;
        this.folio = null;
        
  
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
  
        if (this.compras.length > 0) {
          this.showDetail = true;
          this.searchRazonSocial = this.compras[0].razonSocial;
        } else {
          this.searchRazonSocial = '';
          this.notificationService.info('No se encontró información para el rut ingresado.', '', true);
        }
  
        this.limpiarFiltros();
        this.spinner.hide();
  
      }, err => { this.spinner.hide(); });

    } else {
      this.notificationService.warning('Rut ingresado no es válido', '', true);
      this.showDetail = false;
    }
  }

  openModalSend(content) {
    this.spinner.show();
    this.contactosCliente = [];

    const rut: string[] = this.searchRut.split('-');
    const rut2: string = rut[0].replace('.', '').replace('.', '')

    this.clientesService.getContactosClienteSoftland(rut2).subscribe(res => {
      this.contactosCliente = res;
      this.spinner.hide();
      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });    
  }

  onChange(item: any) {
    const added = this.contactosSeleccionados.find(x => x.correoContacto == item.correoContacto);
    if (added == null) {
      this.contactosSeleccionados.push(item)
    } else {
      for (let i: number = 0; i <= this.contactosSeleccionados.length -1; i++) {
        if (this.contactosSeleccionados[i].correoContacto == item.correoContacto) {
          this.contactosSeleccionados.splice(i, 1);
          break;
        }
      }
    }
  }

  send() {
    console.log(this.selectedDosc)
    if (this.contactosSeleccionados.length == 0 && this.otroCorreo.trim().length == 0) {
      this.notificationService.warning('Debe seleccionar o ingresar algún correo de contacto.', '', true);
      return;
    }

    this.spinner.show();

    const rut: string[] = this.searchRut.split('-');
    const codAux: string = rut[0].replace('.', '').replace('.', '')

    let correos: string = '';
    this.contactosSeleccionados.forEach(element => {
      correos = correos + element.correoContacto + ";";
    });

    if (this.otroCorreo != null && this.otroCorreo.trim().length > 0) {
      correos = correos + this.otroCorreo;
    }

    let documentosACobrar: string = '';
    this.selectedDosc.forEach(element => {
      documentosACobrar = documentosACobrar + element.nro + ";";
    });

    const data: EnviaCobranza = {
      id: 0,
      rut: this.searchRut,
      codAux: codAux,
      documentosACobrar: documentosACobrar,
      correos: correos,
      fechaEnvio: new Date(),
      estado: 1
    };

    this.clientesService.enviaCobranza(data).subscribe(res => {
      this.spinner.hide();
      this.otroCorreo = '';
      this.modalService.dismissAll();      
      this.notificationService.success('Envío de cobranza enviado correctamente.', '', true);
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });

  }
  
  onSel(val: any, c: any) {
    const added = this.selectedDosc.find(x => x.nro == c.nro);
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
