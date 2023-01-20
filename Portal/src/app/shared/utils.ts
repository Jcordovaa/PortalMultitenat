import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NotificationService } from '../shared/services/notificacion.service';
import { ITipoApi } from '../shared/enums/TipoApi';

@Injectable({
    providedIn: 'root'
})
export class Utils {
    // public Server: string = 'https://localhost:7043';
    public Server: string = 'https://apitenat.intgra.cl';
    public ApiUrl: string = 'api/';
    public ServerWithApiUrl = this.Server + '/' + this.ApiUrl;

    constructor(private notificationService: NotificationService, private router: Router) {}

    static isMobile() {
        return window && window.matchMedia('(max-width: 767px)').matches;
    }

    static ngbDateToDate(ngbDate: { month, day, year }) {
        if (!ngbDate) {
            return null;
        }
        return new Date(`${ngbDate.month}/${ngbDate.day}/${ngbDate.year}`);
    }

    static dateToNgbDate(date: Date) {
        if (!date) {
            return null;
        }
        date = new Date(date);
        return { month: date.getMonth() + 1, day: date.getDate(), year: date.getFullYear() };
    }

    static scrollToTop(selector: string) {
        if (document) {
            const element = <HTMLElement>document.querySelector(selector);
            element.scrollTop = 0;
        }
    }

    static genId() {
        let text = '';
        const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        for (let i = 0; i < 5; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return text;
    }

    getHeaders(withAuth: boolean = true) {
        
        if (withAuth) {
            const httpOptions = {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.getToken()}`
                })
            };
            return httpOptions;
        } else {
            const httpOptions = {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'Access-Control-Allow-Origin' : '*'
                })
            };
            return httpOptions;
        } 
    }

    getHeadersType(withAuth: boolean = true) {
        
        if (withAuth) {
            const httpOptions = {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'charset': 'utf-8',
                    'Authorization': `Bearer ${this.getToken()}`
                }),
                responseType: 'text' as 'json'                
            };
            return httpOptions;
        } else {
            const httpOptions = {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                    'charset': 'utf-8',
                    'Access-Control-Allow-Origin' : '*'
                }),
                responseType: 'text' as 'json'

                
            };
            return httpOptions;
        } 
    }

    getToken(): string {
        
        var currentUserAd = localStorage.getItem('currentUserPortal');
        if (currentUserAd != null) {
            return JSON.parse(currentUserAd).token;
        }
        return '';
    }

    validateMail (email: string): boolean {
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
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

    estandarizaRut(rut: string) {
        //Valida si el rut tiene -
        var validaGuion = rut.search("-");

        if (validaGuion == -1) {
            rut = rut.substr(0, rut.length - 1) + "-" + rut.substr(rut.length - 1, 1);
        }

        rut = rut.replace(/\./g, '');
       
        var rut_final= "";

        var ultimo= "";
        var medio= "";
        var primero= ""; 

        var largo = rut.length;

        var inicial = rut.split('-');

        if (inicial[0].length == 7) {

            ultimo = inicial[0].substr(4);
            medio = inicial[0].substr(1, 3);
            primero = inicial[0].substr(0, 1);
            rut_final = primero + "." + medio + "." + ultimo + "-" + inicial[1].toString();
        }

        if (inicial[0].length == 8) {
            ultimo = inicial[0].substr(5);
            medio = inicial[0].substr(2, 3);
            primero = inicial[0].substr(0, 2);
            rut_final = primero + "." + medio + "." + ultimo + "-" + inicial[1].toString();
        }

        if (inicial[0].length == 9) {
            ultimo = inicial[0].substr(6);
            medio = inicial[0].substr(3, 3);
            primero = inicial[0].substr(0, 3);
            rut_final = primero + "." + medio + "." + ultimo + "-" + inicial[1].toString();
        }

        return rut_final;


    }

    checkRut(rut: string) {


        rut = this.estandarizaRut(rut);
        // Despejar Puntos
        var valor = rut.replace(/\./g, '');
        // Despejar Guión
        valor = valor.replace(/-/g, '');

        // Aislar Cuerpo y Dígito Verificador
        var cuerpo = valor.slice(0, -1);
        var dv = valor.slice(-1).toUpperCase();

        // Formatear RUN
        rut = cuerpo + '-' + dv

        // Si no cumple con el mínimo ej. (n.nnn.nnn)
        if (cuerpo.length < 7) {  return ""; }

        // Calcular Dígito Verificador
        var suma = 0;
        var multiplo = 2;
        var i = 0;
        var index = 0;
        // Para cada dígito del Cuerpo
        for (i = 1; i <= cuerpo.length; i++) {

            // Obtener su Producto con el Múltiplo Correspondiente
            index = multiplo * parseInt(valor.charAt(cuerpo.length - i))

            // Sumar al Contador General
            suma = suma + index;

            // Consolidar Múltiplo dentro del rango [2,7]
            if (multiplo < 7) { multiplo = multiplo + 1; } else { multiplo = 2; }

        }

        // Calcular Dígito Verificador en base al Módulo 11
        var dvEsperado = 11 - (suma % 11);

        // Casos Especiales (0 y K)
        dv = (dv == 'K') ? "10" : dv;
        dv = (dv == '0') ? "11" : dv;

        // Validar que el Cuerpo coincide con su Dígito Verificador
        if (dvEsperado.toString() != dv) {  return ""; }

        // Si todo sale bien, eliminar errores (decretar que es válido)
        return this.estandarizaRut(rut);
    }

    transformaDocumento64(base64: any, tipo: string)
    {
        let byteCharacters = atob(base64);
        let byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        let byteArray = new Uint8Array(byteNumbers);
        var tipo = (tipo == "XML")? 'application/xml;base64': (tipo == "PDF")? 'application/pdf;base64':'';
        let file = new Blob([byteArray], {type: tipo});
        let url = URL.createObjectURL(file);

        return url;
    }

    async handleErrors(err: any, tipo: ITipoApi) {
        if (err && err.status == 401) {
            //sin autentificacion
            const response = await this.notificationService.sesionExpiredMsg("Sin autorización", "Su sesión ha finalizado, usted será redirigido(a) al Inicio de Sesión.");
            if (response.isConfirmed) {
                localStorage.removeItem('currentUserAd');
                this.router.navigate(['/sessions/signin']);
            }
        } else {
            switch (tipo) {
                case ITipoApi.GET:
                    this.notificationService.error('Ocurrió un error al obtener la información.', '', true);
                    break;
                case ITipoApi.POST:
                    this.notificationService.error('Ocurrió un error al guardar.', '', true);
                    break;
                case ITipoApi.PUT:
                    this.notificationService.error('Ocurrió un error al actualizar datos.', '', true);
                    break;
                case ITipoApi.DELETE:
                    this.notificationService.error('Ocurrió un error al eliminar.', '', true);
                    break;
                case ITipoApi.MAIL:
                    this.notificationService.error('Ocurrió un error al enviar correo.', '', true);
                    break;
                case ITipoApi.OTHER:
                    this.notificationService.error('Ocurrió un error al ejecutar proceso.', '', true);
                    break;
            }   
        }
    }

}
