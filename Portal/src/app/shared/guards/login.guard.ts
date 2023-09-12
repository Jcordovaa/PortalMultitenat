import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CanActivate } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class LoginGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.authService.isLogged() && state.url.indexOf('activate-account') == -1 && state.url.indexOf('account-state-view') == -1  && state.url.indexOf('pay') == -1 ) {
            var user = this.authService.getuser();
            if(user){
                if(user.esUsuario === true){
                    this.router.navigate(['/dashboard/administrador']);
                }else{
                    this.router.navigate(['/dashboard/cliente']);
                }
                
                return false;
            }
          
        }

        return true;
    }
}