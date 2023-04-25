import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
// import { SecurityService } from '../../../shared/services/secutiry.service';

import { LocalStoreService } from "../../../shared/services/local-store.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { Router, RouteConfigLoadStart, ResolveStart, RouteConfigLoadEnd, ResolveEnd, ActivatedRoute } from '@angular/router';
import { Usuarios } from '../../../shared/models/security.model';
import { ClientesService } from 'src/app/shared/services/clientes.service';

@Component({
  selector: 'app-activate-account',
  templateUrl: './activate-account.component.html',
  styleUrls: ['./activate-account.component.scss'],
  animations: [SharedAnimations]
})
export class ActivateAccountComponent implements OnInit {

  public signInModel: any = {
    correo: '',
    password: ''
  };

  public newPassModel: any = {
    password1: '',
    password2: ''
  };

  public cliente: string = '';
  public idUsuario: number = 0;
  public mail: string = '';
  public step: number = 1;

  loading: boolean;
  loadingText: string;
  signinForm: FormGroup;
  newPassForm: FormGroup;

  constructor(
    private ns: NotificationService,
    private ls: LocalStoreService,
    private fb: FormBuilder,
    private securityService: ClientesService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { 
    this.activatedRoute.params.subscribe(params => {
      if (params['id'] != null) {
        this.cliente = atob(params['id']);
      }
    });
  }

  checkPasswords() { 
    let pass = this.newPassForm.get('password1').value;
    let confirmPass = this.newPassForm.get('password2').value;

    return pass === confirmPass ? null : { notSame: true }     
  }

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
        this.loadingText = 'Cargando Dashboard ...';

        this.loading = true;
      }
      if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
        this.loading = false;
      }
    });

    this.signinForm = new FormGroup({
      'correo': new FormControl(this.signInModel.correo, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
      'pass': new FormControl(this.signInModel.password, [Validators.required, Validators.minLength(6)])
    });

    this.newPassForm = new FormGroup({
      'pass2': new FormControl(this.newPassModel.password1, [Validators.required, Validators.minLength(6)]),
      'pass3': new FormControl(this.newPassModel.password2, [Validators.required, Validators.minLength(6)])
    });
  }

  get correo() { return this.signinForm.get('correo'); }
  get pass() { return this.signinForm.get('pass'); }

  get pass2() { return this.newPassForm.get('pass2'); }
  get pass3() { return this.newPassForm.get('pass3'); }

  signin() {
    if (this.signinForm.invalid) {
      return
    }

    var datos = this.cliente.split(";");
    
    if (this.signinForm.get('correo').value != datos[1])
    {
      this.ns.warning('Correo ingresado no corresponde a la activación de esta cuenta' ,'' , true); 
      return
    }

    const model = { codAux: datos[0], correo: datos[1], clave: this.signinForm.get('pass').value}


    this.loading = true;
    this.loadingText = 'Validando...';
    this.securityService.canActivateAccount(model)
      .subscribe(res => {
        this.mail = model.correo;        
        this.loading = false;
        this.step = 2;
      }, err => {
        this.loading = false;
        if (err && err.error != null && err.error != "") {
          this.ns.error(err.error.message, '', true);
        } else {
          this.ns.error('Ocurrió un error al validar cuenta.' ,'' , true); 
        }  
      });
  }

  activateAccount() {
    if (this.newPassForm.invalid) {
      return
    }

    var datos = this.cliente.split(";");

    if (this.newPassForm.get('pass2').value != this.newPassForm.get('pass3').value) {
      this.ns.warning('Contraseñas no coinciden.' ,'' , true); 
      return
    }
    var datos = this.cliente.split(";");
    var email = this.mail;
    const model = { codAux: datos[0], correo: email, clave: this.newPassForm.get('pass2').value }


    this.loading = true;
    this.loadingText = 'Activando cuenta...';
    this.securityService.activateAccount(model)
      .subscribe(res => {

        this.ns.success('Cuenta activada correctamente.', '', true);
        this.loading = false;        
        this.router.navigate(['/sessions/signin']);

      }, err => {
        this.loading = false;
        if (err && err.error != null && err.error != "") {
          this.ns.error(err.error, '', true);
        } else {
          this.ns.error('Ocurrió un error al validar cuenta.' ,'' , true); 
        }  
      });
  }

}
