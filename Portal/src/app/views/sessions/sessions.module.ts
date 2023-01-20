import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SessionsRoutingModule } from './sessions-routing.module';
import { SignupComponent } from './signup/signup.component';
import { SigninComponent } from './signin/signin.component';
import { ForgotComponent } from './forgot/forgot.component';
import { SharedModule } from '../../shared/shared.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'src/app/shared/components/shared-components.module';
import { ActivateAccountComponent } from './activate-account/activate-account.component';
import { PaymentPortalComponent } from './payment-portal/payment-portal.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedPipesModule } from '../../shared/pipes/shared-pipes.module'

import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgxPaginationModule } from 'ngx-pagination';
import { MontoPipe } from 'src/app/shared/pipes/monto.pipe';
import { PayComponent } from './fasy-payment/pay.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedComponentsModule,
    SessionsRoutingModule,
    NgbModule,
    NgxDatatableModule,
    NgxPaginationModule,
    SharedPipesModule
  ],
  declarations: [SignupComponent, SigninComponent, ForgotComponent, ActivateAccountComponent, PaymentPortalComponent, PayComponent],
  providers: [ MontoPipe ]
})
export class SessionsModule { }
