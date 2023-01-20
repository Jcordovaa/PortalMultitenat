import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { PasarelaPago } from 'src/app/shared/models/pasarelapago.model';
import { PasarelaPagoService } from 'src/app/shared/services/pasarelaPago.service';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';

interface AutoCompleteModel {
  value: any;
  display: string;
}

@Component({
  selector: 'app-payment-settings',
  templateUrl: './payment-settings.component.html',
  styleUrls: ['./payment-settings.component.scss']
})
export class PaymentSettingsComponent implements OnInit {

  pasarelas : PasarelaPago[] =[];
  tipoDocumentos: any = [];
  cuentasContables: any = [];
  

  constructor(private notificationService: NotificationService,
              private spinner: NgxSpinnerService, private pasarelasService: PasarelaPagoService, private softlandService: ConfiguracionSoftlandService) { }

  ngOnInit(): void {
    this.obtenerConfiguracion();
  }

  obtenerConfiguracion(){
    this.spinner.show();
    this.pasarelasService.getAllPasarelasPago().subscribe((res: any) => {
      
      this.pasarelas = res;
      this.softlandService.getAllCuentasContablesSoftland().subscribe((res2: any) => {
        
        this.cuentasContables = res2;
        this.cuentasContables.forEach(element => {
          element.nombre = element.codigo + ' - ' + element.nombre;
        });
        this.softlandService.getAllTipoDocSoftland().subscribe((res3: any) => {
          this.tipoDocumentos = res3;
          this.tipoDocumentos.forEach(element => {
            element.desDoc = element.codDoc + ' - ' + element.desDoc;
          });
        }, err => { this.notificationService.error('Ocurrió un error al obtener tipo de documentos contables', '', true);  });
      }, err => { this.notificationService.error('Ocurrió un error al obtener cuentas contables', '', true);  });
      this.spinner.hide();  
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener pasarelas de pago', '', true);  });
  }

  save(pasarela: PasarelaPago){
    this.spinner.show();
    pasarela.protocolo = 'https'
    this.pasarelasService.edit(pasarela).subscribe(res => {
      this.notificationService.success('Editado correctamente', '', true);
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar pasarela de pago', '', true); });
  }
}
