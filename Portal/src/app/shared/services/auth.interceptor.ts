import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";
import { AuthService } from "./auth.service";
import { throwError } from 'rxjs';
import { NotificationService } from "./notificacion.service";
import { NgxSpinnerService } from "ngx-spinner";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private _router: Router, private authservice: AuthService, private spinner: NgxSpinnerService, private modalService: NgbModal
  ) { }

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(httpRequest).pipe(
      catchError((error) => {     
        if (error.status == 401) {
          this.modalService.dismissAll();
          this.spinner.hide();
          this.authservice.signoutExpiredToken()
           
        } else {
          this.spinner.hide();
          return throwError(error);
        }
      })
    );
  }
}