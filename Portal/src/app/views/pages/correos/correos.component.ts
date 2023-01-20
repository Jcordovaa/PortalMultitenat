import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ConfiguracionCorreoService } from '../../../shared/services/configuracioncorreo.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionCorreo } from '../../../shared/models/configuracioncorreo.model';
import { ITipoApi } from '../../../shared/enums/TipoApi';
import { Utils } from '../../../shared/utils';
import { toInteger } from '@ng-bootstrap/ng-bootstrap/util/util';
import { number } from 'ngx-custom-validators/src/app/number/validator';

interface AutoCompleteModel {
  value: any;
  display: string;
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

  cantidadCorreosAccesos : string = '0';
  cantidadCorreosNotificacion : string = '0';

  constructor(private notificationService: NotificationService, private configuracionCorreoService: ConfiguracionCorreoService,
    private spinner: NgxSpinnerService, private utils: Utils) { }

  ngOnInit(): void {
    this.getConfigCorreos();
  }

  getConfigCorreos() {
    this.spinner.show();
    this.correo1 = [];
    this.configuracionCorreoService.getConfigCorreos().subscribe((res: ConfiguracionCorreo[]) => {
      this.config = res[0];
      if(this.config.cantidadCorreosAcceso != null && this.config.cantidadCorreosAcceso != undefined ){
        this.cantidadCorreosAccesos = this.config.cantidadCorreosAcceso.toString();
      }
      if(this.config.cantidadCorreosNotificacion != null && this.config.cantidadCorreosNotificacion != undefined ){
        this.cantidadCorreosNotificacion = this.config.cantidadCorreosNotificacion.toString();
      }
      
      

      const c1: string[] = this.config.correoAvisoPago != null ? this.config.correoAvisoPago.split(';') : [];
      
      c1.forEach(element => {
        if (element && element.length > 2) {
          this.correo1.push({ value: element, display: element });
        }        
      });      

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
    for (let i: number = 0; i <= arrayCorreos.length -1; i++) {
      arrayCorreos[i].value = arrayCorreos[i].value.toLowerCase();
      arrayCorreos[i].display = arrayCorreos[i].display.toLowerCase();
    }
  }

  removeFromArrayIfMailIsInvalid(mail: string, arrayCorreos: AutoCompleteModel[]) {
    if (!this.utils.validateMail(mail)) {
      this.notificationService.warning('Debe ingresar un correo válido.','', true);
      for (let i: number = 0; i <= arrayCorreos.length -1; i++) {
        if (arrayCorreos[i].value == mail) {
          arrayCorreos.splice(i, 1);
          break;
        }
      }
      return;
    }    
  }

  validaEmail(){
    if (!this.utils.validateMail(this.config.correoOrigen)) {
      this.notificationService.warning('Debe ingresar un correo válido.','', true);
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
    this.config.correoAvisoPago = c1.length > 0 ? c1.substring(0, c1.length -1) : '';
 
    this.spinner.show();
    
    var email_analizado = /^([^]+)@(\w+).(\w+)$/.exec(this.config.usuario);

    var casilla = '-';
    if (email_analizado != null)
    {
      var [,nombre,servidor,dominio] = email_analizado;

      casilla = servidor +'.'+ dominio;
    }
    
    this.configuracionCorreoService.getCantidad(casilla).subscribe((res: number) => {
      
        var cantidad = parseInt(this.cantidadCorreosAccesos) + parseInt(this.cantidadCorreosNotificacion) ;
        if(cantidad > res){
        this.notificationService.warning('La suma entre correos diarios de acceso y notificación supera el máximo de la casilla  ('+ res+')', '', true);
        this.spinner.hide();
      }else{
        if(this.cantidadCorreosAccesos == '')
        {
          this.cantidadCorreosAccesos = '0';
        }
        if(this.cantidadCorreosNotificacion == '')
        {
          this.cantidadCorreosNotificacion = '0';
        }
        this.config.cantidadCorreosAcceso = parseInt(this.cantidadCorreosAccesos);
        this.config.cantidadCorreosNotificacion = parseInt(this.cantidadCorreosNotificacion);
        this.configuracionCorreoService.edit(this.config).subscribe((res: ConfiguracionCorreo) => {
          this.notificationService.success('Datos Actualizados Correctamente', '', true);
          this.getConfigCorreos();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.utils.handleErrors(err, ITipoApi.POST); });
      }
    }, err => {this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cantidad de correos por dia', '', true); });    
  }

}
