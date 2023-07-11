import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

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
    styleUrls: ['./account-state-view.component.scss'],
    animations: [SharedAnimations]
})
export class AcountStateViewComponent implements OnInit {

    public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno();
    codAux: string = '';
    idCobranza: number = 0;
    automatizacion: string = '';
    constructor(
        private clientesService: ClientesService,
        private activatedRoute: ActivatedRoute,
        private notificationService: NotificationService,
        private spinner: NgxSpinnerService, private disenoSerivce: ConfiguracionDisenoService, private router: Router
    ) { }

    ngOnInit(): void {
        this.spinner.show();
        this.disenoSerivce.getConfigDiseno().subscribe((res: ConfiguracionDiseno) => {
            this.configDiseno = res;
            if (this.activatedRoute.snapshot.params['codAux'] != null && this.activatedRoute.snapshot.params['codAux'] != '' && this.activatedRoute.snapshot.params['codAux'] != '0') {
                this.codAux = window.atob(this.activatedRoute.snapshot.params['codAux']);
                if (this.activatedRoute.snapshot.params['idCobranza'] != null && this.activatedRoute.snapshot.params['idCobranza'] != '' && this.activatedRoute.snapshot.params['idCobranza'] != '0') {
                    this.idCobranza = parseInt(this.activatedRoute.snapshot.params['idCobranza']);
                } else {
                    if (this.activatedRoute.snapshot.params['automatizacion'] != null && this.activatedRoute.snapshot.params['automatizacion'] != '' && this.activatedRoute.snapshot.params['automatizacion'] != '0') {
                        this.automatizacion = this.activatedRoute.snapshot.params['automatizacion'];
                    } else {
                        this.router.navigate(['/sessions/signin']);
                    }
                }
            } else {
                this.router.navigate(['/sessions/signin']);
            }
            this.spinner.hide();
        }, err => { this.spinner.hide(); });


    }


    descargar() {
        this.spinner.show()
        let model = {
            CodAux: this.codAux,
            IdCobranza: this.idCobranza,
            Automatizacion: this.automatizacion == '' ? null : this.automatizacion
        }

        this.clientesService.getPDFEstadoCuenta(model).subscribe((res: any) => {
            if (res == "0") {
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
        }, err => { this.spinner.hide(); this.notificationService.error("Error al descargar PDF", '', true); });
    }
}
