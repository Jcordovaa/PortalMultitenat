import { SecurityRoutingModule } from './security-routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedPipesModule } from '../../shared/pipes/shared-pipes.module'


import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TagInputModule } from 'ngx-chips';

import { SharedComponentsModule } from 'src/app/shared/components/shared-components.module';
import { FormWizardModule } from 'src/app/shared/components/form-wizard/form-wizard.module';
import { AccessComponent } from './access/access.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { ProfilesComponent } from './profiles/profiles.component';
import { UsersComponent } from './users/users.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    NgxDatatableModule,
    NgbModule,
    SecurityRoutingModule,
    SharedPipesModule,
    SharedComponentsModule,
    FormWizardModule,
    NgSelectModule,
    TagInputModule
  ],
  declarations: [
    AccessComponent,
    PermissionsComponent,
    ProfilesComponent,
    UsersComponent
  ]
})
export class SecurityModule { }
