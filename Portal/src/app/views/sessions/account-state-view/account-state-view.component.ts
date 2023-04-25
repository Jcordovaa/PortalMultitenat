import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { NgxSpinnerService } from "ngx-spinner";

;


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
    selector: 'app-accountStateView',
    templateUrl: './account-state-view.component.html',
    //   styleUrls: ['./account-state-view.component.scss']
})
export class AcountStateViewComponent implements OnInit {

    codAux: string = '';
    idCobranza: number = 0;
    automatizacion: string = '';
    constructor(
        private clientesService: ClientesService,
        private activatedRoute: ActivatedRoute,
        private notificationService: NotificationService,
        private spinner: NgxSpinnerService
    ) { }

    ngOnInit(): void {
        this.spinner.show();
        if (this.activatedRoute.snapshot.params['codAux'] != null && this.activatedRoute.snapshot.params['codAux'] != '' && this.activatedRoute.snapshot.params['codAux'] != '0') {
            this.codAux = window.atob(this.activatedRoute.snapshot.params['codAux']);
            if (this.activatedRoute.snapshot.params['idCobranza'] != null && this.activatedRoute.snapshot.params['idCobranza'] != '' && this.activatedRoute.snapshot.params['idCobranza'] != '0') {
                this.idCobranza = this.activatedRoute.snapshot.params['idCobranza'];
            } else {
                if (this.activatedRoute.snapshot.params['automatizacion'] != null && this.activatedRoute.snapshot.params['automatizacion'] != '' && this.activatedRoute.snapshot.params['automatizacion'] != '0') {
                    this.automatizacion = this.activatedRoute.snapshot.params['automatizacion'];
                }
            }

            let model = {
                codAux: this.codAux,
                idCobranza: this.idCobranza,
                automatizacion: this.automatizacion
            }

            this.clientesService.getPDFEstadoCuenta(model).subscribe((res: any) => {
                if(res == "0"){
                    this.notificationService.error("No se encontraron documentos o no se logro generar.", '', true);
                    this.spinner.hide();
                }
                if (res.base64 != '' && res.base64 != null) {
                    let a = document.createElement("a");
                    a.href = "data:application/octet-stream;base64," + res.base64;
                    a.download = res.nombre;
                    a.click();
                    this.spinner.hide();
                    window.close();
                }
            }, err => { this.spinner.hide();   this.notificationService.error("Error al descargar PDF", '', true);});

        }
    }
}
