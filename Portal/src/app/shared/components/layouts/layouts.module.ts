import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminLayoutSidebarCompactComponent } from './admin-layout-sidebar-compact/admin-layout-sidebar-compact.component';
import { AuthLayoutComponent } from './auth-layout/auth-layout.component';
import { AuthPayLayoutComponent } from './auth-pay-layout/auth-pay-layout.component';
import { BlankLayoutComponent } from './blank-layout/blank-layout.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule } from '@angular/router';
import { SharedPipesModule } from '../../pipes/shared-pipes.module';
import { SearchModule } from '../search/search.module';
import { SidebarCompactComponent } from './admin-layout-sidebar-compact/sidebar-compact/sidebar-compact.component';
import { HeaderSidebarCompactComponent } from './admin-layout-sidebar-compact/header-sidebar-compact/header-sidebar-compact.component';
import { FooterComponent } from '../footer/footer.component';
import { CustomizerComponent } from '../customizer/customizer.component';
import { SharedDirectivesModule } from '../../directives/shared-directives.module';
import { FormsModule } from '@angular/forms';

const components = [
    HeaderSidebarCompactComponent,
    SidebarCompactComponent,
    FooterComponent,
    CustomizerComponent,
    AdminLayoutSidebarCompactComponent,
    AuthLayoutComponent,
    AuthPayLayoutComponent,
    BlankLayoutComponent,
];

@NgModule({
  imports: [
    NgbModule,
    RouterModule,
    FormsModule,
    SearchModule,
    SharedPipesModule,
    SharedDirectivesModule,
    CommonModule
  ],
  declarations: components,
  exports: components
})
export class LayoutsModule { }
