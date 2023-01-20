import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { MarcasService } from '../../../shared/services/marcas.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Marcas } from '../../../shared/models/marcas.model';
import { Paginator } from '../../../shared/models/paginator.model';

@Component({
  selector: 'app-marcas',
  templateUrl: './marcas.component.html',
  styleUrls: ['./marcas.component.scss'],
  animations: [SharedAnimations]
})
export class MarcasComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nueva Marca';
  public marcas: Marcas[] = [];
  public marca: Marcas = null;
  public defaultImage: FileList = null;
  public noResultsText: string = '';
  public selectedCategoriaPadre: any = 0;

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

  constructor(private marcasService: MarcasService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.marca = new Marcas();
  }

  ngOnInit() {
    this.getMarcas();
  }

  getMarcas() {
    this.spinner.show();    
    this.marcasService.getMarcasByPage(this.paginador).subscribe((res: Marcas[]) => {
      this.marcas = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron marcas.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener marcas', '', true); });
  }

  
  openModal(content, marca: Marcas) {
    if (marca != null) {

      //obtiene marca de bd
      this.spinner.show();

      this.marcasService.getMarca(marca.idMarca).subscribe((res: Marcas) => {
        this.marca = res;
        this.modalTitle = 'Editar Marca';
        this.spinner.hide();        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener marca' ,'' , true); });
      
    } else {
      this.marca = new Marcas();
      this.modalTitle = 'Nueva Marca';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onChange(event: any) {
    this.defaultImage = event.srcElement.files;
  }

  uploadImage(idMarca: number) {
    this.marcasService.subirImagen(this.defaultImage, idMarca).then(res => {
      this.notificationService.success('Guardado correctamente', '', true);
      this.getMarcas();
      this.modalService.dismissAll();
      this.spinner.hide();
      this.defaultImage = null;
    }).catch(err => {
      this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir imagen', '', true);
    });
  }

  async save(content: any) {
    this.marca.estado = this.marca.estado ? 1 : 0;

    if (this.marca.idMarca == 0) {

      this.spinner.show();

      this.marcasService.save(this.marca).subscribe((res: Marcas) => {

        if (this.defaultImage == null || this.defaultImage.length == 0) {

          this.notificationService.success('Guardado correctamente', '', true);
          this.getMarcas();
          this.modalService.dismissAll();
          this.spinner.hide();

        } else {
          this.uploadImage(res.idMarca);
        }
        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar marca', '', true); });

    } else {

      if (this.defaultImage != null && this.defaultImage.length > 0) {

        const response = await this.notificationService.confirmation('Marcas', 'La imagen actual será reemplazada por la seleccionada, ¿Desea continuar?');
        if (response.isConfirmed) {
          this.spinner.show();

          this.marcasService.edit(this.marca).subscribe(res => {
            this.uploadImage(this.marca.idMarca);
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar la marca', '', true);});
        }

      } else {

        this.spinner.show();

        this.marcasService.edit(this.marca).subscribe(res => {
          this.notificationService.success('Editado correctamente', '', true);
          this.getMarcas();
          this.modalService.dismissAll();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar la marca', '', true); });

      }
    }    
  }

  async delete (item: Marcas) {
    const response = await this.notificationService.confirmation('Eliminar Marca', '¿Confirma eliminar esta marca?');
    if (response.isConfirmed) {
      this.marcasService.delete(item.idMarca).subscribe(res => {
        this.notificationService.success('Marca eliminada', '', true);
        this.getMarcas();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar la marca', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getMarcas()
  }

  openFile(item: Marcas, tipo: number) {
    if (tipo == 1) {
      if (item.rutaImagen) {
        var win = window.open(item.rutaImagen, '_blank');
        win.focus();
      }
    } else if (tipo == 2) {
      if (item.url) {
        var win = window.open(item.url, '_blank');
        win.focus();
      }
    }    
  }

}
