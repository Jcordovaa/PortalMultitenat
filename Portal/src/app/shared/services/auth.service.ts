import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { LocalStoreService } from "./local-store.service";
import { Utils } from '../../shared/utils'
import { NotificationService } from "./notificacion.service";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  //Only for demo purpose
  authenticated = true;
  private apiUrl: string = '';

  constructor(private store: LocalStoreService, private utils: Utils, private http: HttpClient, private router: Router , private notificationService: NotificationService) {
    this.apiUrl = this.utils.ServerWithApiUrl + 'auth';
    this.checkAuth();
  }

  checkAuth() {
    // this.authenticated = this.store.getItem("demo_login_status");
  }

  getuser() {
    var user = localStorage.getItem('currentUserPortal');
    if (user != null) {
      
      return JSON.parse(user);
    } 
    return null
  }

  getUserPortal() {
    var user = localStorage.getItem('currentUserAdPortal');
    if (user != null) {
      return JSON.parse(user);
    } 
    return null
  }

  signin(credentials: any) {    
    const data = {
      rut: '0-0',
      email: credentials.correo,
      password: credentials.pass
    };

    const body = JSON.stringify(data);
    return this.http.post(this.apiUrl + '/authenticate', body, this.utils.getHeaders(false));
  }

  signinPayment(credentials: any) {    
    const data = {
      rut: credentials.rutLogin,
      email: credentials.correo,
      password: credentials.pass,
      token: '',
      nombre: '',
      codAux: '',
      esUsuario: true
    };

    const body = JSON.stringify(data);
    return this.http.post(this.apiUrl + '/authenticate', body, this.utils.getHeaders(false));
  }

  signout() {
    // this.store.removeItem('currentUserAd');
    this.store.removeItem('currentUserPortal');
    
    this.router.navigateByUrl("/sessions/signin");
  }

  signoutPayment(withRedirect: boolean = true) {
    this.store.removeItem('currentUserPortal');
    if (withRedirect) {
      this.router.navigateByUrl("/sessions/paymentportal");
    }    
  }

  isLogged(): boolean {
    var currentUserAd = this.store.getItem("currentUserPortal");
    if (currentUserAd) {
      return true
    }
     return false
  }

  isLoggedPortal(): boolean {
    var currentUserAdPortal = this.store.getItem("currentUserPortal");
    if (currentUserAdPortal) {
      return true
    }
     return false
  }

  async signoutExpiredToken(){
    const response = await this.notificationService.sesionExpiredMsg('Sesión Expirada', 'Sera redirigido al inicio de sesión');
    if (response.isConfirmed) {
      this.signout();
    }
   
  }


}
