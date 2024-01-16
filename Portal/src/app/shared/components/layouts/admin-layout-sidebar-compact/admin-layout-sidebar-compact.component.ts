import { Component, OnInit, ViewChild } from '@angular/core';
import { NavigationService } from '../../../services/navigation.service';
import { SearchService } from 'src/app/shared/services/search.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router, RouteConfigLoadStart, ResolveStart, RouteConfigLoadEnd, ResolveEnd } from '@angular/router';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';

@Component({
  selector: 'app-admin-layout-sidebar-compact',
  templateUrl: './admin-layout-sidebar-compact.component.html',
  styleUrls: ['./admin-layout-sidebar-compact.component.scss']
})
export class AdminLayoutSidebarCompactComponent implements OnInit {
    moduleLoading: boolean;
    configuracion: ConfiguracionPortal = new ConfiguracionPortal();
    constructor(
      public navService: NavigationService,
      public searchService: SearchService, private configuracionService: ConfiguracionPagoClientesService,
      private router: Router
    ) { }
  
    ngOnInit() {
      this.router.events.subscribe(event => {
        if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
          this.moduleLoading = true;
        }
        if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
          this.moduleLoading = false;
        }
      });

      const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
      if (configuracionCompletaPortal != null) {
        this.configuracion = configuracionCompletaPortal.configuracionPortal;
      }
    }

}
