import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SignupComponent } from './signup/signup.component';
import { SigninComponent } from './signin/signin.component';
import { ForgotComponent } from './forgot/forgot.component';
import { ActivateAccountComponent } from './activate-account/activate-account.component';
import { PaymentPortalComponent } from './payment-portal/payment-portal.component';
import { PayComponent } from './fasy-payment/pay.component';

const routes: Routes = [
  {
    path: 'signup',
    component: SignupComponent
  },
  {
    path: 'signin',
    component: SigninComponent
  },
  {
    path: 'forgot',
    component: ForgotComponent
  },
  {
    path: 'activate-account',
    component: ActivateAccountComponent
  },
  { 
    path:'activate-account/:id', 
    component: ActivateAccountComponent 
  },
  {
    path: 'paymentportal',
    component: PaymentPortalComponent
  },
  {
    path: 'paymentportal/:state',
    component: PaymentPortalComponent
  },
  {
    path: 'paymentportal/:rut',
    component: PaymentPortalComponent
  },
  {
    path: 'pay/:rut/:numDoc/:idCobranza',
    component: PayComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SessionsRoutingModule { }
