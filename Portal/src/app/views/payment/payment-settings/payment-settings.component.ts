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

  pasarelas: PasarelaPago[] = [];
  tipoDocumentos: any = [];
  cuentasContables: any = [];
  verContrasena: number = 0;
  icon : string = 'assets/images/icon/view.png';
  public ambiente: string = '';

  constructor(private notificationService: NotificationService,
    private spinner: NgxSpinnerService, private pasarelasService: PasarelaPagoService, private softlandService: ConfiguracionSoftlandService) { }

  ngOnInit(): void {
    this.obtenerConfiguracion();
  }

  obtenerConfiguracion() {
    this.spinner.show();
    this.pasarelasService.getAllPasarelasPago().subscribe((res: any) => {

      this.pasarelas = res;

      let tbk = this.pasarelas.filter(x => x.idPasarela == 1);
      
      if(tbk.length != null){
        
        if(tbk[0].codigoComercio == '597055555532'){
          this.ambiente = 'INTEGRACION';
        }else{
          this.ambiente = 'PRODUCCION';
        }
      }
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
        }, err => { this.notificationService.error('Ocurrió un error al obtener tipo de documentos contables', '', true); });
      }, err => { this.notificationService.error('Ocurrió un error al obtener cuentas contables', '', true); });
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener pasarelas de pago', '', true); });
  }

  save(pasarela: PasarelaPago) {

    if(pasarela.idPasarela == 1){
      if(pasarela.codigoComercio == '' || pasarela.codigoComercio == null){
        this.notificationService.warning('Debe ingresar código de comercio', '', true); 
        return;
      }

      if(pasarela.secretKey == '' || pasarela.secretKey == null){
        this.notificationService.warning('Debe ingresar Api Key', '', true); 
        return;
      }
    }

    this.spinner.show();
    pasarela.protocolo = 'https'
    this.pasarelasService.edit(pasarela).subscribe(res => {
      this.notificationService.success('Editado correctamente', '', true);
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar pasarela de pago', '', true); });
  }

  verPass() {
    if (this.verContrasena == 1) {
      this.verContrasena = 0;
    } else {
      this.verContrasena = 1;
    }
    if (this.verContrasena == 0) {
      this.icon = 'assets/images/icon/view.png';
      document.getElementsByName("apiKey")[0].setAttribute('type', 'password');
    } else {
      this.icon = 'assets/images/icon/invisible.png';
      document.getElementsByName("apiKey")[0].setAttribute('type', 'text');
    }

  }

  cambiaAmbienteTbk(pasarela: any){
    if(pasarela.idPasarela == 1){
      if(this.ambiente == 'INTEGRACION'){
        pasarela.codigoComercio = '597055555532'
        pasarela.secretKey = '579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C'
      }else{
        pasarela.codigoComercio = ''
        pasarela.secretKey = ''
      }
    }
  }
}
