import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';
import { NotificationService } from 'src/app/shared/services/notificacion.service';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-forgot',
  templateUrl: './forgot.component.html',
  styleUrls: ['./forgot.component.scss'],
  animations: [SharedAnimations]
})



export class ForgotComponent implements OnInit {

  rut: string = '';
  email: string = '';
  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();

  constructor(private utils: Utils, private notificationService: NotificationService,  private clienteService: ClientesService, 
    private spinner: NgxSpinnerService, private disenoSerivce: ConfiguracionDisenoService) { }

  ngOnInit() {
    this.spinner.show();
    this.disenoSerivce.getConfigDiseno().subscribe((res: ConfiguracionDiseno) => {
      this.configDiseno = res;
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
  }


  validaRut() {

    if (this.utils.isValidRUT(this.rut)) {
      this.rut = this.utils.checkRut(this.rut);
    } else {
      this.rut = '';
      this.notificationService.warning('RUT invalido', '', true);
    }
  }

  async recuperarContrasena(){
    if(this.email == '' || this.rut == ''){
      this.notificationService.warning('Debe ingresar los datos para recuperar contraseña.', '', true);
      return;
    }
    const response = await this.notificationService.confirmation('Recuperar Contraseña', 'Se enviará una nueva contraseña a su correo, para cambiarla deberá hacerlo desde Mi Perfil ¿Desea continuar?');
    if (response.isConfirmed) {
      
      this.spinner.show();
      let cliente = {
        rut: this.rut,
        codAux: this.rut.replace('.','').replace('.','').split('-')[0],
        correo: this.email
      }

      this.clienteService.postRecuperarContrasena(cliente).subscribe(res => {
        if(res == 1){
          this.notificationService.success('Nueva contraseña enviada, Revise su bandeja de correo electrónico.', '', true);          
        }else{
          this.notificationService.error('Usuario no encontrado.', '', true);
        }
        this.spinner.hide();
      }, err => {  this.spinner.hide(); this.notificationService.error('Ocurrió un error al actualizar contraseña, favor comunicarse con un administrador', '', true); });
    }
  }
}
