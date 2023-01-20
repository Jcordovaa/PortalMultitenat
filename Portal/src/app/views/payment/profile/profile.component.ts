import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../shared/services/auth.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Utils } from '../../../shared/utils';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { Cliente } from '../../../shared/models/clientes.model';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { Router } from '@angular/router';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  public cliente: any = {
    rut: ''
  }

  public clienteChangeDatos :  any = {};

  changePass: any = {
    newPass1: '',
    newPass2: ''
  };

  selectedRegion: any = null;
  selectedCiudad: any = null;
  selectedComuna: any = null;
  selectedGiro:  any = null;
  selectedRazonSocial: any = null;
  selectedCorreo: any = null;
  selectedCorreoDTE: any = null;
  selectedDireccion: any = null;
  selectedNumDir: any = null;
  selectedTelefono: any = null;
  selectedRut: any = null;
  selectedCodAux:any = null;
  public regiones: any = [];
  public ciudades: any = [];
  public comunas: any = [];
  public comunasRes: any = [];
  public giros: any = [];
  public nombreCliente: string = '';

  configuracion: ConfiguracionPortal = new ConfiguracionPortal(); 

  loadingUpdate: boolean = false;
  loadingChangePass: boolean = false;
  loadingEstadoCuenta: boolean = false;
  loadingCompras: boolean = false;
  invalidRut: boolean = false;

  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno(); //FCA 06-07-2022  
  hoverBtnEdit : boolean = false;
  hoverBtnClave : boolean = false;
  hoverBtnEstado : boolean = false;

  correosDTE: any = [];

  constructor(private softlandService: ConfiguracionSoftlandService, private disenoSerivce: ConfiguracionDisenoService,//FCA 06-07-2022
              private configuracionService: ConfiguracionPagoClientesService,
              private authService: AuthService, 
              private clientesService: ClientesService, 
              private modalService: NgbModal,
              private spinner: NgxSpinnerService, 
              private utils: Utils, 
              private notificationService: NotificationService,
              private router: Router) {

    this.spinner.show();
    
    this.softlandService.getUbicaciones().subscribe(res => {
      this.regiones = res.regiones;     
      this.comunasRes = res.comunas;
      this.configuracionService.getConfigPortal().subscribe(res => {
        this.configuracion = res;

        this.getGiros();  
        this.getCorreosDTE();
      }, err => { this.spinner.hide(); });     
    }, err => { this.spinner.hide(); });     
  }

  getUserData() {
    this.spinner.show();
    var user = this.authService.getuser();
    if (user) {
      const data: any = {
        correo: user.email,
        rut: user.rut,
        codaux: user.codAux
      };

      this.clientesService.getClienteByMailAndRut(data).subscribe((res: any) => {
        debugger
        this.cliente = res;
        this.nombreCliente = this.cliente.nombre
        
        // const giro = this.giros.find(x => x.idGiro == res.giro.idGiro);

        // if (region) 
        //   this.selectedRegion = region;
        // if (comuna) 
        //   this.selectedComuna = comuna;
        // if (giro) 
        //   this.selectedGiro = giro;

          this.spinner.hide();

      }, err => { this.spinner.hide(); });
    }    
  }

  getGiros() {
    this.softlandService.getGirosSoftland().subscribe(res => {
      
      this.giros = res;
      this.getUserData();
    }, err => { this.spinner.hide(); });
  }

  getCorreosDTE() {
    var user = this.authService.getuser();
    this.clientesService.getCorreosDTE(user.rut).subscribe(res => {
      this.correosDTE = res;
    }, err => { this.spinner.hide(); });
  }

  validaRut() {
    if (this.utils.isValidRUT(this.cliente.rut)) {
      this.invalidRut = false;
    } else {
      this.invalidRut = true;
    }
  }

  ngOnInit(): void {
    this.getConfigDiseno(); //FCA 06-07-2022
  }

  async ActualizaCliente() {
    const response = await this.notificationService.confirmation('Actualizar datos', '¿Confirma actualizar los datos?'); 
    if (response.isConfirmed) {
      var user = this.authService.getuser();
      debugger
      this.clienteChangeDatos.codAux = this.selectedCodAux;
      this.clienteChangeDatos.rut = this.selectedRut;
      this.clienteChangeDatos.codGiro = this.selectedGiro;
      this.clienteChangeDatos.idRegion = this.selectedRegion;
      this.clienteChangeDatos.idCiudad = this.selectedCiudad;
      this.clienteChangeDatos.comCod = this.selectedComuna;
      this.clienteChangeDatos.correoUsuario = user.email;
      this.clienteChangeDatos.telefono = this.selectedTelefono;
      this.clienteChangeDatos.dirAux = this.selectedDireccion;
      this.clienteChangeDatos.dirNum = this.selectedNumDir;
      this.clienteChangeDatos.emailDTE = this.selectedCorreoDTE;
      this.clienteChangeDatos.correo = this.selectedCorreo;
      this.clienteChangeDatos.nombre = this.selectedRazonSocial;
      this.spinner.show(); 
      this.clientesService.actualizaClienteSoftland(this.clienteChangeDatos).subscribe(res => {
        this.modalService.dismissAll();    
        this.notificationService.success('Datos actualizados Correctamente', '', true); 
       
        this.getUserData();
      }, err => { this.spinner.hide();  this.loadingUpdate = false;  this.notificationService.error('Ocurrió un error al actualizar datos', '', true); });
    }
  }

  openModalChangePass(content) {
    this.changePass.newPass1 = '';
    this.changePass.newPass2 = '';   
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  openModalModificarDatos(content) {

    const comuna = this.comunasRes.filter(x => x.idRegion == this.cliente.idRegion);
    this.comunas = comuna;

    if(this.correosDTE.length > 0){
      let existeDTE = this.correosDTE.filter(x => x.mail == this.cliente.emailDTE);
      if(existeDTE.length == 0){
        let dte : any = {
            id: 0,
            rut: this.cliente.rut,
            correo: this.cliente.emailDTE
        };
  
        this.correosDTE.unshift(dte);
      }
    }
   
    
    this.selectedComuna = this.cliente.comCod;
    this.selectedGiro = this.cliente.codGiro;
    this.selectedRegion = this.cliente.idRegion
    this.selectedCorreo = this.cliente.correo;
    this.selectedCorreoDTE = this.cliente.emailDTE;
    this.selectedRazonSocial = this.cliente.nombre;
    this.selectedDireccion = this.cliente.dirAux;
    this.selectedNumDir = this.cliente.dirNum;
    this.selectedTelefono = this.cliente.telefono;
    this.selectedRut = this.cliente.rut;
    debugger
    this.selectedCodAux = this.cliente.codAux;

    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  onChangeRegion(value: any)
  {
    if (this.selectedRegion != null || this.selectedRegion != 0)
      {
        this.selectedComuna = "";
        this.comunas = [];
        const comuna = this.comunasRes.filter(x => x.idRegion == value.idRegion);
    this.comunas = comuna;
      }
  }

  async onChangePass() {
    if (this.changePass.newPass1 != this.changePass.newPass2) {
      this.notificationService.warning('Claves no coinciden.' ,'' , true); 
      return;
    }

    const response = await this.notificationService.confirmation('Cambiar Clave', 'Al cambiar su clave usted será redirigido al inicio de sesión, ¿Confirma actualizar su clave?'); 
    if (response.isConfirmed) {
      this.loadingChangePass = true;
      
      this.spinner.show();

      const data: any = {
        idCliente: this.cliente.idCliente,
        clave: this.changePass.newPass1
      };

      this.clientesService.changePassword(data).subscribe(res => {
        this.spinner.hide();
        this.modalService.dismissAll();
        this.changePass.newPass1 = '';
        this.changePass.newPass2 = '';
        this.notificationService.success('Clave actualizada correctamente' ,'', true);

        setTimeout(() => {
          this.authService.signout();
        }, 1000);

      }, err => { 
        this.spinner.hide();
        if (err && err.error != null && err.error.message != "") {
          this.notificationService.error(err.error.message, '', true);
        } else {
          this.notificationService.error('Ocurrió un error al cambiar clave.' ,'' , true); 
        }  
      });

    }   
  }

  irEstadoCuenta()
  {
    this.router.navigate(['/payment/accounts-state']);
  }

  //FCA 06-07-2022
    //FCA 01-07-2022
    private getConfigDiseno() {
      this.disenoSerivce.getConfigDiseno().subscribe((res: ConfiguracionDiseno) => {
          this.configDiseno = res;
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener configuración', '', true); });
  }

}
