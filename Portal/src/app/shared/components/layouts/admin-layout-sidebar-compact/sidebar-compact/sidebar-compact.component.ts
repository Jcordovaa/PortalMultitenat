import { Component, OnInit, HostListener } from "@angular/core";
import {
  NavigationService,
  IMenuItem,
  IChildItem
} from "../../../../services/navigation.service";
import { Router, NavigationEnd } from "@angular/router";
import { filter } from "rxjs/operators";
import { Utils } from "../../../../utils";
import { SecurityService } from "../../../../services/secutiry.service";
import { AuthService } from "../../../../services/auth.service";
import { NgxSpinnerService } from "ngx-spinner";
import { initType } from '../../../../../app.constants';
import { ConfiguracionDiseno } from "src/app/shared/models/configuraciondiseno.model";
import { ConfiguracionDisenoService } from "src/app/shared/services/configuraciondiseno.service";

@Component({
  selector: "app-sidebar-compact",
  templateUrl: "./sidebar-compact.component.html",
  styleUrls: ["./sidebar-compact.component.scss"]
})
export class SidebarCompactComponent implements OnInit {
  selectedItem: IMenuItem;
  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();
  nav: IMenuItem[];
  defaultMenu: IMenuItem[] = [
        {   
            id: 1,
            name: 'Dashboard',
            description: '',
            type: 'dropDown',
            icon: 'i-Bar-Chart',
            sub: [              
                { id: 11, icon: 'i-Bar-Chart-3', name: 'Mi Dashboard', state: '/dashboard/administrador', type: 'link' }               
            ]
        },        
        {
          id: 2, 
          name: 'Portal Clientes',
          description: '',
          type: 'dropDown',
          icon: 'i-Business-ManWoman',
          sub: [
              { id: 7, icon: 'i-Bar-Chart-3', name: 'Mi Dashboard', state: '/dashboard/cliente', type: 'link' },
              //{ id: 4, icon: 'i-Male', name: 'Mi Perfil', state: '/payment/profile', type: 'link' },
              { id: 5, icon: 'i-Book', name: 'Estado de Cuenta / Paga tu cuenta', state: '/payment/accounts-state', type: 'link' },
              { id: 5, icon: 'i-Shopping-Cart', name: 'Mis Compras', state: '/payment/shopping', type: 'link' },   
                               
          ]
       },       
        {
            id: 3, 
            name: 'Administración',
            description: 'Portal',
            type: 'dropDown',
            icon: 'i-Management',
            sub: [
                { id: 11, icon: 'i-Male', name: 'Mi Perfil', state: '/pages/productos', type: 'link' },               
                { id: 8, icon: 'i-Security-Check', name: 'Accesos Cliente', state: '/payment/send-access', type: 'link' },
                { id: 9, icon: 'i-Gear', name: 'Configuración Portal', state: '/payment/config', type: 'link' },
                { id: 10, icon: 'i-Mail-Send', name: 'Configuración Correos', state: '/pages/correos', type: 'link' },
                { id: 13, icon: 'i-Mail-Send', name: 'Pasarelas de pago', state: '/payment/payment-settings', type: 'link' },
                { id: 18, icon: 'i-Lock-User', name: 'Perfiles', state: '/security/profiles', type: 'link' },
                { id: 19, icon: 'i-Security-Check', name: 'Permisos', state: '/security/permissions', type: 'link' },
                { id: 20, icon: 'i-MaleFemale', name: 'Usuarios', state: '/security/users', type: 'link' }
            ]
        }
        ,       
        {
            id: 14, 
            name: 'Cobranzas',
            description: '',
            type: 'dropDown',
            icon: 'i-Management',
            sub: [
              { id: 15, icon: 'i-Book', name: 'Cobranzas', state: '/payment/collections', type: 'link' } ,
              { id: 16, icon: 'i-Book', name: 'Clientes Excluidos', state: '/payment/excluded', type: 'link' } ,
              { id: 17, icon: 'i-Book', name: 'Automatizaciones', state: '/payment/automation', type: 'link' } 
            ]
        }
    ];

  paymentMenu: IMenuItem[] = [
    {   
        name: 'Portal de Pagos',
        description: '',
        type: 'dropDown',
        icon: 'i-Money-Bag',
        sub: [
            { icon: 'i-Home1', name: 'Inicio', state: '/payment/payment', type: 'link' },
            { icon: 'i-Male', name: 'Mi Perfil', state: '/payment/profile', type: 'link' },
            { icon: 'i-Book', name: 'Estado de Cuentas', state: '/payment/accounts-state', type: 'link' },
            { icon: 'i-Shopping-Cart', name: 'Compras', state: '/payment/shopping', type: 'link' },
            { icon: 'i-Mail-Send', name: 'Envío de Cobranza', state: '/payment/send-collections', type: 'link' },
            { icon: 'i-Gear', name: 'Configuración', state: '/payment/config', type: 'link' }
        ]
    }
  ];

  constructor(public router: Router, public navService: NavigationService, private spinner: NgxSpinnerService, private disenoSerivce: ConfiguracionDisenoService,
    private securityService: SecurityService, private authService: AuthService) {}

  ngOnInit() {
    this.updateSidebar();
    // CLOSE SIDENAV ON ROUTE CHANGE
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(routeChange => {
        this.closeChildNav();
        if (Utils.isMobile()) {
          this.navService.sidebarState.sidenavOpen = false;
        }
      });

      if (initType() == "PAYMENT") {
        this.nav = this.paymentMenu;
        this.setActiveFlag();
      } else {
        const user = this.authService.getuser();

        if (user != null) {
          
          const data: any = {
            
            rut: user.rut,
            email: user.email,
            password: '-',
            codAux: user.codaux,
            esUsuario: user.esUsuario
          };

         

          this.securityService.getPermisosByEmail(data).subscribe(res => {
            this.defaultMenu.forEach(menu => {

              if (menu.id != null) {
                let added = res.find(x => x.idAcceso == menu.id)
                if (added != null) {
                  menu.visible = true;
                } else {
                  menu.visible = false;
                }

                menu.sub.forEach(submenu => {
                  let added = res.find(x => x.idAcceso == submenu.id)
                  if (added != null) {
                    submenu.visible = true;
                  } else {
                    submenu.visible = false;
                  }
                });
              } else {
                menu.visible = true;
              }
            });

            let newMenu: any = [];

            this.defaultMenu.forEach(element => {
              if (element.visible) {              

                let sm = element.sub.filter(x => x.visible == true)
                if (sm && sm.length > 0) {
                  element.sub = Object.assign([], sm);
                } else {
                  if (element.id != null) {
                    element.sub = [];
                  }                
                }

                newMenu.push(element);
              }
            });

            this.nav = newMenu;
            this.setActiveFlag();
           
            
          }, err => { this.spinner.hide(); });
        }

      }    
      
  }

  selectItem(item) {
    this.navService.sidebarState.childnavOpen = true;
    this.selectedItem = item;
    this.setActiveMainItem(item);
  }

  closeChildNav() {
    this.navService.sidebarState.childnavOpen = false;
    this.setActiveFlag();
  }

  onClickChangeActiveFlag(item) {
    this.setActiveMainItem(item);
  }

  setActiveMainItem(item) {
    this.nav.forEach(item => {
      item.active = false;
    });
    item.active = true;
  }

  setActiveFlag() {
    if (window && window.location) {
      const activeRoute = window.location.hash || window.location.pathname;
      this.nav.forEach(item => {
        item.active = false;
        if (activeRoute.indexOf(item.state) !== -1) {
          this.selectedItem = item;
          item.active = true;
        }
        if (item.sub) {
          item.sub.forEach(subItem => {
            subItem.active = false;
            if (activeRoute.indexOf(subItem.state) !== -1) {
              this.selectedItem = item;
              item.active = true;
              // subItem.active = true;
              // debugger;
            }
            if (subItem.sub) {
              subItem.sub.forEach(subChildItem => {
                if (activeRoute.indexOf(subChildItem.state) !== -1) {
                  this.selectedItem = item;
                  item.active = true;
                  subItem.active = true;
                }
              });
            }
          });
        }
      });
    }
  }

  updateSidebar() {
    if (Utils.isMobile()) {
      this.navService.sidebarState.sidenavOpen = false;
      this.navService.sidebarState.childnavOpen = false;
    } else {
      this.navService.sidebarState.sidenavOpen = true;
    }
  }

  toggelSidebar() {
    const state = this.navService.sidebarState;
    state.sidenavOpen = !state.sidenavOpen;
    state.childnavOpen = !state.childnavOpen;
  }

  @HostListener("window:resize", ["$event"])
  onResize(event) {
    this.updateSidebar();
  }

  

}
