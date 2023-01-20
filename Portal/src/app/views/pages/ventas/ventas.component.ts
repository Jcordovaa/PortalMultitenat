import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { VentasService } from '../../../shared/services/ventas.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Venta, VentaDetalle } from '../../../shared/models/ventas.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-ventas',
  templateUrl: './ventas.component.html',
  styleUrls: ['./ventas.component.scss'],
  animations: [ SharedAnimations ]
})
export class VentasComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public ventas: Venta[] = [];
  public venta: Venta = null;
  public noResultsText: string = '';
  public step: number = 1;

  public loaded: boolean = false;
  public totalItems: number = 0;
  public config: any;
  public p: number = 1;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  constructor(private ventasService: VentasService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.venta = new Venta();
  }

  ngOnInit(): void {
    this.getVentas();
  }

  getVentas() {
    this.spinner.show();    
    this.ventasService.getVentasByPage(this.paginador).subscribe((res: Venta[]) => {
      this.ventas = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron ventas.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener las ventas', '', true); });
  }

  ventaDetalle(v: Venta) {
    this.spinner.show();    
    this.ventasService.getDetallesVenta(v.idVenta).subscribe((res: VentaDetalle[]) => {
      this.venta = v;
      this.venta.ventaDetalle = res;
      this.step = 2;  
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la venta.', '', true); });     
  }

}
