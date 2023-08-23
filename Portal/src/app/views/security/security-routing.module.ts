import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccessComponent } from './access/access.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { ProfilesComponent } from './profiles/profiles.component';
import { UsersComponent } from './users/users.component';
import { AuthGuard } from 'src/app/shared/services/authguard';

const routes: Routes = [
    {
        path: 'access',
        component: AccessComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
    },
    {
        path: 'permissions',
        component: PermissionsComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
    },
    {
        path: 'profiles',
        component: ProfilesComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
        
    },
    {
        path: 'users',
        component: UsersComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
    }   
];



@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SecurityRoutingModule { }
