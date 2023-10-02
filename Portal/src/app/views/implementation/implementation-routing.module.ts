import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/shared/services/authguard';
import { CompanyComponent } from './company/company.component';

const routes: Routes = [
    {
        path: 'company',
        component: CompanyComponent,
        canActivate: [AuthGuard],
        data: { requiresImplementation: true }
    },

];



@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ImplementationRoutingModule { }
