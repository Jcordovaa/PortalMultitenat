import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboadDefaultComponent } from './dashboad-default/dashboad-default.component';
import { DashboardAdministradorComponent } from './dashboard-administrador/dashboard-administrador.component';
import { DashboardClienteComponent } from './dashboard-cliente/dashboard-cliente.component';
import { DashboardV2Component } from './dashboard-v2/dashboard-v2.component';
import { DashboardV3Component } from './dashboard-v3/dashboard-v3.component';
import { DashboardV4Component } from './dashboard-v4/dashboard-v4.component';
import { AuthGuard } from 'src/app/shared/services/authguard';


const routes: Routes = [
  {
    path: 'cliente',
    component: DashboardClienteComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'administrador',
    component: DashboardAdministradorComponent,
    canActivate: [AuthGuard],
    data: { requiresAdmin: true }
  },
  {
    path: 'v3',
    component: DashboardV3Component
  },
  {
    path: 'v4',
    component: DashboardV4Component
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
