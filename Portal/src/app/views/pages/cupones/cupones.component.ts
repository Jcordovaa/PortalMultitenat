import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { CuponesService } from '../../../shared/services/cupon.service';
import { ProductosService } from '../../../shared/services/productos.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Cupon, TipoDescuento } from '../../../shared/models/cupones.model';
import { Producto } from '../../../shared/models/productos.model';
import { Paginator } from '../../../shared/models/paginator.model';

@Component({
  selector: 'app-cupones',
  templateUrl: './cupones.component.html',
  styleUrls: ['./cupones.component.scss'],
  animations: [SharedAnimations]
})
export class CuponesComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Cupón';
  public cupones: Cupon[] = [];
  public productos: Producto[] = [];
  public tiposDescuento: TipoDescuento[] = [];
  public cupon: Cupon = null;
  public noResultsText: string = '';
  public selectedCategoriaPadre: any = 0;
  public selectedProducto: any = null;
  public selectedTipoDescuento: any = null;

  public config: any;
  public loaded: boolean = false;
  public totalItems: number = 0;
  public p: number = 1;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  constructor(private cuponesService: CuponesService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService, private productosService: ProductosService) {
      this.cupon = new Cupon();
  }

  ngOnInit() {
    this.getCupones();
    this.getProductos();
    this.getTiposDescuento();
  }

  getProductos() {
    this.productosService.getProductos().subscribe((res: Producto[]) => {
      this.productos = res;
    }, err => { this.notificationService.error('Ocurrió un error al obtener productos', '', true) });
  }

  getTiposDescuento() {
    this.cuponesService.getTiposDescuento().subscribe((res: TipoDescuento[]) => {
      this.tiposDescuento = res;
    }, err => { this.notificationService.error('Ocurrió un error al obtener tipos de descuento', '', true) });
  }
  
  getCupones() {
    this.spinner.show();    
    this.cuponesService.getCuponesByPage(this.paginador).subscribe((res: Cupon[]) => {
      this.cupones = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron cupones.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cupones', '', true); });
  }

  openModal(content, cupon: Cupon) {
    if (cupon != null) {

      //obtiene cupon de bd
      this.spinner.show();

      this.cuponesService.getCupon(cupon.idCupon).subscribe((res: Cupon) => {
        this.cupon = res;
        this.modalTitle = 'Editar Cupón';
        this.spinner.hide();
        this.selectedProducto = res.codProducto;
        this.selectedTipoDescuento = res.idTipoDescuento;        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cupon' ,'' , true); });
      
    } else {
      this.cupon = new Cupon();
      this.modalTitle = 'Nuevo Cupón';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save(content: any) {
    this.cupon.tipoDescuento = null;
    this.cupon.codProducto = this.selectedProducto;
    this.cupon.idTipoDescuento = this.selectedTipoDescuento;
    this.cupon.estado = this.cupon.estado ? 1 : 0;

    if (this.cupon.idCupon == 0) {

      this.spinner.show();

      this.cuponesService.save(this.cupon).subscribe((res: Cupon) => {
        this.notificationService.success('Correcto', '', true);
        this.getCupones();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar cupón', '', true); });

    } else {
      this.spinner.show();

      this.cuponesService.edit(this.cupon).subscribe(res => {
        this.notificationService.success('Editado correctamente', '', true);
        this.getCupones();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar categoría', '', true); });

    }    
  }

  async delete (item: Cupon) {
    const response = await this.notificationService.confirmation('Eliminar Cupón', '¿Confirma eliminar este cupón?');
    if (response.isConfirmed) {
      this.cuponesService.delete(item.idCupon).subscribe(res => {
        this.notificationService.success('Cupón eliminado', '', true);
        this.getCupones();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar cupón', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getCupones()
  }

}
