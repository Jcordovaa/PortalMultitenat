import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboadDefaultComponent } from './dashboad-default/dashboad-default.component';
import { NgxEchartsModule } from 'ngx-echarts';
import { SharedComponentsModule } from 'src/app/shared/components/shared-components.module';
import { DashboardV2Component } from './dashboard-v2/dashboard-v2.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { DashboardV3Component } from './dashboard-v3/dashboard-v3.component';
import { DashboardV4Component } from './dashboard-v4/dashboard-v4.component';
import { DashboardClienteComponent } from './dashboard-cliente/dashboard-cliente.component';
import { SharedPipesModule } from '../../shared/pipes/shared-pipes.module'
import { DashboardAdministradorComponent } from './dashboard-administrador/dashboard-administrador.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormsModule } from '@angular/forms';
import { TagInputModule } from 'ngx-chips';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxSpinnerModule } from 'ngx-spinner';


@NgModule({
  imports: [
    CommonModule,
    SharedComponentsModule,
    NgxEchartsModule,
    NgxDatatableModule,
    NgbModule,
    DashboardRoutingModule,
    SharedPipesModule,
    NgxPaginationModule,
    FormsModule,
    TagInputModule,
    NgSelectModule,
    NgxSpinnerModule
    
  ],
  declarations: [DashboadDefaultComponent,
     DashboardV2Component, 
     DashboardV3Component, 
     DashboardV4Component,
     DashboardClienteComponent,
    DashboardAdministradorComponent]
})
export class DashboardModule { }
