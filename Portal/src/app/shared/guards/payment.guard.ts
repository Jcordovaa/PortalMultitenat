import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CanActivate } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class PaymentGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        if (state.url && state.url.indexOf('codAux') > -1) {
            if (this.authService.isLoggedPortal()) {
                this.authService.signoutPayment(false);
                return true;
            }
        }

        if (this.authService.isLoggedPortal()) {
            this.router.navigate(['/payment/payment']);
            return false;
        }

        return true;

        // if (state.url == "/") {
        //     this.router.navigate(['/payment/payment']);
        //     return false;
        // }

        // return true;
    }
}