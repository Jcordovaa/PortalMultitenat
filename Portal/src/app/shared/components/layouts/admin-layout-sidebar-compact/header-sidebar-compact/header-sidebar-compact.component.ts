import { Component, OnInit } from "@angular/core";
import { NavigationService } from "src/app/shared/services/navigation.service";
import { SearchService } from "src/app/shared/services/search.service";
import { AuthService } from "src/app/shared/services/auth.service";
import { LocalStoreService } from "src/app/shared/services/local-store.service";
import { initType } from '../../../../../app.constants'
import { ConfiguracionEmpresaService } from "src/app/shared/services/configuracionempresa.service";

@Component({
  selector: "app-header-sidebar-compact",
  templateUrl: "./header-sidebar-compact.component.html",
  styleUrls: ["./header-sidebar-compact.component.scss"]
})
export class HeaderSidebarCompactComponent implements OnInit {
  notifications: any[];
  userName: string = '';
  showBar: boolean = initType() == "PAYMENT" ? false : true;
  user : any = null;
  empresa: any = {
    nombreEmpresa: ''
  };

  constructor(
    private navService: NavigationService, private configuracionEmpresaService :ConfiguracionEmpresaService,
    public searchService: SearchService,
    private auth: AuthService, 
    private localStoreService: LocalStoreService
  ) {
    this.notifications = [
      {
        icon: "i-Speach-Bubble-6",
        title: "New message",
        badge: "3",
        text: "James: Hey! are you busy?",
        time: new Date(),
        status: "primary",
        link: "/chat"
      },
      {
        icon: "i-Receipt-3",
        title: "New order received",
        badge: "$4036",
        text: "1 Headphone, 3 iPhone x",
        time: new Date("11/11/2018"),
        status: "success",
        link: "/tables/full"
      },
      {
        icon: "i-Empty-Box",
        title: "Product out of stock",
        text: "Headphone E67, R98, XL90, Q77",
        time: new Date("11/10/2018"),
        status: "danger",
        link: "/tables/list"
      },
      {
        icon: "i-Data-Power",
        title: "Server up!",
        text: "Server rebooted successfully",
        time: new Date("11/08/2018"),
        status: "success",
        link: "/dashboard/v2"
      },
      {
        icon: "i-Data-Block",
        title: "Server down!",
        badge: "Resolved",
        text: "Region 1: Server crashed!",
        time: new Date("11/06/2018"),
        status: "danger",
        link: "/dashboard/v3"
      }
    ];
    
    
  
  }

  ngOnInit() {
    //   if (this.user != null) {
    //    if(this.user.esUsuario == true){
    //        window.location.href="#/dashboard/administrador";
    //   }else{
    //        window.location.href="#/dashboard/cliente";
    //   }
    // }
    this.user = this.localStoreService.getItem('currentUserPortal')
    if ( this.user) {
      this.userName =  this.user.nombre;
      
    }
    if(this.user.esUsuario == true){
      this.configuracionEmpresaService.getConfig().subscribe(res => {
        this.empresa = res;
      }, err => {  });
    }else{
      const user = this.auth.getuser();
      this.userName =  this.user.nombre;
      this.empresa.nombreEmpresa = user.nombre
      this.empresa.rutEmpresa = user.rut;
     
    }



  }

  toggelSidebar() {
    const state = this.navService.sidebarState;
    state.sidenavOpen = !state.sidenavOpen;
    state.childnavOpen = !state.childnavOpen;
  }

  signout() {
    if (initType() == "PAYMENT") {
      this.auth.signoutPayment();
    } else {
      this.auth.signout();
    }
    
  }

  perfil(){
    if (this.user != null) {
      if(this.user.esUsuario == true){
        window.location.href="#/pages/productos";
      }else{
        window.location.href="#/payment/profile";
      }
    }
  }
}
