import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PaymentComponent } from './payment/payment.component';
import { TermsComponent } from './terms/terms.component';
import { ProfileComponent } from './profile/profile.component';
import { ShoppingComponent } from './shopping/shopping.component';
import { AccountsStateComponent } from './accounts-state/accounts-state.component';
import { SendCollectionsComponent } from './send-collections/send-collections.component';
import { ConfigComponent } from './config/config.component';
import { PaymentSettingsComponent } from './payment-settings/payment-settings.component';
import { SendAccessComponent } from './send-access/send-access.component';
import { CollectionsComponent } from './collections/collections.component';
import { ExcludedComponent } from './excluded/excluded.component';
import { AutomationComponent } from './automation/automation.component';
import { AuthGuard } from 'src/app/shared/services/authguard';


const routes: Routes = [
  {
    path: 'payment',
    component: PaymentComponent
  },
  {
    path: 'terms',
    component: TermsComponent
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'shopping',
    component: ShoppingComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'accounts-state',
    component: AccountsStateComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'accounts-state/:state',
    component: AccountsStateComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'send-collections',
    component: SendCollectionsComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  },
  {
    path: 'config',
    component: ConfigComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
  ,
  {
    path: 'payment-settings',
    component: PaymentSettingsComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
  ,
  {
    path: 'send-access',
    component: SendAccessComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
  ,
  {
    path: 'collections', 
    component: CollectionsComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
  ,
  {
    path: 'excluded', 
    component: ExcludedComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
  ,
  {
    path: 'automation', 
    component: AutomationComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PaymentRoutingModule { }
