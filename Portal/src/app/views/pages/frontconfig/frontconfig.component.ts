import { Component, OnInit } from '@angular/core';
import { Configuracion } from '../../../shared/models/configuracion.model';
import { TipoBanners } from '../../../shared/models/banners.model';
import { ConfiguracionService } from '../../../shared/services/configuracion.service';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { BannersService } from '../../../shared/services/banner.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-frontconfig',
  templateUrl: './frontconfig.component.html',
  styleUrls: ['./frontconfig.component.scss']
})
export class FrontconfigComponent implements OnInit {

  public tipoBanners: TipoBanners[] = [];
  public config: Configuracion = new Configuracion();

  public selectedTipoBanner: number = null;
  public modalTitle: string = '';
  public modalContent: string = '';
  public modalImage: string = '';

  constructor(private modalService: NgbModal, private configuracionService: ConfiguracionService,
    private bannersService: BannersService, private notificationService: NotificationService,
    private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.spinner.show();    

    this.configuracionService.getConfig().subscribe((res: Configuracion[]) => {
      this.config = res[0];

      this.bannersService.getTipoBanners().subscribe((res: TipoBanners[]) => {
        this.tipoBanners = res;
        this.selectedTipoBanner = this.config.tipoBanner;
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener tipos de banners', '', true);  });

    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener información', '', true); });
  }

  openImage() {
    if (this.modalImage.length > 0) {
      var win = window.open(`${window.location.origin}/${this.modalImage}`, '_blank');
      win.focus();
    }    
  }

  openInfoModal(content: any, type: number) {
    switch (type) {
      case 1:
        this.modalTitle = 'Categorías Destacadas';
        this.modalContent = 'Al marcar la casilla "Categorías destacadas", se mostraran 4 categorias en la página de inicio.';
        this.modalImage = 'assets/images/config/categoriasDestacadas.JPG';
        break;
      case 2:
        this.modalTitle = 'Caraterísticas empresa';
        this.modalContent = 'Al marcar la casilla "Caraterísticas empresa", se mostrará ...';
        this.modalImage = '';
        break;
      case 3:
        this.modalTitle = 'Recomendar producto';
        this.modalContent = 'Al marcar la casilla "Recomendar producto", se mostrará el listado de productos recomendados en ...';
        this.modalImage = 'assets/images/config/productosRecomendados.JPG';
        break;
      case 4:
        this.modalTitle = 'Mostrar unidad de medida';
        this.modalContent = 'Al marcar la casilla "Muestra unida de medida", Se mostrara un texto con la unidad de medida de venta del producto en Softland.';
        this.modalImage = 'assets/images/config/mostrarUnidadMedida.JPG';
        break;
      case 5:
        this.modalTitle = 'Mostrar precio a pedido';
        this.modalContent = 'Al marcar la casilla "Mostrar precio a pedido", se mostrara el precio en forma referencial en los productos marcados como "A Pedido" en Softland.';
        this.modalImage = '';
        break;
      case 6:
        this.modalTitle = 'Mostrar cupones';
        this.modalContent = 'Al marcar la casilla "Mostrar cupones", se mostrará el campo para ingresar cupones en el carrito de compras.';
        this.modalImage = '';
        break;
      case 7:
        this.modalTitle = 'Mostrar términos';
        this.modalContent = 'Al marcar la casilla "Mostrar términos", Se requerirá al usuario aceptar los términos y condiciones de venta web antes de finalizar la compra.';
        this.modalImage = '';
        break;
      case 8:
        this.modalTitle = 'Mostrar documentos de venta';
        this.modalContent = 'Al marcar la casilla "Mostrar documentos de venta", se mostraran los documentos de venta en el carrito de compras.';
        this.modalImage = '';
        break;
      case 9:
        this.modalTitle = 'Mostrar observaciones';
        this.modalContent = 'Al marcar la casilla "Mostrar observaciones", se mostrará el campo para ingresar observaciones en el carrito de compras.';
        this.modalImage = '';
        break;
      case 10:
        this.modalTitle = 'Generar venta';
        this.modalContent = 'Al marcar la casilla "Generar venta", se mostrará ...';
        this.modalImage = '';
        break;
    }
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  save() {
    this.spinner.show(); 
    this.config.tipoBanner = this.selectedTipoBanner;
    this.config.categoriasDestacadas = this.config.categoriasDestacadas ? 1 : 0;
    this.config.caracteristicasEmpresa = this.config.caracteristicasEmpresa ? 1 : 0;
    this.config.recomendarProducto = this.config.recomendarProducto ? 1 : 0;
    this.config.mostrarUnidadMedida = this.config.mostrarUnidadMedida ? 1 : 0;
    this.config.muestraPrecioPedido = this.config.muestraPrecioPedido ? 1 : 0;
    this.config.muestraCupones = this.config.muestraCupones ? 1 : 0;
    this.config.muestraTerminos = this.config.muestraTerminos ? 1 : 0;
    this.config.muestraDocVenta = this.config.muestraDocVenta ? 1 : 0;
    this.config.muestraObservaciones = this.config.muestraObservaciones ? 1 : 0;
    this.config.generaVenta = this.config.generaVenta ? 1 : 0;

    this.configuracionService.edit(this.config).subscribe(res => {
      this.spinner.hide(); 
      this.notificationService.success('Correcto', '', true);
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar', '', true); });
  }

}
