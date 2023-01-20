import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivate } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class CanActivateViaAuthPortalGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate() {
        if (!this.authService.isLoggedPortal()) {
            this.router.navigate(['/sessions/paymentportal']);
            return false;
        }

        return true;
    }
}