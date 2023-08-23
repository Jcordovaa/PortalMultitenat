import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { BannersComponent } from './banners/banners.component';
import { ProductosComponent } from './productos/productos.component';
import { CategoriasComponent } from './categorias/categorias.component';
import { ClientesComponent } from './clientes/clientes.component';
import { SuscriptoresComponent } from './suscriptores/suscriptores.component';
import { VentasComponent } from './ventas/ventas.component';
import { CuponesComponent } from './cupones/cupones.component';
import { SinonimosComponent } from './sinonimos/sinonimos.component';
import { MarcasComponent } from './marcas/marcas.component';
import { CatalogosComponent } from './catalogos/catalogos.component';
import { FaqComponent } from './faq/faq.component';
import { CorreosComponent } from './correos/correos.component';
import { TipoDespachoComponent } from './tipo-despacho/tipo-despacho.component';
import { SoftlandConfigComponent } from './softland-config/softland-config.component';
import { FrontconfigComponent } from './frontconfig/frontconfig.component';
import { IngresoPagoClientesComponent } from './ingreso-pago-clientes/ingreso-pago-clientes.component';
import { AccountStateComponent } from './account-state/account-state.component';
import { EnvioCobranzaComponent } from './envio-cobranza/envio-cobranza.component';
import { CobranzaConfigComponent } from './cobranza-config/cobranza-config.component';
import { CobranzaPagoClienteComponent } from './cobranza-pago-cliente/cobranza-pago-cliente.component';
import { AuthGuard } from 'src/app/shared/services/authguard';

const routes: Routes = [
    {
        path: 'profile',
        component: UserProfileComponent
    },
    {
        path: 'banners',
        component: BannersComponent
    },
    {
        path: 'productos',
        component: ProductosComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
    },
    {
        path: 'categorias',
        component: CategoriasComponent
    },
    {
        path: 'clientes',
        component: ClientesComponent
    },
    {
        path: 'suscriptores',
        component: SuscriptoresComponent
    },
    {
        path: 'ventas',
        component: VentasComponent
    },
    {
        path: 'cupones',
        component: CuponesComponent
    },
    {
        path: 'sinonimos',
        component: SinonimosComponent
    },
    {
        path: 'marcas',
        component: MarcasComponent
    },
    {
        path: 'catalogos',
        component: CatalogosComponent
    },
    {
        path: 'faq',
        component: FaqComponent
    },
    {
        path: 'correos',
        component: CorreosComponent,
        canActivate: [AuthGuard],
        data: { requiresAdmin: true }
    },
    {
        path: 'tipodespachos',
        component: TipoDespachoComponent
    },
    {
        path: 'softlandconfig',
        component: SoftlandConfigComponent
    },  
    {
        path: 'config',
        component: FrontconfigComponent
    },
    {
        path: 'client-payment',
        component: IngresoPagoClientesComponent
    },
    {
        path: 'account-state',
        component: AccountStateComponent
    },
    {
        path: 'send-payment',
        component: EnvioCobranzaComponent
    },
    {
        path: 'payment-config',
        component: CobranzaConfigComponent
    },
    {
        path: 'payment-client-config',
        component: CobranzaPagoClienteComponent
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
