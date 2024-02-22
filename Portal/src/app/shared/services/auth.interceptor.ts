import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable } from "rxjs";
import { catchError } from "rxjs/operators";
import { AuthService } from "./auth.service";
import { throwError } from 'rxjs';
import { NotificationService } from "./notificacion.service";
import { NgxSpinnerService } from "ngx-spinner";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { LocalStoreService } from "./local-store.service";
import Swal from "sweetalert2";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private sessionExpiredShownSubject = new BehaviorSubject<boolean>(false);
  constructor(
    private _router: Router, private authservice: AuthService, private spinner: NgxSpinnerService, private modalService: NgbModal, private ls: LocalStoreService
  ) { }

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(httpRequest).pipe(
      catchError((error) => {

        debugger
        if (error.status == 401) {
          if (!this.sessionExpiredShownSubject.value) {
            this.sessionExpiredShownSubject.next(true);
            this.modalService.dismissAll();
            this.spinner.hide();
            Swal.fire({
              title: 'Sesión expirada',
              text: 'Será redirigido al inicio de sesión en unos segundos.',
              icon: 'warning',
              timer: 5000,
              timerProgressBar: true,
              backdrop: true,
              showConfirmButton: false,
              onClose: () => {
                this.sessionExpiredShownSubject.next(false);
                this.authservice.signout();
              }
            });
          } 
        }else {
          this.spinner.hide();
          return throwError(error);
        }
      })
    )
  }
}
