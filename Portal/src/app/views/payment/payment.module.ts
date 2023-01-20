import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PaymentRoutingModule } from './payment-routing.module';
import { PaymentComponent } from './payment/payment.component';
import { TermsComponent } from './terms/terms.component';
import { SharedModule } from '../../shared/shared.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'src/app/shared/components/shared-components.module';
import { ProfileComponent } from './profile/profile.component';
import { AccountsStateComponent } from './accounts-state/accounts-state.component';
import { ShoppingComponent } from './shopping/shopping.component';
import { SharedPipesModule } from '../../shared/pipes/shared-pipes.module'
import { NgxEchartsModule } from 'ngx-echarts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgxPaginationModule } from 'ngx-pagination';
import { SendCollectionsComponent } from './send-collections/send-collections.component';
import { ConfigComponent } from './config/config.component';
import { TagInputModule } from 'ngx-chips';
import { NgSelectModule } from '@ng-select/ng-select';
import { PaymentSettingsComponent } from './payment-settings/payment-settings.component';
import { SendAccessComponent } from './send-access/send-access.component';
import { MontoPipe } from 'src/app/shared/pipes/monto.pipe';
import { CollectionsComponent } from './collections/collections.component';
import { FormWizardModule } from 'src/app/shared/components/form-wizard/form-wizard.module';
import { ExcludedComponent } from './excluded/excluded.component';
import { AutomationComponent } from './automation/automation.component';

// import { CookieService } from 'angular2-cookie/services/cookies.service';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedComponentsModule,
    PaymentRoutingModule,
    NgbModule,
    NgxDatatableModule,
    NgxPaginationModule,
    SharedPipesModule,
    NgxEchartsModule,
    TagInputModule,
    NgSelectModule  ,
    FormWizardModule  
  ],
  declarations: [PaymentComponent, 
                 TermsComponent, 
                 ProfileComponent, 
                 AccountsStateComponent, 
                 ShoppingComponent, 
                 SendCollectionsComponent, 
                 ConfigComponent,
                 PaymentSettingsComponent,
                 SendAccessComponent,
                CollectionsComponent,
                AutomationComponent,
                ExcludedComponent],
   providers: [ MontoPipe ]
})
export class PaymentModule { }
