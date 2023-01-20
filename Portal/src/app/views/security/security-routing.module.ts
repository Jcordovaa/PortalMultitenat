import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccessComponent } from './access/access.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { ProfilesComponent } from './profiles/profiles.component';
import { UsersComponent } from './users/users.component';

const routes: Routes = [
    {
        path: 'access',
        component: AccessComponent
    },
    {
        path: 'permissions',
        component: PermissionsComponent
    },
    {
        path: 'profiles',
        component: ProfilesComponent
    },
    {
        path: 'users',
        component: UsersComponent
    }   
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SecurityRoutingModule { }
