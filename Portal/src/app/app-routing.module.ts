import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthLayoutComponent } from './shared/components/layouts/auth-layout/auth-layout.component';
import { AuthPayLayoutComponent } from './shared/components/layouts/auth-pay-layout/auth-pay-layout.component';
import { BlankLayoutComponent } from './shared/components/layouts/blank-layout/blank-layout.component';
import { AdminLayoutSidebarCompactComponent } from './shared/components/layouts/admin-layout-sidebar-compact/admin-layout-sidebar-compact.component';
import { CanActivateViaAuthGuard } from './shared/guards/auth.route.guard';
import { CanActivateViaAuthPortalGuard } from './shared/guards/auth.portal.route.guard';
import { LoginGuard } from './shared/guards/login.guard';
import { PaymentGuard } from './shared/guards/payment.guard';
import { initType } from './app.constants';

const adminRoutes: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./views/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'uikits',
    loadChildren: () => import('./views/ui-kits/ui-kits.module').then(m => m.UiKitsModule)
  },
  {
    path: 'forms',
    loadChildren: () => import('./views/forms/forms.module').then(m => m.AppFormsModule)
  },
  {
    path: 'invoice',
    loadChildren: () => import('./views/invoice/invoice.module').then(m => m.InvoiceModule)
  },
  {
    path: 'inbox',
    loadChildren: () => import('./views/inbox/inbox.module').then(m => m.InboxModule)
  },
  {
    path: 'calendar',
    loadChildren: () => import('./views/calendar/calendar.module').then(m => m.CalendarAppModule)
  },
  {
    path: 'chat',
    loadChildren: () => import('./views/chat/chat.module').then(m => m.ChatModule)
  },
  {
    path: 'contacts',
    loadChildren: () => import('./views/contacts/contacts.module').then(m => m.ContactsModule)
  },
  {
    path: 'tables',
    loadChildren: () => import('./views/data-tables/data-tables.module').then(m => m.DataTablesModule)
  },
  {
    path: 'pages',
    loadChildren: () => import('./views/pages/pages.module').then(m => m.PagesModule)
  },
  {
    path: 'implementation',
    loadChildren: () => import('./views/implementation/implementation.module').then(m => m.ImplementationModule)
  },
  {
    path: 'security',
    loadChildren: () => import('./views/security/security.module').then(m => m.SecurityModule)
  },
  {
    path: 'icons',
    loadChildren: () => import('./views/icons/icons.module').then(m => m.IconsModule)
  },
  {
    path: 'payment',
    loadChildren: () => import('./views/payment/payment.module').then(m => m.PaymentModule)
  }
];

const paymentRoutes: Routes = [
  {
    path: 'payment',
    loadChildren: () => import('./views/payment/payment.module').then(m => m.PaymentModule)
  }
];

const routes: Routes = initType() == "PAYMENT" ? [
  {
    path: '',
    redirectTo: 'sessions/paymentportal',
    pathMatch: 'full'
  },
  {
    path: '',
    component: AuthPayLayoutComponent,
    children: [
      {
        path: 'sessions',
        loadChildren: () => import('./views/sessions/sessions.module').then(m => m.SessionsModule),
        canActivate: [PaymentGuard]
      }
    ]
  },
  {
    path: '',
    component: AdminLayoutSidebarCompactComponent,
    canActivate: [CanActivateViaAuthPortalGuard],
    children: paymentRoutes
  },
  {
    path: '**',
    redirectTo: 'others/404'
  }
]
  :
  [
    {
      path: '',
      redirectTo: 'sessions/signin',
      pathMatch: 'full'
    },
    {
      path: '',
      component: AuthLayoutComponent,
      children: [
        {
          path: 'sessions',
          loadChildren: () => import('./views/sessions/sessions.module').then(m => m.SessionsModule),
          canActivate: [LoginGuard]
        }
      ]
    },
    {
      path: '',
      component: BlankLayoutComponent,
      children: [
        {
          path: 'others',
          loadChildren: () => import('./views/others/others.module').then(m => m.OthersModule)
        }
      ]
    },
    {
      path: '',
      component: AdminLayoutSidebarCompactComponent,
      canActivate: [CanActivateViaAuthGuard],
      children: adminRoutes
    },
    {
      path: '**',
      redirectTo: 'others/404'
    }

  ];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
