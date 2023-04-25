import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ConfiguracionCorreoService } from '../../../shared/services/configuracioncorreo.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionCorreo } from '../../../shared/models/configuracioncorreo.model';
import { ITipoApi } from '../../../shared/enums/TipoApi';
import { Utils } from '../../../shared/utils';
import { toInteger } from '@ng-bootstrap/ng-bootstrap/util/util';
import { number } from 'ngx-custom-validators/src/app/number/validator';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DomSanitizer } from '@angular/platform-browser';

interface AutoCompleteModel {
  value: any;
  display: string;
}

export interface IPreviewImgs {
  url?: string;
}


@Component({
  selector: 'app-correos',
  templateUrl: './correos.component.html',
  styleUrls: ['./correos.component.scss']
})
export class CorreosComponent implements OnInit {

  public config: ConfiguracionCorreo = new ConfiguracionCorreo();
  public correo1: AutoCompleteModel[] = [];
  public correo2: AutoCompleteModel[] = [];
  public correo3: AutoCompleteModel[] = [];
  public correo4: AutoCompleteModel[] = [];
  public isSsl: boolean = false;

  public defaultImageLogoPortada: FileList = null;
  public urlImagenLogoPortada: IPreviewImgs = null;

  cantidadCorreosAccesos: string = '0';
  cantidadCorreosNotificacion: string = '0';
  general: number = 0;
  pagos: number = 0;
  activacion: number = 0;
  cambioDatos: number = 0;
  actualizaClave: number = 0;
  actualizaCorreo: number = 0;
  recuperaClave: number = 0;
  envioDocumentos: number = 0;
  notificacionPago: number = 0;
  notificacionPagoSinComprobante: number = 0;
  cobranza: number = 0;
  preCobranza: number = 0;
  estadoCuenta: number = 0;
  modalTitle: string = '';
  template: string = '';
  url: any = null;
  constructor(private notificationService: NotificationService, private configuracionCorreoService: ConfiguracionCorreoService,
    private spinner: NgxSpinnerService, private utils: Utils, private modalService: NgbModal, private sanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.getConfigCorreos();
  }

  getConfigCorreos() {
    this.spinner.show();
    this.correo1 = [];
    this.configuracionCorreoService.getConfigCorreos().subscribe((res: ConfiguracionCorreo[]) => {
      this.config = res[0];
      if (this.config.cantidadCorreosAcceso != null && this.config.cantidadCorreosAcceso != undefined) {
        this.cantidadCorreosAccesos = this.config.cantidadCorreosAcceso.toString();
      }
      if (this.config.cantidadCorreosNotificacion != null && this.config.cantidadCorreosNotificacion != undefined) {
        this.cantidadCorreosNotificacion = this.config.cantidadCorreosNotificacion.toString();
      }



      const c1: string[] = this.config.correoAvisoPago != null ? this.config.correoAvisoPago.split(';') : [];

      c1.forEach(element => {
        if (element && element.length > 2) {
          this.correo1.push({ value: element, display: element });
        }
      });

      if (this.config.logoCorreo != null && this.config.logoCorreo != '') {
        let preview: IPreviewImgs = {
          url: this.config.logoCorreo
        }
        this.urlImagenLogoPortada = preview;
      }
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener configuración', '', true); });
  }

  public onAdd(tag: AutoCompleteModel, type: number) {
    switch (type) {
      case 1:
        this.removeFromArrayIfMailIsInvalid(tag.value, this.correo1);
        this.toLowerMails(this.correo1);
        break;
      case 2:
        this.removeFromArrayIfMailIsInvalid(tag.value, this.correo2);
        this.toLowerMails(this.correo2);
        break;
      case 3:
        this.removeFromArrayIfMailIsInvalid(tag.value, this.correo3);
        this.toLowerMails(this.correo3);
        break;
      case 4:
        this.removeFromArrayIfMailIsInvalid(tag.value, this.correo4);
        this.toLowerMails(this.correo4);
        break;
    }

  }

  toLowerMails(arrayCorreos: AutoCompleteModel[]) {
    for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
      arrayCorreos[i].value = arrayCorreos[i].value.toLowerCase();
      arrayCorreos[i].display = arrayCorreos[i].display.toLowerCase();
    }
  }

  removeFromArrayIfMailIsInvalid(mail: string, arrayCorreos: AutoCompleteModel[]) {
    if (!this.utils.validateMail(mail)) {
      this.notificationService.warning('Debe ingresar un correo válido.', '', true);
      for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
        if (arrayCorreos[i].value == mail) {
          arrayCorreos.splice(i, 1);
          break;
        }
      }
      return;
    }
  }

  validaEmail() {
    if (!this.utils.validateMail(this.config.correoOrigen)) {
      this.notificationService.warning('Debe ingresar un correo válido.', '', true);
      this.config.correoOrigen = '';
      return;
    }
  }

  save() {
    let c1: string = '';
    let c2: string = '';
    let c3: string = '';
    let c4: string = '';

    this.correo1.forEach(element => {
      c1 += `${element.value};`;
    });

    this.config.ssl = this.config.ssl ? 1 : 0;
    this.config.correoAvisoPago = c1.length > 0 ? c1.substring(0, c1.length - 1) : '';

    this.spinner.show();
    debugger
    var email_analizado = /^([^]+)@(\w+).(\w+)$/.exec(this.config.usuario);

    var casilla = '';
    if (email_analizado != null) {
      var [, nombre, servidor, dominio] = email_analizado;

      casilla = servidor + '.' + dominio;
    }

    // this.configuracionCorreoService.getCantidad(casilla).subscribe((res: number) => {

      // var cantidad = parseInt(this.cantidadCorreosAccesos) + parseInt(this.cantidadCorreosNotificacion);
      // if (cantidad > res) {
      //   this.notificationService.warning('La suma entre correos diarios de acceso y notificación supera el máximo de la casilla  (' + res + ')', '', true);
      //   this.spinner.hide();
      // } else {
        // if (this.cantidadCorreosAccesos == '') {
        //   this.cantidadCorreosAccesos = '0';
        // }
        // if (this.cantidadCorreosNotificacion == '') {
        //   this.cantidadCorreosNotificacion = '0';
        // }
        // this.config.cantidadCorreosAcceso = parseInt(this.cantidadCorreosAccesos);
        // this.config.cantidadCorreosNotificacion = parseInt(this.cantidadCorreosNotificacion);
        this.configuracionCorreoService.edit(this.config).subscribe((res: ConfiguracionCorreo) => {
          this.notificationService.success('Datos Actualizados Correctamente', '', true);
          this.getConfigCorreos();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.utils.handleErrors(err, ITipoApi.POST); });
      // }
    // }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cantidad de correos por dia', '', true); });
  }

  MostrarSeccion(tipo: number) {
    debugger
    switch (tipo) {
      case 1:
        this.general == 0 ? this.general = 1 : this.general = 0;
        break;

      case 2:
        this.pagos == 0 ? this.pagos = 1 : this.pagos = 0;
        break;

      case 3:
        this.activacion == 0 ? this.activacion = 1 : this.activacion = 0;
        break;

      case 4:
        this.cambioDatos == 0 ? this.cambioDatos = 1 : this.cambioDatos = 0;
        break;

      case 5:
        this.actualizaClave == 0 ? this.actualizaClave = 1 : this.actualizaClave = 0;
        break;

      case 6:
        this.actualizaCorreo == 0 ? this.actualizaCorreo = 1 : this.actualizaCorreo = 0;
        break;

      case 7:
        this.recuperaClave == 0 ? this.recuperaClave = 1 : this.recuperaClave = 0;
        break;

      case 8:
        this.envioDocumentos == 0 ? this.envioDocumentos = 1 : this.envioDocumentos = 0;
        break;

      case 9:
        this.notificacionPago == 0 ? this.notificacionPago = 1 : this.notificacionPago = 0;
        break;

      case 10:
        this.notificacionPagoSinComprobante == 0 ? this.notificacionPagoSinComprobante = 1 : this.notificacionPagoSinComprobante = 0;
        break;

      case 11:
        this.cobranza == 0 ? this.cobranza = 1 : this.cobranza = 0;
        break;

      case 12:
        this.preCobranza == 0 ? this.preCobranza = 1 : this.preCobranza = 0;
        break;

      case 13:
        this.estadoCuenta == 0 ? this.estadoCuenta = 1 : this.estadoCuenta = 0;
        break;
    }
  }

  verificaColor() {
    let pattern = /^#+([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$/

    if (this.config.colorBoton != '' && this.config.colorBoton != null) {  //FCA 08-06-2022
      if (!pattern.test(this.config.colorBoton)) {
        this.config.colorBoton = '';
      }
    }
  }


  preview(event: any) {
    let files = event.target.files;
    if (files) {
      for (let file of files) {
        let reader = new FileReader();
        reader.onload = (e: any) => {
          let img: IPreviewImgs = {
            url: e.target.result
          }
          this.urlImagenLogoPortada = img;
        }
        reader.readAsDataURL(file);
      }
      this.defaultImageLogoPortada = event.target.files
    }

  }

  onChange(event: any) {
    this.defaultImageLogoPortada = event.srcElement.files;
  }

  deletePreviewImg() {
    this.urlImagenLogoPortada = null;
    this.defaultImageLogoPortada = null;
    this.clearInput(document.getElementById('fileImageLogoPortada'));
  }

  clearInput(input) {
    try {
      input.value = null;
    } catch (ex) { }
    if (input.value) {
      input.parentNode.replaceChild(input.cloneNode(true), input);
    }
  }

  uploadImage() {
    this.configuracionCorreoService.subirImagen(this.defaultImageLogoPortada).then(res => {
      this.notificationService.success('Datos Actualizados Correctamente', '', true);
      this.defaultImageLogoPortada = null;
      this.getConfigCorreos();
      this.spinner.hide();
    }).catch(err => {
      this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir  Logo', '', true);
    });
  }

  saveDisenoCorreo(tipo: number) {
    this.spinner.show();

    this.configuracionCorreoService.actualizaTextos(tipo, this.config).subscribe((res: ConfiguracionCorreo) => {
      if (tipo == 1) {
        this.uploadImage();
      } else {
        this.notificationService.success('Datos Actualizados Correctamente', '', true);
        this.getConfigCorreos();
        this.spinner.hide();
      }
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al actualizar datos', '', true); });
  }




  previsualizar(tipo: number, content) {
    this.spinner.show();
    switch (tipo) {
      case 1:
        this.modalTitle = 'GENERAL';
        break;

      case 2:
        this.modalTitle = 'PAGOS';
        break;
      case 3:
        this.modalTitle = 'ENVÍO DE ACCESO';
        break;
      case 4:
        this.modalTitle = 'ACTUALIZACIÓN DE DATOS CLIENTE';
        break;
      case 5:
        this.modalTitle = 'ACTUALIZACIÓN DE CLAVE';
        break;
      case 6:

        break;
      case 7:
        this.modalTitle = 'RECUPERAR CLAVE';
        break;
      case 8:
        this.modalTitle = 'ENVÍO DE DOCUMENTOS';
        break;
      case 9:

        break;
      case 10:

        break;
      case 11:
        this.modalTitle = 'ENVÍO COBRANZA';
        break;
      case 12:
        this.modalTitle = 'ENVÍO PRE COBRANZA';
        break;
      case 13:
        this.modalTitle = 'ENVÍO ESTADO DE CUENTA';
        break;
    }

    this.configuracionCorreoService.getTemplate(tipo, this.config).subscribe((res: any) => {
      debugger
      this.template = res.body;
      if (tipo == 1) {
        this.template = this.template.replace('{LOGO}', this.urlImagenLogoPortada.url);
      }
      this.url = this.sanitizer.bypassSecurityTrustResourceUrl('data:text/html;charset=utf-8,' + encodeURIComponent(this.template));
      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', windowClass: 'modalPrevisualizar' });
      let el = document.getElementsByClassName('modal-content');
      el[0].setAttribute("style", "width: 160%; margin-left: -100px;");
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error cargar previsualización', '', true); });
  }

}
