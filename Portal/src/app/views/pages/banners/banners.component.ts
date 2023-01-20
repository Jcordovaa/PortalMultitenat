import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { BannersService } from '../../../shared/services/banner.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Banners, TipoBanners } from '../../../shared/models/banners.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-banners',
  templateUrl: './banners.component.html',
  styleUrls: ['./banners.component.scss'],
  animations: [SharedAnimations]
})
export class BannersComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Banner';
  public banners: Banners[] = [];
  public tipoBanners: TipoBanners[] = [];
  public banner: Banners = null;
  public selectedTipoBanner: TipoBanners = null;
  public defaultImage: FileList = null;
  public noResultsText: string = '';

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

  constructor(private bannersService: BannersService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.banner = new Banners();
      this.selectedTipoBanner = new TipoBanners();
  }

  ngOnInit() {
    this.getBanners();
    this.getTipoBanners();
  }

  getBanners() {
    this.spinner.show();    
    this.bannersService.getBannersByPage(this.paginador).subscribe((res: Banners[]) => {
      this.banners = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron banners.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los banners', '', true); });
  }

  getTipoBanners() {
    this.bannersService.getTipoBanners().subscribe((res: TipoBanners[]) => {
      this.tipoBanners = res;
    }, err => { this.notificationService.error('Ocurrió un error al obtener los tipos de banners', '', true); });
  }

  openModal(content, banner: Banners) {
    if (banner != null) {

      //obtiene banner de bd
      this.spinner.show();

      this.bannersService.getBanner(banner.idBanner).subscribe((res: Banners) => {
        this.banner = res;
        this.spinner.hide();
        this.selectedTipoBanner = this.tipoBanners.find(x => x.idTipoBanner == res.idTipoBanner);
        this.modalTitle = 'Editar Banner';
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener banner' ,'' , true); });
      
    } else {
      this.banner = new Banners();
      this.modalTitle = 'Nuevo Banner';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onChange(event: any) {
    this.defaultImage = event.srcElement.files;
  }

  uploadImage(idBanner: number) {
    this.bannersService.subirImagen(this.defaultImage, idBanner).then(res => {
      this.notificationService.success('Banner guardado correctamente', 'Correcto', true);
      this.getBanners();
      this.modalService.dismissAll();
      this.spinner.hide();
    }).catch(err => {
      this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir la imagen', '', true);
    });
  }

  async save (content: any) {  
    this.banner.idTipoBanner = this.selectedTipoBanner.idTipoBanner;
    this.banner.estado = this.banner.estado ? 1 : 0;

    if (this.banner.idBanner == 0) {

      if (this.defaultImage == null || this.defaultImage.length == 0) {
        this.notificationService.warning('Debe seleccionar una imagen', '', true);
        return;
      }

      this.spinner.show();

      this.bannersService.save(this.banner).subscribe((res: Banners) => {
        this.uploadImage(res.idBanner);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar banner', '', true); });

    } else {

      if (this.defaultImage != null && this.defaultImage.length > 0) {

        const response = await this.notificationService.confirmation('Editar Banner', 'El banner actual será reemplazado por el seleccionado, ¿Desea continuar?');
        if (response.isConfirmed) {
          this.spinner.show();

          this.bannersService.edit(this.banner).subscribe(res => {
            this.uploadImage(this.banner.idBanner);
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar banner', '', true);  });
        }

      } else {

        this.spinner.show();

        this.bannersService.edit(this.banner).subscribe(res => {
          this.notificationService.success('Editado correctamente', 'Correcto', true); 
          this.getBanners();
          this.modalService.dismissAll();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar banner', '', true);  });

      }
    }    
  }

  async delete (item: Banners) {
    const response = await this.notificationService.confirmation('Eliminar Banner', '¿Confirma eliminar este banner?');
    if (response.isConfirmed) {
      this.bannersService.delete(item.idBanner).subscribe(res => {
        this.notificationService.success('Banner eliminado correctamente', 'Correcto', true); 
        this.getBanners();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar el banner', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getBanners()
  } 

  openImage(imagen: any) {
    if (imagen) {
      var win = window.open(imagen, '_blank');
      win.focus();
    }
  }

}
