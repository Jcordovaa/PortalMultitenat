import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { FormGroup, FormBuilder, Validators, FormControl, ValidationErrors, ValidatorFn} from '@angular/forms';
import { AuthService } from '../../../shared/services/auth.service';
import { LocalStoreService } from "../../../shared/services/local-store.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { MailOldService } from '../../../shared/services/mailold.service';
import { VentasService } from '../../../shared/services/ventas.service';
import { Utils } from '../../../shared/utils';
import { Cliente, ContactoCliente } from '../../../shared/models/clientes.model';
import { EnviaCobranza } from '../../../shared/models/enviacobranza.model';
import { TbkRedirect } from '../../../shared/enums/TbkRedirect';
import { NgxSpinnerService } from "ngx-spinner";
import { Router, RouteConfigLoadStart, ResolveStart, RouteConfigLoadEnd, ResolveEnd, ActivatedRoute } from '@angular/router';
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';

const I18N_VALUES = {
    en: {
        weekdays: ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'],
        months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    },
    es: {
        weekdays: ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'],
        months: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
    }
  };

@Component({
    selector: 'app-payment-portal',
    templateUrl: './payment-portal.component.html',
    styleUrls: ['./payment-portal.component.scss'],
    animations: [SharedAnimations]
})
export class PaymentPortalComponent implements OnInit {

    public signInModel: any = {
        rutLogin: '',
        correo: '',
        password: ''
    };

    public contactModel: any = {
        nombre: '',
        mail: '',
        mensaje: ''
    };

    public paymentModel: any = {
        rut: ''
    };

    public paymentDataModel: any = {
        correoPay: '',
        telefonoPay: ''
    };

    public registerModel: any = {
        rutReg: '',
        razonSocialReg: '',
        correoReg: '',
        claveReg: '',
        nombreConReg: '',
        apellidoConReg: '',
        correoConReg: '',
        telefonoConReg: ''
    };

    loading: boolean;
    loadingPR: boolean;
    loadingFilter: boolean;
    loadingRegister: boolean;
    loadingText: string;
    loadingPRText: string = 'Buscando...';
    loadingFilterText: string = 'Buscando...';

    signinForm: FormGroup;
    contactForm: FormGroup;
    paymentForm: FormGroup;
    paymentDataForm: FormGroup;
    registerForm: FormGroup;

    compras: any = [];
    showTable: boolean = false;
    showRegistro: boolean = false;
    comprasResp: any = [];
    selected = [];
    multi = "multi";
    multiClick = "multiClick";
    selectionType = this.multiClick;
    total: string = "$ 0";
    totalPagar: number = 0;
    dateDesde: NgbDateStruct;
    dateHasta: NgbDateStruct;
    logTbk: any = null;
    cobranzaPagos: any = [];
    esCobranza: boolean = false;

    estados: any = [];
    tiposDocs: any = [];
    selectedEstado: any = null;
    selectedTipoDoc: any = null;
    folio: number = null;
    existeSoftland:boolean = false;

    selectedDosc: any = [];
    showModalPaymentResult: boolean = true;
    paymentResultState: number = 1;
    idCobranza: number = 1;
    checkAll: boolean = false;

    constructor(
        private ns: NotificationService,
        private ls: LocalStoreService,
        private fb: FormBuilder,
        private auth: AuthService,
        private router: Router,
        private clientesService: ClientesService,
        private ngbDatepickerConfig: NgbDatepickerConfig,
        private ngbDatepickerI18n: NgbDatepickerI18n,
        private ngbCalendar: NgbCalendar,
        private utils: Utils,
        private spinner: NgxSpinnerService,
        private modalService: NgbModal,
        private activatedRoute: ActivatedRoute,
        private mailOldService: MailOldService,
        private ventasService: VentasService
    ) {
        this.ngbDatepickerConfig.firstDayOfWeek = 1;
        this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
            return I18N_VALUES['es'].weekdays[weekday - 1];
        };
        this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
            return I18N_VALUES['es'].months[months - 1];
        };

        // this.dateDesde = this.ngbCalendar.getToday();
        // this.dateHasta = this.ngbCalendar.getToday();
    }

    ngOnInit() {
        this.router.events.subscribe(event => {
            if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
                this.loadingText = 'Cargando Dashboard ...';

                this.loading = true;
            }
            if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
                this.loading = false;
            }
        });

        this.signinForm = new FormGroup({
            'rutLogin': new FormControl(this.signInModel.rutLogin, [Validators.required, this.rutPaymentValidator]),
            'correo': new FormControl(this.signInModel.correo, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            'pass': new FormControl(this.signInModel.password, [Validators.required, Validators.minLength(6)])
        });

        this.contactForm = new FormGroup({
            'nombre': new FormControl(this.contactModel.nombre, [Validators.required]),
            'mail': new FormControl(this.contactModel.mail, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            'mensaje': new FormControl(this.contactModel.mensaje, [Validators.required])
        });

        this.paymentForm = new FormGroup({
            'rut': new FormControl(this.paymentModel.rut, [Validators.required, this.rutPaymentValidator])
        });

        this.paymentDataForm = new FormGroup({
            'correoPay': new FormControl(this.paymentDataModel.correoPay, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            'telefonoPay': new FormControl(this.paymentDataModel.telefonoPay, [Validators.required])
        });

        this.registerForm = new FormGroup({
            'rutReg': new FormControl(this.registerModel.rutReg, [Validators.required, this.rutPaymentValidator]),
            'razonSocialReg': new FormControl(this.registerModel.razonSocialReg, [Validators.required]),
            'correoReg': new FormControl(this.registerModel.correoReg, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            'claveReg': new FormControl(this.registerModel.claveReg, [Validators.required]),
            'nombreConReg': new FormControl(this.registerModel.nombreConReg, [Validators.required]),
            'apellidoConReg': new FormControl(this.registerModel.apellidoConReg, [Validators.required]),
            'correoConReg': new FormControl(this.registerModel.correoConReg, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]),
            'telefonoConReg': new FormControl(this.registerModel.telefonoConReg, [Validators.required])
        });

        this.activatedRoute.queryParams.subscribe(params => {
            if (params['state'] != null && params['idVenta']) {
      
              this.paymentResultState = parseInt(params['state']);
              this.showModalPaymentResult = true;
      
              this.ventasService.getLogTbk(params['idVenta']).subscribe(res => {
                this.logTbk = res;
                var btn = document.getElementById('btnModalPortal')
                btn.click();
              }, err => {
                this.logTbk = null;
                var btn = document.getElementById('btnModalPortal')
                btn.click();
              });
                    
            }

            if (params['id'] != null && params['codAux'] != null) {
                this.idCobranza = parseInt(params['id']);
                this.esCobranza = true;
                this.procesaCobranza(params['id'], params['codAux']);                
            } else {
                this.idCobranza = 0;
                this.esCobranza = false;
            }

            if (params['rut'] != null) {
                const rut: string[] = params['rut'].toString().split('-');
                const codAux: string = rut[0].replace('.', '').replace('.', '')
                this.pagoExpress(false, codAux, true);             
            }

        });
    }

    get rutLogin() { return this.signinForm.get('rutLogin'); }
    get correo() { return this.signinForm.get('correo'); }
    get pass() { return this.signinForm.get('pass'); }

    get nombre() { return this.contactForm.get('nombre'); }
    get mail() { return this.contactForm.get('mail'); }
    get mensaje() { return this.contactForm.get('mensaje'); }

    get rut() { return this.paymentForm.get('rut'); }

    get correoPay() { return this.paymentDataForm.get('correoPay'); }
    get telefonoPay() { return this.paymentDataForm.get('telefonoPay'); }

    get rutReg() { return this.registerForm.get('rutReg'); }
    get razonSocialReg() { return this.registerForm.get('razonSocialReg'); }
    get correoReg() { return this.registerForm.get('correoReg'); }
    get claveReg() { return this.registerForm.get('claveReg'); }
    get nombreConReg() { return this.registerForm.get('nombreConReg'); }
    get apellidoConReg() { return this.registerForm.get('apellidoConReg'); }
    get correoConReg() { return this.registerForm.get('correoConReg'); }
    get telefonoConReg() { return this.registerForm.get('telefonoConReg'); }

    async procesaCobranza(id: any, codAux: string) {
        this.cobranzaPagos = [];
        this.spinner.show();

        this.clientesService.getCobranza(id).subscribe((res: EnviaCobranza) => {

            //si estado != 1, cobranza ya fue procesada y debe redigir
            if (res.estado != 1) {
                this.spinner.hide();
                this.outCobranza();
            } else {
                this.cobranzaPagos = res;
                this.spinner.hide();
                this.pagoExpress(true, codAux);
            }
            
        }, err => { this.spinner.hide(); this.ns.error('Ocurrió un error al obtener información de la cobranza.', '', true); });
    }

    async outCobranza() {
        const response: any = await this.ns.warningAync('Información','Los documentos cobrados ya no se encuentran disponibles.');
        if (response.isConfirmed) {
            window.location.href = `${window.location.origin}/#/sessions/paymentportal`
        }        
    }

    formatRut(tipo: number) {
        let formattedRut: string = '';
        switch (tipo) {
            case 1:
                if (this.utils.isValidRUT(this.rutLogin.value)) {
                    formattedRut = this.utils.estandarizaRut(this.rutLogin.value);
                    this.signinForm.get('rutLogin').setValue(formattedRut);
                }   
            break;
            case 2:
                if (this.utils.isValidRUT(this.rut.value)) {
                    formattedRut = this.utils.estandarizaRut(this.rut.value);
                    this.paymentForm.get('rut').setValue(formattedRut);
                }   
            break;
        }
             
    }

    rutPaymentValidator(control: FormControl) { 
        let campo = control.value;

        if (campo.length < 8) { return {
            rutPayment: {
                parsedDomain: rut
            }
        } }

        if (campo.length > 0) {
            campo = campo.replace('-', '')
            campo = campo.replace(/\./g, '')
    
            var suma = 0;
            var caracteres = "1234567890kK";
            var contador = 0;
            for (var i = 0; i < campo.length; i++) {
                let u = campo.substring(i, i + 1);
                if (caracteres.indexOf(u) != -1)
                    contador++;
            }
            if (contador == 0) { return {
                rutPayment: {
                    parsedDomain: rut
                }
            }}
    
            var rut = campo.substring(0, campo.length - 1)
            var drut = campo.substring(campo.length - 1)
            var dvr = '0';
            var mul = 2;
            var res = 0;
            var dvi;
    
            for (i = rut.length - 1; i >= 0; i--) {
                suma = suma + rut.charAt(i) * mul
                if (mul == 7) mul = 2
                else mul++
            }
            res = suma % 11
            if (res == 1) dvr = 'k'
            else if (res == 0) dvr = '0'
            else {
                dvi = 11 - res
                dvr = dvi + ""
            }
            if (dvr != drut.toLowerCase()) { 
                return {
                    rutPayment: {
                        parsedDomain: rut
                    }
                }
            }
            else { return null; }
        }
        return null;     
    }  

    openRegistro() {
        this.showRegistro = true;
    }

    cancelRegister(withRedirect: boolean = true) {
        if (withRedirect) {
            this.showRegistro = false;
            this.registerModel.rut = '';
            this.registerForm.get('rutReg').setValue('');
        }
        
        this.registerModel = {
            razonSocialReg: '',
            correoReg: '',
            claveReg: '',
            nombreConReg: '',
            apellidoConReg: '',
            correoConReg: '',
            telefonoConReg: ''
        };        
        
        this.registerForm.get('razonSocialReg').setValue('');
        this.registerForm.get('correoReg').setValue('');
        this.registerForm.get('claveReg').setValue('');
        this.registerForm.get('nombreConReg').setValue('');
        this.registerForm.get('apellidoConReg').setValue('');
        this.registerForm.get('correoConReg').setValue('');
        this.registerForm.get('telefonoConReg').setValue('');

        this.registerForm.markAsPristine();
        this.registerForm.markAsUntouched();
        this.registerForm.updateValueAndValidity();
    }

    validaClienteSoftland() {        
        if (this.isValidRUT(this.rutReg.value)) {

            this.spinner.show();
            this.cancelRegister(false);
            this.existeSoftland = false;

            const rutEstandar: string = this.utils.checkRut(this.rutReg.value);
            const model: any = { Email: '', Rut: rutEstandar };

            this.registerForm.get('rutReg').setValue(rutEstandar);

            this.clientesService.getClientFromSoftland(model).subscribe((res: any) => {
                if (res.length > 0) {
                    
                    if (res[0].email != null)
                        this.registerForm.get('correoReg').setValue(res[0].email);
                    if (res[0].nombre != null)
                        this.registerForm.get('razonSocialReg').setValue(res[0].nombre);
                    if (res[0].razon_social != null)
                        this.registerForm.get('razonSocialReg').setValue(res[0].nombre);

                    this.spinner.hide();
                    this.existeSoftland = true;

                } else {
                    //consulto al SII
                    this.clientesService.getSII(model).subscribe((res: any) => {
                        this.registerForm.get('razonSocialReg').setValue(res.razon_social);
                        this.spinner.hide();
                    }, err => { this.spinner.hide(); });
                }
            }, err => { this.spinner.hide(); });
        }
    }

    register() {
        if (!this.existeSoftland) {
            this.ns.warning('Registro de cliente', 'No es posible registrar un cliente que no este registrado en Softland.', false);
            return;
        }

        if (this.registerForm.invalid) {
            return
        }  

        this.loadingRegister = true;

        const contactoCliente: ContactoCliente = {
            idContactoCliente: 0,
            rutCliente: this.rutReg.value,
            nombreContacto: this.nombreConReg.value,
            apellidoContacto: this.apellidoConReg.value,
            idCargo: null,
            telefono: this.telefonoConReg.value,
            correoContacto: this.correoConReg.value
        };

        const cliente: Cliente = {
            idCliente: 0,
            rut: this.rutReg.value,
            nombre: this.razonSocialReg.value,
            email: this.correoReg.value,
            estado: 1,
            claveAcceso: this.claveReg.value,
            fechaRegistro: new Date(),
            horaRegistro: `${new Date().getHours()}:${new Date().getMinutes()}`,
            contactoCliente: contactoCliente,
            esSoftland: 1
        };

        this.clientesService.newClientePortal(cliente).subscribe(res => {
            this.ns.success('Correcto.', '', true);
            this.showRegistro = false;
            this.router.navigateByUrl('/payment/payment');
            this.loadingRegister = false;
        }, err => {
            this.loadingRegister = false;
            if (err && err.error != null && err.error != "" && err.error.message != "") {
                this.ns.error(err.error.message, '', true);
            } else {
                this.ns.error('Ocurrió un error al guardar cliente.', '', true);
            }
        });
    }

    goToHome() {
        this.showRegistro = false;
        this.showTable = false;

        this.signinForm.get('rutLogin').setValue('');
        this.signinForm.get('correo').setValue('');
        this.signinForm.get('pass').setValue('');        

        this.signinForm.markAsPristine();
        this.signinForm.markAsUntouched();
        this.signinForm.updateValueAndValidity();

        this.paymentForm.get('rut').setValue('');

        this.paymentForm.markAsPristine();
        this.paymentForm.markAsUntouched();
        this.paymentForm.updateValueAndValidity();
    }

    signin() {
        //this.router.navigateByUrl('/payment/payment');
        if (this.signinForm.invalid) {
            return
        }  

        this.loading = true;
        this.loadingText = 'Iniciando sesión...';
        this.auth.signinPayment(this.signinForm.value)
            .subscribe(res => {
                
                this.ls.setItem("currentUserAdPortal", res);
                this.router.navigateByUrl('/payment/payment');
                this.loading = false;
            }, err => {
                this.loading = false;
                if (err && err.error != null && err.error != "") {
                    this.ns.error(err.error, '', true);
                } else {
                    this.ns.error('Inicio de sesión inválido, compruebe sus credenciales.', '', true);
                }
            });
    }

    enviar() {
        if (this.contactForm.invalid) {
            return
        }
    }

    pagoExpress(esCobranza: boolean = false, codAux: string = '', rutDesdeUrl: boolean = false) {
        if (!esCobranza) {
            if (!rutDesdeUrl) {
                if (this.paymentForm.invalid) {
                    return
                }
            }            
        }        

        let model = {};
        this.loadingPR = true;

        if (!esCobranza) {
            if (!rutDesdeUrl) {
                const rut: string[] = this.rut.value.split('-');
                const rut2: string = rut[0].replace('.', '').replace('.', '')
                model = { nombre: rut2 };
            } else {
                model = { nombre: codAux };
            }            
        } else {
            model = { nombre: codAux };
        }

        this.clientesService.getClienteEstadoComprasFromSoftland(model).subscribe((res: any) => {
            this.loadingPR = false;            
            
            //si es cobranza solo debe mostrar los dctos enviados como cobranza.
            if (esCobranza) {
                let documentosACobrar: any = [];

                const folios: string[] = this.cobranzaPagos.documentosACobrar.split(';')
                folios.forEach(element => {
                    if (element && element != ';' && element != '') {
                        documentosACobrar.push({ nro: element });
                    }                    
                });

                res.forEach(element => {
                    let add = documentosACobrar.find((x: any) => x.nro == element.nro);
                    if (add != null) {
                        this.compras.push(element);
                    }
                });

                this.selectedDosc = Object.assign([], this.compras);
                this.selected = Object.assign([], this.compras);

                let valor: number = 0;
                this.selected.forEach(element => {
                    valor += element.debe
                });
                this.total = `$ ${valor}`;
                this.totalPagar = valor;

                setTimeout(() => {
                    var inputs = document.getElementsByTagName("input");
                    for (let i: number = 0; i < inputs.length; i++) {
                        if(inputs[i].type == "checkbox") {
                            inputs[i].disabled = true;
                        }  
                    }
                }, 1000);

            } else {
                this.compras = res;
            }

            this.comprasResp = Object.assign([], this.compras);
            this.estados = [];
            this.tiposDocs = [];
            this.showTable = true;

            res.forEach((element, index) => {
                const ex1 = this.estados.find(x => x.nombre == element.estado);
                const ex2 = this.tiposDocs.find(x => x.nombre == element.documento);

                if (ex1 == null) {
                    this.estados.push({
                        id: index + 1,
                        nombre: element.estado
                    });
                }
                if (ex2 == null) {
                    this.tiposDocs.push({
                        id: index + 1,
                        nombre: element.documento
                    });
                }
            });           

            // this.selectedEstado = this.estados[0];
            // this.selectedTipoDoc = this.tiposDocs[0];

        }, err => { this.loadingPR = false; });
    }

    onSelect(val: any) {
        this.selectedDosc = val.selected;

        if (val.selected.length > 0) {
            let valor: number = 0;
            val.selected.forEach(element => {
                valor += element.debe
            });
            this.total = `$ ${valor}`;
            this.totalPagar = valor;
        } else {
            this.total = "$ 0";
            this.totalPagar = 0;
        }
    }

    isValidRUT (campo: any) {       
        if (campo.length == 0) { return false; }
            if (campo.length < 8) { return false; }

            campo = campo.replace('-', '')
            campo = campo.replace(/\./g, '')

            var suma = 0;
            var caracteres = "1234567890kK";
            var contador = 0;
            for (var i = 0; i < campo.length; i++) {
                let u = campo.substring(i, i + 1);
                if (caracteres.indexOf(u) != -1)
                    contador++;
            }
            if (contador == 0) { return false }

            var rut = campo.substring(0, campo.length - 1)
            var drut = campo.substring(campo.length - 1)
            var dvr = '0';
            var mul = 2;
            var res = 0;
            var dvi;

            for (i = rut.length - 1; i >= 0; i--) {
                suma = suma + rut.charAt(i) * mul
                if (mul == 7) mul = 2
                else mul++
            }
            res = suma % 11
            if (res == 1) dvr = 'k'
            else if (res == 0) dvr = '0'
            else {
                dvi = 11 - res
                dvr = dvi + ""
            }
            if (dvr != drut.toLowerCase()) { return false; }
            else { return true; }   
    }

    filter() {
        let data: any = Object.assign([], this.comprasResp);

        if (this.folio != null) {
            data = data.filter(x => x.nro == this.folio)
        }
        if (this.selectedEstado != null) {
            data = data.filter(x => x.estado == this.selectedEstado.nombre)
        }
        if (this.selectedTipoDoc != null) {
            data = data.filter(x => x.documento == this.selectedTipoDoc.nombre)
        }
        if (this.dateDesde !=  null) {
            const fDesde = new Date(this.dateDesde.year, this.dateHasta.month -1, this.dateDesde.day, 0, 0, 0);
            data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
        }
        if (this.dateHasta !=  null) {
            const fHasta = new Date(this.dateHasta.year, this.dateHasta.month -1, this.dateHasta.day, 23, 59, 59);
            data = data.filter(x => new Date(x.fechaEmision) <= fHasta)
        }

        this.compras = data;
    }

    clearFilters() {
        this.folio = null;
        this.selectedEstado = null;
        this.selectedTipoDoc = null;
        this.dateDesde = null;
        this.dateHasta = null;

        this.compras = this.comprasResp;
    }

    pagar() {
        if (this.totalPagar <= 0) {
            this.ns.warning('Debe seleccionar documentos a pagar.','', true)
            return
        }

        if (this.paymentDataForm.invalid) {
            this.ns.warning('Debe ingresar Correo y teléfono.','', true)
            return
        }

        let detalle = [];
        const codAux = this.rut.value.replace('.', '').replace('.', '').split('-')[0]

        for (let i = 0; i <= this.selectedDosc.length - 1; i++) {
            detalle.push({
                IdPago: 0,
                Documento: this.selectedDosc[i].documento,
                Folio: this.selectedDosc[i].nro,
                Monto: this.selectedDosc[i].saldo,
                TipoDoc: this.selectedDosc[i].tipoDoc,
            });
        }

        let pago = {
            IdPago: 0,
            CodAux: codAux,
            MontoTotal: this.totalPagar,
            Detalles: detalle,
            Email: this.correoPay.value
        };

        this.spinner.show();

        this.clientesService.getEstadoConexionSoftland().subscribe(
            resVal => {
                if (resVal) {
                    this.clientesService.postSavePago(pago).subscribe(
                        (res: any) => {
                            this.spinner.hide();
                            window.location.href = `${this.utils.Server}/pagotbk?idVenta=${res}&isDocumentPayment=1&redirectTo=${TbkRedirect.Portal}&idCobranza=${this.idCobranza}`;
                        },
                        err => { this.spinner.hide(); }
                    );
                } else {
                    this.spinner.hide(); this.ns.warning('No es posible realizar el pago en estos momentos, por favor vuelva a intentarlo más tarde.', '', true);
                }
            },
            err => { this.spinner.hide(); this.ns.error('Error al verificar conexión.', '', true); }
        );
    }

    volver() {
        this.showTable = false;
        
        this.signinForm.get('rutLogin').setValue('');
        this.signinForm.get('correo').setValue('');
        this.signinForm.get('pass').setValue('');        

        this.signinForm.markAsPristine();
        this.signinForm.markAsUntouched();
        this.signinForm.updateValueAndValidity();

        this.paymentForm.get('rut').setValue('');

        this.paymentForm.markAsPristine();
        this.paymentForm.markAsUntouched();
        this.paymentForm.updateValueAndValidity();
    }

    openModal(content) {
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
    }

    closeModal() {
        this.modalService.dismissAll();
        window.location.href = window.location.origin + '/#/sessions/paymentportal'
        //this.router.navigate(['/payment/accounts-state']);
    }

    sendContact() {
        if (this.contactForm.invalid) {
            return;
        }

        this.loading = true;

        const mail: any = {
            tipo: 6,
            nombres: '',
            asunto: '',
            mensaje: `${this.nombre.value}|${this.mail.value}|${this.mensaje.value}`
        };

        this.mailOldService.send(mail).subscribe(res => {
            this.loading = false;
            this.ns.success('Correcto', '', true);

            this.contactForm.get('nombre').setValue('');
            this.contactForm.get('mail').setValue('');    
            this.contactForm.get('mensaje').setValue('');       

            this.contactForm.markAsPristine();
            this.contactForm.markAsUntouched();
            this.contactForm.updateValueAndValidity();
        }, err => { this.loading = false; });
    }

    onSel(val: any, c: any) {
        const added = this.selectedDosc.find(x => x.nro == c.nro);
        if (added != null) {
            //remueve
            for (let i=0; i <= this.selectedDosc.length -1; i++) {
            if (this.selectedDosc[i].nro == c.nro) {
                this.selectedDosc.splice(i, 1);
                break;
            }
            }
        } else {
            this.selectedDosc.push(c);
        }

        let valor: number = 0; 

        this.selectedDosc.forEach(element => {
            valor += element.debe;  
        });

        this.total = `$ ${valor}`;
        this.totalPagar = valor;

        if (this.selectedDosc.length == 0) {
            this.checkAll = false;
        }
    }

    onSelAll(val: any) {   
        let valor: number = 0; 
        this.selectedDosc = [];

        this.compras.forEach(element => {
            element.checked = val.target.checked
            if (val.target.checked) {
            this.selectedDosc.push(element);
            valor += element.debe;        
            }
        });

        this.total = `$ ${valor}`;
        this.totalPagar = valor;
    }

}
