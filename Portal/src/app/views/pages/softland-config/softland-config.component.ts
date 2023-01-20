import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from "ngx-spinner";
import { ConfiguracionSoftlandService } from '../../../shared/services/configuracionsoftland.service';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ConfiguracionSoftland } from '../../../shared/models/configuracion.model';

@Component({
  selector: 'app-softland-config',
  templateUrl: './softland-config.component.html',
  styleUrls: ['./softland-config.component.scss']
})
export class SoftlandConfigComponent implements OnInit {

  public config: ConfiguracionSoftland = new ConfiguracionSoftland();
  public listasPrecios: any = [];
  public listasPreciosOferta: any = [];
  public condicionesVenta: any = [];
  public vendedores: any = [];
  public canalesVenta: any = [];
  public cargos: any = [];
  public categorias: any = [];
  public giros: any = [];

  public selectedListaPrecio: string = null;
  public selectedListaPrecioOferta: string = null;
  public selectedCondicionVenta: string = null;
  public selectedVendedor: string = null;
  public selectedCanalVenta: string = null;
  public selectedCargo: string = null;
  public selectedCategoria: string = null;
  public selectedGiro: string = null;
  public stockMinimo: number = null;

  constructor(private configuracionSoftlandService: ConfiguracionSoftlandService, private notificationService: NotificationService,
              private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.spinner.show();

    this.configuracionSoftlandService.getConfig().subscribe((res: any) => {
      this.listasPrecios = res.listaPrecios;
      this.listasPreciosOferta = res.listaPreciosOferta;
      this.condicionesVenta = res.condicionesVenta;
      this.vendedores = res.vendedores;
      this.canalesVenta = res.canalesVenta;
      this.cargos = res.cargos;
      this.categorias = res.categorias;
      this.giros = res.giros;
      this.config = res.config;

      this.selectedListaPrecio = this.config.codListaPrecios;
      this.selectedListaPrecioOferta = this.config.codListaPreciosOferta;
      this.selectedCondicionVenta = this.config.codCondicionVenta;
      this.selectedVendedor = this.config.codVendedor;
      this.selectedCanalVenta = this.config.codCanalVenta;
      this.selectedCargo = this.config.codCargo;
      this.selectedCategoria = this.config.codCategoriaCliente;
      this.selectedGiro = this.config.codGiro;
      this.stockMinimo = this.config.stockMinimo;
      this.spinner.hide();

    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener información', '', true); });
  }

  save() {
    this.spinner.show();

    this.config.codListaPrecios = this.selectedListaPrecio;
    this.config.codListaPreciosOferta = this.selectedListaPrecioOferta;
    this.config.codCondicionVenta = this.selectedCondicionVenta;
    this.config.codVendedor = this.selectedVendedor;
    this.config.codCanalVenta = this.selectedCanalVenta;
    this.config.codCargo = this.selectedCargo;
    this.config.codCategoriaCliente = this.selectedCategoria;
    this.config.stockMinimo = this.stockMinimo;

    this.configuracionSoftlandService.edit(this.config).subscribe(res => {
      this.spinner.hide();
      this.notificationService.success('Correcto', '', true);
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al actualizar información', '', true); });
  }

}
