import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ProductosService } from '../../../shared/services/productos.service';
import { CategoriasService } from '../../../shared/services/categorias.service';
import { UnidadMedidaService } from '../../../shared/services/unidadmedida.service';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from "ngx-spinner";
import { Producto } from '../../../shared/models/productos.model';
import { Categorias } from '../../../shared/models/categorias.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { UnidadMedida } from '../../../shared/models/unidadmedida.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { Cliente } from 'src/app/shared/models/clientes.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { AuthService } from 'src/app/shared/services/auth.service';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { Utils } from 'src/app/shared/utils';
import { Router } from '@angular/router';
import { ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { User } from '../../chat/chat.service';
import { Usuario } from 'src/app/shared/models/usuarios.model';
import { UsuariosService } from 'src/app/shared/services/usuarios.service';

export interface IPreviewImgs {
  name?: string;
  url?: string;
  idProductoImagen?: number;
}

@Component({
  selector: 'app-productos',
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.scss'],
  animations: [SharedAnimations]
})
export class ProductosComponent implements OnInit {

  usuario: any;

  changePass: any = {
    newPass1: '',
    newPass2: ''
  };
  public nombreEmpresa: string = '';

  loadingUpdate: boolean = false;
  loadingChangePass: boolean = false;
  loadingEstadoCuenta: boolean = false;
  loadingCompras: boolean = false;
  invalidRut: boolean = false;
  nuevoCorreo: string = '';

  constructor(private softlandService: ConfiguracionSoftlandService,
    private configuracionService: ConfiguracionPagoClientesService,
    private authService: AuthService,
    private clientesService: ClientesService,
    private modalService: NgbModal,
    private spinner: NgxSpinnerService,
    private utils: Utils,
    private notificationService: NotificationService,
    private router: Router, private usuariosService: UsuariosService) {

  }

  getUserData() {
    var user = this.authService.getuser();
    if (user) {
      const data: any = {
        correo: user.email,
        rut: user.rut,
        codaux: user.codAux
      };

      this.usuariosService.getUsuarioByMail(data).subscribe((res: Usuario) => {
        this.usuario = res;
        this.usuariosService.getEmpresa().subscribe((res: string) => {

          this.nombreEmpresa = res;
        }, err => { this.spinner.hide(); });
        this.spinner.hide();

      }, err => { this.spinner.hide(); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }



  ngOnInit(): void {
    this.getUserData();
  }

  openModalChangePass(content) {
    this.changePass.newPass1 = '';
    this.changePass.newPass2 = '';
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  openmodalCambioCorreo(content) {
    this.nuevoCorreo = '';
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async changeCorreo() {
    let user = this.usuario;
    user.email = this.nuevoCorreo;

    const response = await this.notificationService.confirmation('Cambiar Correo', 'Al cambiar su correo usted será redirigido al inicio de sesión, ¿Confirma actualizar su correo?');
    if (response.isConfirmed) {
      this.spinner.show();
      this.usuariosService.changeCorreo(user).subscribe(res => {
        this.spinner.hide();
        this.modalService.dismissAll();
        this.notificationService.success('Correo actualizado correctamente', '', true);

        setTimeout(() => {
          this.authService.signout();
        }, 1000);

      }, err => {
        this.spinner.hide();
        this.notificationService.error('Ocurrió un error al cambiar correo.', '', true);
      });
    }

  }

  async onChangePass() {
    if (this.changePass.newPass1 != this.changePass.newPass2) {
      this.notificationService.warning('Claves no coinciden.', '', true);
      return;
    }

    const response = await this.notificationService.confirmation('Cambiar Clave', 'Al cambiar su clave usted será redirigido al inicio de sesión, ¿Confirma actualizar su clave?');
    if (response.isConfirmed) {
      this.loadingChangePass = true;

      this.spinner.show();

      const data: any = {
        idUsuario: this.usuario.idUsuario,
        password: this.changePass.newPass1
      };

      this.usuariosService.changePassword(data).subscribe(res => {
        this.spinner.hide();
        this.modalService.dismissAll();
        this.changePass.newPass1 = '';
        this.changePass.newPass2 = '';
        this.notificationService.success('Clave actualizada correctamente', '', true);

        setTimeout(() => {
          this.authService.signout();
        }, 1000);

      }, err => {
        this.spinner.hide();
        if (err && err.error != null && err.error.message != "") {
          this.notificationService.error(err.error.message, '', true);
        } else {
          this.notificationService.error('Ocurrió un error al cambiar clave.', '', true);
        }
      });

    }
  }

  onBlurEmail() {
    let pattern = /[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}$/
    if (this.nuevoCorreo != null && this.nuevoCorreo != '') {
      if (!pattern.test(this.nuevoCorreo)) {
        this.notificationService.warning('Formato de correo invalido.', '', true);
        this.nuevoCorreo = ''
      } else {
        this.nuevoCorreo = this.nuevoCorreo.toLowerCase();
      }
    }
  }

}