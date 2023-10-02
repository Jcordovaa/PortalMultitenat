import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { LocalStoreService } from "./local-store.service";
import { AuthService } from "./auth.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private localStoreService: LocalStoreService
    ) { }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean {
        debugger
        const currentRoute = state.url;
        const user = this.localStoreService.getItem('currentUserPortal');

        if (user) {
            const isAdmin = user.esUsuario === true; // Suponiendo que 0 es el valor para administradores
            const isImplementador = user.esImplementador === true;
            const requiresAdminPermission = next.data.requiresAdmin === true;
            const requiresImplementationPermission = next.data.requiresImplementation === true;

            if (requiresImplementationPermission && !isImplementador) {
                this.localStoreService.removeItem('currentUserPortal');

                this.router.navigateByUrl("/sessions/signin");
                return false;
            }

            if (requiresAdminPermission && !isAdmin) {
                // Si la ruta requiere permisos de administrador y el usuario no es administrador,
                // redirige a la página de inicio de sesión u otra página adecuada.
                this.localStoreService.removeItem('currentUserPortal');

                this.router.navigateByUrl("/sessions/signin");
                return false;
            }

            if (!requiresAdminPermission && isAdmin) {
                this.localStoreService.removeItem('currentUserPortal');

                this.router.navigateByUrl("/sessions/signin");
                return false;
            }

            // Si el usuario tiene permisos de administrador o la ruta no requiere permisos de administrador,
            // permite el acceso a la ruta.
            return true;
        } else {
            this.router.navigateByUrl('/sessions/signin');
            return false;
        }
    }
}
