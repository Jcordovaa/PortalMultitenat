import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { TiposDespachoService } from '../../../shared/services/tiposdespacho.service';
import { Ubicacioneservice } from '../../../shared/services/ubicaciones.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { TiposDespacho, TipoDesp } from '../../../shared/models/tiposdespacho.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { Region, Ciudad, Comuna, Ubicaciones } from '../../../shared/models/ubicaciones.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-tipo-despacho',
  templateUrl: './tipo-despacho.component.html',
  styleUrls: ['./tipo-despacho.component.scss'],
  animations: [SharedAnimations]
})
export class TipoDespachoComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public tiposDespachos: TiposDespacho[] = [];
  public tiposDesp: TipoDesp[] = [];
  public tipoDespacho: TiposDespacho = null;
  public noResultsText: string = '';
  public modalTitle: string = 'Nueva Tipo Despacho';
  public selectedTipo: TipoDesp = null;

  public regiones: Region[] = [];
  public ciudades: Ciudad[] = [];
  public comunas: Comuna[] = [];

  public selectedRegion: number = null;
  public selectedCiudad: number = null;
  public selectedComuna: number = null;

  public showPrice: boolean = false;
  public showRegion: boolean = false;
  public showProducto: boolean = false;
  public showCompra: boolean = false;

  public requiredProducto: boolean = false;
  public requiredRegion: boolean = false;
  public requiredPrecio: boolean = false;
  public requiredComuna: boolean = false;

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

  constructor(private tiposDespachoService: TiposDespachoService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService, private ubicacioneservice: Ubicacioneservice) {
      this.tipoDespacho = new TiposDespacho();
  }

  ngOnInit() {
    this.getTiposDespachos();
    this.getTipoDesp();
    this.getUbicaciones();
  }

  getUbicaciones() {
    this.ubicacioneservice.getUbicaciones().subscribe((res: Ubicaciones) => {
      this.regiones = res.regiones;
      this.ciudades = res.ciudades;
      this.comunas = res.comunas;
    }, err => { });
  }

  getTipoDesp() {
    this.tiposDespachoService.getTipoDesp().subscribe((res: TipoDesp[]) => {
      this.tiposDesp = res;
    }, err => { });
  }
  
  getTiposDespachos() {
    this.spinner.show();    
    this.tiposDespachoService.getTiposDespachosByPage(this.paginador).subscribe((res: TiposDespacho[]) => {
      this.tiposDespachos = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron tipos de despacho.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener tipos de despacho', '', true); });
  }

  openModal(content, td: TiposDespacho) {
    if (td != null) {

      //obtiene cupon de bd
      this.spinner.show();

      this.tiposDespachoService.getTipoDespacho(td.idTipoDespacho).subscribe((res: TiposDespacho) => {
        this.tipoDespacho = res;
        this.modalTitle = 'Editar Tipo Despacho';
        this.spinner.hide();    
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener tipos de despacho' ,'' , true); });
      
    } else {
      this.tipoDespacho = new TiposDespacho();
      this.modalTitle = 'Nuevo Tipo Despacho';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save(content: any) {
    this.tipoDespacho.tipo = this.selectedTipo.idTipoDes;
    this.tipoDespacho.estado = this.tipoDespacho.estado ? 1 : 0;
    this.tipoDespacho.idRegion = this.selectedRegion != null ? this.selectedRegion : null;
    this.tipoDespacho.idComuna = this.selectedComuna != null ? this.selectedComuna : null;
    this.tipoDespacho.idCiudad = null;

    if (this.tipoDespacho.idTipoDespacho == 0) {

      this.spinner.show();

      this.tiposDespachoService.save(this.tipoDespacho).subscribe((res: TiposDespacho) => {
        this.notificationService.success('Correcto', '', true);
        this.getTiposDespachos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar', '', true); });

    } else {
      this.spinner.show();

      this.tiposDespachoService.edit(this.tipoDespacho).subscribe(res => {
        this.notificationService.success('Editado correctamente', '', true);
        this.getTiposDespachos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar', '', true); });

    }    
  }

  async delete (item: TiposDespacho) {
    const response = await this.notificationService.confirmation('Eliminar Tipo de Despacho', '¿Confirma eliminar este tipo de despacho?');
    if (response.isConfirmed) {
      this.tiposDespachoService.delete(item.idTipoDespacho).subscribe(res => {
        this.notificationService.success('Tipo Despacho eliminado correctamente', '', true);
        this.getTiposDespachos();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getTiposDespachos()
  }

  onChangeTipo() {
    switch (this.selectedTipo.idTipoDes) {
      case 1:
        this.showPrice = false;
        this.showRegion = true;
        this.showCompra = true;
        this.showProducto = false;

        this.requiredPrecio = false;
        this.requiredRegion = false;
        this.requiredComuna = false;
        this.requiredProducto = false;
        break;
      case 2:
        this.showPrice = false;
        this.showRegion = false;
        this.showCompra = false;
        this.showProducto = false;

        this.requiredPrecio = false;
        this.requiredRegion = false;
        this.requiredComuna = false;
        this.requiredProducto = false;
        break;
      case 3:
        this.showPrice = true;
        this.showRegion = true;
        this.showCompra = false;
        this.showProducto = true;

        this.requiredPrecio = true;
        this.requiredRegion = true;
        this.requiredComuna = true;
        this.requiredProducto = true;
        break;
      default:
          break;
    }
  }

}
