import { PagesRoutingModule } from './pages-routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgxPaginationModule } from 'ngx-pagination';
import { SharedPipesModule } from '../../shared/pipes/shared-pipes.module'


import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { BannersComponent } from './banners/banners.component';
import { ProductosComponent } from './productos/productos.component';
import { CategoriasComponent } from './categorias/categorias.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TagInputModule } from 'ngx-chips';

import { SharedComponentsModule } from 'src/app/shared/components/shared-components.module';
import { FormWizardModule } from 'src/app/shared/components/form-wizard/form-wizard.module';
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
import { CobranzaConfigComponent } from './cobranza-config/cobranza-config.component';
import { CobranzaPagoClienteComponent } from './cobranza-pago-cliente/cobranza-pago-cliente.component';
import { EnvioCobranzaComponent } from './envio-cobranza/envio-cobranza.component';
import { Step1Component } from './ingreso-pago-clientes/wizard/step1/step1.component';
import { Step2Component } from './ingreso-pago-clientes/wizard/step2/step2.component';
import { Step3Component } from './ingreso-pago-clientes/wizard/step3/step3.component';
import { CurrencyMaskModule } from "ng2-currency-mask";
import { ColorPickerModule } from 'ngx-color-picker';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxPaginationModule,
    NgxDatatableModule,
    NgbModule,
    PagesRoutingModule,
    SharedPipesModule,
    SharedComponentsModule,
    FormWizardModule,
    NgSelectModule,
    TagInputModule,
    CurrencyMaskModule,
    ColorPickerModule
  ],
  declarations: [
    UserProfileComponent,
    BannersComponent,
    ProductosComponent,
    CategoriasComponent,
    ClientesComponent,
    SuscriptoresComponent,
    VentasComponent,
    CuponesComponent,
    SinonimosComponent,
    MarcasComponent,
    CatalogosComponent,
    FaqComponent,
    CorreosComponent,
    TipoDespachoComponent,
    SoftlandConfigComponent,
    FrontconfigComponent,
    IngresoPagoClientesComponent,
    AccountStateComponent,
    CobranzaConfigComponent,
    CobranzaPagoClienteComponent,
    EnvioCobranzaComponent,
    Step1Component,
    Step2Component,
    Step3Component
  ],
  exports: [ColorPickerModule]
})
export class PagesModule { }
