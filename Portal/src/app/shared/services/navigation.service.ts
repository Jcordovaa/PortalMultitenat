import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface IMenuItem {
    id?: number;
    title?: string;
    description?: string;
    type: string;       // Possible values: link/dropDown/extLink
    name?: string;      // Used as display text for item and title for separator type
    state?: string;     // Router state
    icon?: string;      // Material icon name
    tooltip?: string;   // Tooltip text
    disabled?: boolean; // If true, item will not be appeared in sidenav.
    sub?: IChildItem[]; // Dropdown items
    badges?: IBadge[];
    active?: boolean;
    visible?: boolean;
}
export interface IChildItem {
    id?: number;
    parentId?: string;
    type?: string;
    name: string;       // Display text
    state?: string;     // Router state
    icon?: string;
    sub?: IChildItem[];
    active?: boolean;
    visible?: boolean;
}

interface IBadge {
    color: string;      // primary/accent/warn/hex color codes(#fff000)
    value: string;      // Display text
}

interface ISidebarState {
    sidenavOpen?: boolean;
    childnavOpen?: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class NavigationService {
    public sidebarState: ISidebarState = {
        sidenavOpen: true,
        childnavOpen: false
    };
    selectedItem: IMenuItem;
    
    constructor() {
    }

    defaultMenu: IMenuItem[] = [
        {   
            name: 'Dashboard',
            description: '',
            type: 'dropDown',
            icon: 'i-Bar-Chart',
            sub: [
                { icon: 'i-Clock-3', name: 'Version 1', state: '/dashboard/cliente', type: 'link' },
                { icon: 'i-Clock-4', name: 'Version 2', state: '/dashboard/v2', type: 'link' },
                { icon: 'i-Over-Time', name: 'Version 3', state: '/dashboard/v3', type: 'link' },
                { icon: 'i-Clock', name: 'Version 4', state: '/dashboard/v4', type: 'link' },
            ]
        },
        {
            id: 1, 
            name: 'Administración',
            description: 'Mantenedores del sistema.',
            type: 'dropDown',
            icon: 'i-Management',
            sub: [
                { id: 5, icon: 'i-Landscape', name: 'Banners', state: '/pages/banners', type: 'link' },  
                { id: 6, icon: 'i-Files', name: 'Catálogos', state: '/pages/catalogos', type: 'link' },              
                { id: 7, icon: 'i-Receipt-4', name: 'Categorías', state: '/pages/categorias', type: 'link' },
                { id: 8, icon: 'i-Business-ManWoman', name: 'Clientes', state: '/pages/clientes', type: 'link' },
                { id: 9, icon: 'i-Ticket', name: 'Cupones', state: '/pages/cupones', type: 'link' },
                { id: 10, icon: 'i-ID-3', name: 'Marcas', state: '/pages/marcas', type: 'link' },   
                { id: 11, icon: 'i-Big-Data', name: 'Productos', state: '/pages/productos', type: 'link' },
                { id: 12, icon: 'i-Shuffle1', name: 'Sinónimos', state: '/pages/sinonimos', type: 'link' },
                { id: 13, icon: 'i-Add-UserStar', name: 'Suscriptores', state: '/pages/suscriptores', type: 'link' }
            ]
        },
        {
            id: 24, 
            name: 'Cobranzas',
            description: 'Cobranzas del sistema.',
            type: 'dropDown',
            icon: 'i-Money-Bag',
            sub: [
                { id: 29, icon: 'i-Gear', name: 'Configuración de cobranza', state: '/pages/payment-config', type: 'link' },
                { id: 28, icon: 'i-Gear-2', name: 'Configuración de pago cliente', state: '/pages/payment-client-config', type: 'link' },
                { id: 27, icon: 'i-Mail-Send', name: 'Envío de cobranza', state: '/pages/send-payment', type: 'link' },
                { id: 26, icon: 'i-Financial', name: 'Estado de cuenta cliente', state: '/pages/account-state', type: 'link' },
                { id: 25, icon: 'i-Male', name: 'Ingreso pago clientes', state: '/pages/client-payment', type: 'link' }
            ]
        },
        {
            id: 2, 
            name: 'E-Commerce',
            description: 'Ventas E-Commerce',
            type: 'dropDown',
            icon: 'i-Car-Coins',
            sub: [
                { id: 14, icon: 'i-Car-Coins', name: 'Ventas', state: '/pages/ventas', type: 'link' },
                { id: 15, icon: 'i-Jeep', name: 'Despachos', state: '/pages/tipodespachos', type: 'link' },
                { id: 16, icon: 'i-Gear-2', name: 'Configuración', state: '/pages/config', type: 'link' },
                { id: 17, icon: 'i-Email', name: 'Correos', state: '/pages/correos', type: 'link' },
                { id: 18, icon: 'i-Car-Coins', name: 'Preguntas Frecuentes', state: '/pages/faq', type: 'link' },
            ]
        },
        {
            id: 3, 
            name: 'Softland',
            description: 'Configuración Softland',
            type: 'dropDown',
            icon: 'i-Gears',
            sub: [
                { id: 19, icon: 'i-Gear', name: 'Configuración', state: '/pages/softlandconfig', type: 'link' }
            ]
        },
        {
            id: 4, 
            name: 'Seguridad',
            description: 'Administración de seguridad',
            type: 'dropDown',
            icon: 'i-Lock-2',
            sub: [
                { id: 20, icon: 'i-Right', name: 'Accesos', state: '/security/access', type: 'link' },
                { id: 21, icon: 'i-Lock-User', name: 'Perfiles', state: '/security/profiles', type: 'link' },                
                { id: 22, icon: 'i-Security-Check', name: 'Permisos', state: '/security/permissions', type: 'link' },
                { id: 23, icon: 'i-MaleFemale', name: 'Usuarios', state: '/security/users', type: 'link' }
            ]
        }
    ];


    // sets iconMenu as default;
    menuItems = new BehaviorSubject<IMenuItem[]>(this.defaultMenu);
    // navigation component has subscribed to this Observable
    menuItems$ = this.menuItems.asObservable();

    // You can customize this method to supply different menu for
    // different user type.
    // publishNavigationChange(menuType: string) {
    //   switch (userType) {
    //     case 'admin':
    //       this.menuItems.next(this.adminMenu);
    //       break;
    //     case 'user':
    //       this.menuItems.next(this.userMenu);
    //       break;
    //     default:
    //       this.menuItems.next(this.defaultMenu);
    //   }
    // }
}
