import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { SecurityService } from '../../../shared/services/secutiry.service';
import { AuthService } from '../../../shared/services/auth.service';
import { NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Perfil, Usuarios } from '../../../shared/models/security.model';
import { Mail } from '../../../shared/models/mail.model';
import { MailTipo } from '../../../shared/enums/MailTipo';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { MailService } from '../../../shared/services/mail.service';

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
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
  animations: [SharedAnimations]
})
export class UsersComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Usuario';
  public usuarios: Usuarios[] = [];
  public usuario: Usuarios = null;
  public noResultsText: string = '';
  public selectedPerfil: Perfil = null;
  public perfiles: Perfil[] = [];
  public radioCheckedM: boolean = true;
  public radioCheckedF: boolean = false;
  public selectedDate: any;
  public password1: string = '';
  public password2: string = '';
  public model: NgbDateStruct = {
    "year": 2018,
    "month": 8,
    "day": 15
  };
  public model2: NgbDateStruct = {
    "year": new Date().getFullYear(),
    "month": new Date().getMonth() + 1,
    "day": new Date().getDate()
  };

  public loaded: boolean = false;
  public totalItems: number = 0;
  public config: any;
  public p: number = 1;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };
  dateMask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];

  constructor(private securityService: SecurityService, private spinner: NgxSpinnerService, private ngbDatepickerI18n: NgbDatepickerI18n,
    private modalService: NgbModal, private notificationService: NotificationService, private ngbDatepickerConfig: NgbDatepickerConfig,
    private mailService: MailService, private authService: AuthService) {
    this.usuario = new Usuarios();

    this.ngbDatepickerConfig.firstDayOfWeek = 1;
    this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
      return I18N_VALUES['es'].weekdays[weekday - 1];
    };
    this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
      return I18N_VALUES['es'].months[months - 1];
    };
    // this.ngbDatepickerI18n.getMonthFullName = (weekday: number) => {
    //   return I18N_VALUES['es'].weekdays[weekday - 1];
    // };
    // this.ngbDatepickerI18n.getDayAriaLabel = (weekday: string) => {
    //   return I18N_VALUES['es'].weekdays[weekday - 1];
    // };
  }

  ngOnInit(): void {
    this.getUsuarios();
    this.getPerfiles();
  }

  dateEvent(event: any) {
  }

  getPerfiles() {
    let pag: Paginator = Object.assign({}, this.paginador);
    pag.endRow = 1000;

    this.securityService.getPerfilesByPage(pag).subscribe((res: Perfil[]) => {
      this.perfiles = res;
    }, err => { });
  }

  getUsuarios() {
    this.spinner.show();
    this.securityService.getUsuarioByPage(this.paginador).subscribe((res: Usuarios[]) => {
      this.usuarios = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron usuarios.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => {console.log(err); this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los usuarios', '', true); });
  }

  onChangeSexo(event: any, tipo: string) {
    if (tipo == 'M') {
      this.radioCheckedM = event.target.checked;
      this.radioCheckedF = !event.target.checked;
    } else if (tipo == 'F') {
      this.radioCheckedF = event.target.checked;
      this.radioCheckedM = !event.target.checked;
    }
  }

  onBlurEmail() {
    if (this.usuario.email != null && this.usuario.email != "") {
      this.usuario.email = this.usuario.email.toString().toLowerCase();
    }
  }

  openModal(content, usuario: Usuarios) {
    if (usuario != null) {
      //obtiene banner de bd
      this.spinner.show();

      this.securityService.getUsuarioId(usuario.idUsuario).subscribe((res: Usuarios) => {
        this.usuario = res;


        const perfil = this.perfiles.find(x => x.idPerfil == res.idPerfil);
        if (perfil) {
          this.selectedPerfil = perfil;
        }

        this.password1 = this.usuario.password;
        this.password2 = this.usuario.password;

        this.spinner.hide();
        this.modalTitle = 'Editar Usuario';
        //this.datepicker.navigateTo({year: 2018, month: 2});
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener usuario.', '', true); });

    } else {
      this.usuario = new Usuarios();
      this.modalTitle = 'Nuevo Usuario';
    }

    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save(content: any) {
    if (this.password1 != this.password2) {
      this.notificationService.warning('Contraseñas no coinciden.', '', true);
      return;
    }

    this.usuario.idPerfil = this.selectedPerfil.idPerfil;
    this.usuario.password = this.password1;

    if (this.usuario.idUsuario == 0) {

      this.spinner.show();

      this.securityService.saveUsuario(this.usuario).subscribe((res: any) => {
        if (res == -1) {
          this.notificationService.warning('Ya existe un usuario con el correo.', '', true);
        } else if (res == 0) {
          this.notificationService.error('Error al enviar correo de notificación', '', true);
        } else {
          this.notificationService.success('Correcto', '', true);
          this.getUsuarios();
        }
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => {
        this.spinner.hide();
        if (err && err.error != null && err.error != "") {
          this.notificationService.error(err.error, '', true);
        } else {
          this.notificationService.error('Ocurrió un error al crear usuario.', '', true);
        }
      });

    } else {

      this.securityService.editUsuario(this.usuario).subscribe(res => {
        this.notificationService.success('Correcto', 'Correcto', true);
        this.getUsuarios();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar Usuario', '', true); });

    }
  }

  async delete(item: Usuarios) {
    const loguedUser = this.authService.getuser();
    if (loguedUser != null) {
      if (loguedUser.email == item.email) {
        this.notificationService.warning('No es posible eliminar el usuario en sesión.', '', true);
        return;
      } else {
        const response = await this.notificationService.confirmation('Eliminar Usuario', '¿Confirma eliminar este Usuario?');
        if (response.isConfirmed) {
          this.securityService.deleteUsuario(item.idUsuario).subscribe(res => {
            this.notificationService.success('Usuario eliminado correctamente', 'Correcto', true);
            this.getUsuarios();
          }, err => { this.notificationService.error('Ocurrió un error al eliminar el Usuario', '', true); });
        }
      }
    }

  }

  async changePasswordRequest(item: Usuarios, content) {
    const loguedUser = this.authService.getuser();
    if (loguedUser != null) {
      if (loguedUser.email == item.email) {
        this.notificationService.warning('No es posible cambiar la contraseña del usuario en ' + '\n' + 'sesión, podrá hacerlo desde el menu Mi cuenta.', '', true);
        return;
      }
    }

    this.usuario = item;
    if (item.cuentaActivada == null || item.cuentaActivada == 0) {

      // si el usuario ya tiene en proceso una activación de cuenta, se consulta si el administrador desea enviar correo nuevamente o cambiarle manualmente la contraseña.
      const response = await this.notificationService.confirmation('Cambiar contraseña',
        `El usuario seleccionado ya posee un cambio de contraseña pendiente, ¿Desea enviar nuevamente un correo o cambiar su contraseña manualmente?`,
        'Enviar correo', 'Cambiar manualmente');

      if (response.isConfirmed) {
        //Envia correo nuevamente
        this.sendChangePassMail(item);
      } else if (response.isDismissed && response.dismiss.toString() == "cancel") {
        //levanta modal para cambio de contraseña manual
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
      }

    }
    else if (item.cuentaActivada == 1) {
      this.sendChangePassMail(item);
    }
  }

  async sendChangePassMail(item: Usuarios) {
    const response = await this.notificationService.confirmation('Cambiar contraseña', `Se enviará un correo electrónico al correo: ${item.email}, desde donde el usuario deberá modificar su contraseña, la cuenta se deshabilitara hasta que se realice el cambio. ¿Desea contínuar?`);
    if (response.isConfirmed) {
      this.spinner.show();
      this.securityService.restableceContraseñaUsuario(item).subscribe((res: any) => {
        if (res == 0) {
          this.notificationService.error('Error al enviar Correo con nueva contraseña.', '', true);
        } else {
          this.notificationService.success('Contraseña restablecida correctamente.', '', true);
        }
        this.modalService.dismissAll();
        this.spinner.hide();
        this.getUsuarios();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al actualizar contraseña.', '', true); });
    }
  }

  changePassword() {
    if (this.password1 != this.password2) {
      this.notificationService.warning('Contraseñas no coinciden.', '', true);
      return;
    }

    this.spinner.show();
    this.usuario.password = this.password1;

    this.securityService.editUsuario(this.usuario).subscribe((res: any) => {
      if (res == 0) {
        this.notificationService.error('Error al enviar Correo con nueva contraseña.', '', true);
      } else {
        this.notificationService.success('Contraseña actualizada correctamente.', '', true);
      }
      this.modalService.dismissAll();
      this.spinner.hide();
      this.getUsuarios();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al actualizar contraseña.', '', true); });
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';

    this.getUsuarios()
  }

}
