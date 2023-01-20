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
    component: ProfileComponent
  },
  {
    path: 'shopping',
    component: ShoppingComponent
  },
  {
    path: 'accounts-state',
    component: AccountsStateComponent
  },
  {
    path: 'accounts-state/:state',
    component: AccountsStateComponent
  },
  {
    path: 'send-collections',
    component: SendCollectionsComponent
  },
  {
    path: 'config',
    component: ConfigComponent
  }
  ,
  {
    path: 'payment-settings',
    component: PaymentSettingsComponent
  }
  ,
  {
    path: 'send-access',
    component: SendAccessComponent
  }
  ,
  {
    path: 'collections', 
    component: CollectionsComponent
  }
  ,
  {
    path: 'excluded', 
    component: ExcludedComponent
  }
  ,
  {
    path: 'automation', 
    component: AutomationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PaymentRoutingModule { }
