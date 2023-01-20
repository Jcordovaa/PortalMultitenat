import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { CategoriasService } from '../../../shared/services/categorias.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Categorias } from '../../../shared/models/categorias.model';
import { Paginator } from '../../../shared/models/paginator.model';

@Component({
  selector: 'app-categorias',
  templateUrl: './categorias.component.html',
  styleUrls: ['./categorias.component.scss'],
  animations: [SharedAnimations]
})
export class CategoriasComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nueva Categoría';
  public categorias: Categorias[] = [];
  public categoria: Categorias = null;
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

  constructor(private categoriasService: CategoriasService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.categoria = new Categorias();
  }

  ngOnInit() {
    this.getCategorias();
  }

  getCategorias() {
    this.spinner.show();    
    this.categoriasService.getCategoriasByPage(this.paginador).subscribe((res: Categorias[]) => {
      this.categorias = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron categorías.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener categorías', '', true); });
  }

  openModal(content, categoria: Categorias) {
    if (categoria != null) {

      //obtiene categoria de bd
      this.spinner.show();

      this.categoriasService.getCategoria(categoria.idCategoria).subscribe((res: Categorias) => {
        this.categoria = res;
        if (res.idCategoriaPadre == null || res.idCategoriaPadre == 0) {
          this.selectedCategoriaPadre = 0;
        } else {
          this.selectedCategoriaPadre = this.categorias.find(x => x.idCategoria == res.idCategoriaPadre)
        }
        this.modalTitle = 'Editar Categoría';
        this.spinner.hide();
        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener categoría' ,'' , true); });
      
    } else {
      this.categoria = new Categorias();
      this.modalTitle = 'Nueva Categoría';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onChange(event: any) {
    this.defaultImage = event.srcElement.files;
  }

  uploadImage(idCategoria: number) {
    this.categoriasService.subirImagen(this.defaultImage, idCategoria).then(res => {
      this.notificationService.success('Guardado correctamente', '', true);
      this.getCategorias();
      this.modalService.dismissAll();
      this.spinner.hide();
      this.defaultImage = null;
    }).catch(err => {
      this.spinner.hide(); this.notificationService.error('Ocurrió un error al subir imagen', '', true);
    });
  }

  async save(content: any) {
    this.categoria.idCategoriaPadre = this.selectedCategoriaPadre.idCategoria == null 
                                        ? (this.selectedCategoriaPadre === "0" ? 0 : this.selectedCategoriaPadre) 
                                        : this.selectedCategoriaPadre.idCategoria;
    this.categoria.estado = this.categoria.estado ? 1 : 0;
    this.categoria.esCategoriaPadre = this.categoria.esCategoriaPadre ? 1 : 0;
    this.categoria.categoriaDestacada = this.categoria.categoriaDestacada ? 1 : 0;
    this.categoria.destacarSeccion = this.categoria.destacarSeccion ? 1 : 0;

    if (this.categoria.idCategoria == 0) {

      if (this.defaultImage == null || this.defaultImage.length == 0) {
        this.notificationService.warning('Debe seleccionar una imagen', '', true);
        return;
      }

      this.spinner.show();

      this.categoriasService.save(this.categoria).subscribe((res: Categorias) => {
        this.uploadImage(res.idCategoria);
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar categoría', '', true); });

    } else {

      if (this.defaultImage != null && this.defaultImage.length > 0) {

        const response = await this.notificationService.confirmation('Categoría', 'La imagen actual será reemplazada por la seleccionada, ¿Desea continuar?');
        if (response.isConfirmed) {
          this.spinner.show();

          this.categoriasService.edit(this.categoria).subscribe(res => {
            this.uploadImage(this.categoria.idCategoria);
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar categoría', '', true);});
        }

      } else {

        this.spinner.show();

        this.categoriasService.edit(this.categoria).subscribe(res => {
          this.notificationService.success('Editado correctamente', '', true);
          this.getCategorias();
          this.modalService.dismissAll();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar categoría', '', true); });

      }
    }    
  }

  async delete (item: Categorias) {
    const response = await this.notificationService.confirmation('Eliminar Categoría', '¿Confirma eliminar esta categoría?');
    if (response.isConfirmed) {
      this.categoriasService.delete(item.idCategoria).subscribe(res => {
        this.notificationService.success('Categoría eliminada', '', true);
        this.getCategorias();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar categoría', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getCategorias()
  }

  openImage(imagen: any) {
    if (imagen) {
      var win = window.open(imagen, '_blank');
      win.focus();
    }
  }

}
