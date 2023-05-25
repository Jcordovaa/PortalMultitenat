import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AuthService } from '../../../shared/services/auth.service';
import { LocalStoreService } from "../../../shared/services/local-store.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { Router, RouteConfigLoadStart, ResolveStart, RouteConfigLoadEnd, ResolveEnd } from '@angular/router';
import { Utils } from '../../../shared/utils';
import { NgxSpinnerService } from "ngx-spinner";
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { PasarelaPagoService } from 'src/app/shared/services/pasarelaPago.service';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { MontoPipe } from 'src/app/shared/pipes/monto.pipe';
import { TbkRedirect } from 'src/app/shared/enums/TbkRedirect';
import { Cliente } from 'src/app/shared/models/clientes.model';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';

@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss'],
    animations: [SharedAnimations]
})
export class SigninComponent implements OnInit {

    public signInModel: any = {
        rutLogin: '',
        correo: '',
        pass: ''
    };


    public muestraPago: boolean = false;
    public muestraInicio: boolean = false;
    public esPagoRapido: boolean = false;
    public rutPagoRapido: string = '';
    public numDocPagoRapido: string = '';
    public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();

    cliente: any;
    loading: boolean = false;
    compras: any = [];
    comprasResp: any = [];
    dateDesde: NgbDateStruct;
    dateHasta: NgbDateStruct;
    selected: any = [];
    tiposDocs: any = [];
    estados: any = [];
    selectedEstado: any = null;
    selectedTipoDoc: any = null;
    folio: number = null;
    total: string = "$ 0";
    totalPagar: number = 0;
    selectedDosc: any = [];
    showModalPaymentResult: boolean = true;
    paymentResultState: number = 1;
    logPasarela: any = null;
    pasarelas: any = [];
    selectedPasarela: number = 0;
    tipoFecha: number = 1;
    checkAll: boolean = false
    page = 1;
    pageSize = 2;
    collectionSize = 8;
    configuracion: ConfiguracionPortal = new ConfiguracionPortal();
    esPago: boolean = false;
    rutEncriptado: string = '';
    loadingScreen: boolean = false;


    constructor(
        private ns: NotificationService,
        private ls: LocalStoreService,
        private fb: FormBuilder,
        private auth: AuthService,
        private router: Router,
        private utils: Utils, private disenoSerivce: ConfiguracionDisenoService,
        private spinner: NgxSpinnerService,
        private modalService: NgbModal,
        private clientesService: ClientesService, private notificationService: NotificationService, private pasarelaService: PasarelaPagoService,
        private configuracionService: ConfiguracionPagoClientesService,
        private montoPipe: MontoPipe,
    ) { }

    ngOnInit() {

        this.spinner.show();
        this.configuracionService.getConfigPortal().subscribe(res => {

            this.configuracion = res;
            this.getConfigDiseno();
        }, err => { this.spinner.hide(); });
        // this.router.events.subscribe(event => {
        //     if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
        //         this.loadingText = 'Cargando Dashboard ...';

        //         this.loading = true;
        //     }
        //     if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
        //         this.loading = false;
        //     }
        // });       
    }


    private getConfigDiseno() {
        this.disenoSerivce.getConfigDiseno().subscribe((res: ConfiguracionDiseno) => {
            this.configDiseno = res;
            this.loadingScreen = true;
            this.spinner.hide();
        }, err => { this.spinner.hide(); });
    }

    signin() {
        // if (this.signinForm.invalid) {
        //     return
        // }

        // this.loading = true;
        // this.loadingText = 'Iniciando sesión...';
        // this.auth.signin(this.signinForm.value)
        //     .subscribe(res => {
        //         this.ls.setItem("currentUserAd", res);
        //         this.router.navigateByUrl('/dashboard/v1');
        //         this.loading = false;
        //     }, err => {
        //         this.loading = false;
        //         if (err && err.error != null && err.error != "") {
        //             this.ns.error(err.error, '', true);
        //         } else {
        //             this.ns.error('Inicio de sesión inválido, compruebe sus credenciales.','', true);
        //         }                
        //     });
    }

    iniciarSesion() {
        if (this.signInModel.rut == '') {
            this.ns.warning('Debe ingresar el Rut.', '', true);
            return;
        }
        if (this.signInModel.correo == '') {
            this.ns.warning('Debe ingresar el Correo.', '', true);
            return;
        }
        if (this.signInModel.pass == '') {
            this.ns.warning('Debe ingresar la clave de acceso.', '', true);
            return;
        }

        this.spinner.show();
        this.auth.signinPayment(this.signInModel)
            .subscribe((res: any) => {

                this.ls.setItem("currentUserPortal", res);

                if (res.esUsuario == false) {
                    this.configuracionService.getAllConfigPortal().subscribe((res2: any) => {
                        this.ls.setItem("configuracionCompletaPortal", res2);
                        this.router.navigateByUrl('/dashboard/cliente');
                      }, err => { this.spinner.hide(); });
                    //this.spinner.hide();
                } else {
                    this.configuracionService.getAllConfigPortal().subscribe((res2: any) => {
                        this.ls.setItem("configuracionCompletaPortal", res2);
                        this.router.navigateByUrl('/dashboard/administrador');
                      }, err => { this.spinner.hide(); });
                    
                    //this.spinner.hide();
                }

            }, err => {
                if (err && err.error != null && err.error != "") {
                    this.ns.error(err.error, '', true);
                    this.spinner.hide();
                } else {
                    this.ns.error('Inicio de sesión inválido, compruebe sus credenciales.', '', true);
                    this.spinner.hide();
                }
            });

    }




    cambioMenu(menu: number) {
        if (menu == 1) //Inicio sesión
        {
            this.muestraInicio = true;
            this.muestraPago = false;
        } else if (menu == 2) //Pago rapido
        {
            this.muestraPago = true;
            this.muestraInicio = false;
        } else if (menu == -1) //Volver 
        {
            this.muestraPago = false;
            this.muestraInicio = false;
        }

    }

    validaRut() {
        if (this.signInModel.rutLogin != '' && this.signInModel.rutLogin != null) {
            if (this.utils.isValidRUT(this.signInModel.rutLogin)) {
                this.signInModel.rutLogin = this.utils.checkRut(this.signInModel.rutLogin);
            } else {
                this.notificationService.warning('RUT invalido', '', true);
                this.signInModel.rutLogin = '';
            }
        }
    }

    validaRutPagoRapido() {
        if (this.rutPagoRapido != '' && this.rutPagoRapido != null) {
            if (this.utils.isValidRUT(this.rutPagoRapido)) {
                this.rutPagoRapido = this.utils.checkRut(this.rutPagoRapido);
                this.rutEncriptado = window.btoa(this.rutPagoRapido)
            } else {
                this.notificationService.warning('RUT invalido', '', true);
                this.rutPagoRapido = '';
                this.rutEncriptado = '';
            }
        }
    }

    pagar() {

        if (this.rutPagoRapido != '' && this.rutPagoRapido != null && this.numDocPagoRapido != null && this.numDocPagoRapido != '') {
            this.notificationService.warning('Debe ingresar solo rut o número de documento', '', true);
            this.numDocPagoRapido = '';
            this.rutPagoRapido = '';
            return;
        } else if ((this.rutPagoRapido == '' || this.rutPagoRapido == null) && (this.numDocPagoRapido == null || this.numDocPagoRapido == '')) {
            this.notificationService.warning('Debe ingresar rut o número de documento', '', true);
            return;
        }

        if (this.rutPagoRapido != '' && this.rutPagoRapido != null) {
            var codAux: string = '';
            if (this.rutPagoRapido != '' && this.rutPagoRapido != null) {
                let str = this.rutPagoRapido.split('');
                for (let a of str) {
                    if (a != '.') {
                        if (a == '-') {
                            break;
                        }
                        codAux = codAux + a;
                    }
                }
            }


            const data: any = {
                correo: '',
                rut: this.rutPagoRapido,
                codaux: ''
            };
            this.spinner.show();
            this.clientesService.getClienteByMailAndRut(data).subscribe((res: Cliente) => {

                this.cliente = res;

                if (this.cliente.rut != '' && this.cliente.rut != null) {
                    window.location.href = window.location.origin + '#/sessions/pay/' + this.rutEncriptado + '/0/0/0'
                } else {
                    this.notificationService.error('Cliente no existe', '', true);
                }
                this.spinner.hide();
            }, err => { this.spinner.hide(); });
        } else if (this.numDocPagoRapido != '' && this.numDocPagoRapido != null) {
            this.spinner.show();
            const model = { codAux: codAux, folio: this.numDocPagoRapido }
            this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any[]) => {

                if (res.length > 0) {
                    window.location.href = window.location.origin + '#/sessions/pay/0/' + this.numDocPagoRapido + '/0/0'
                } else {
                    this.notificationService.error('Documento no existe', '', true);
                }

                this.spinner.hide();
            }, err => { this.spinner.hide(); });
        }


    }


    recuperarContrasena() {
        this.router.navigateByUrl('/sessions/forgot');
    }


    onKeydownInicioSesion(event) {
        if (event.keyCode === 13) {
            this.iniciarSesion();
        }
    }

    onKeydownPagoRapido(event) {
        if (event.keyCode === 13) {
            this.pagar();
        }
    }
}
