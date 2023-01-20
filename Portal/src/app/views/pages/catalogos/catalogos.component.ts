import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { CatalogosService } from '../../../shared/services/catalogos.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Catalogo } from '../../../shared/models/catalogo.model';
import { Paginator } from '../../../shared/models/paginator.model';

@Component({
  selector: 'app-catalogos',
  templateUrl: './catalogos.component.html',
  styleUrls: ['./catalogos.component.scss'],
  animations: [SharedAnimations]
})
export class CatalogosComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Catálogo';
  public catalogos: Catalogo[] = [];
  public catalogo: Catalogo = null;
  public defaultImage: FileList = null;
  public defaultImageFile: FileList = null;
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

  constructor(private catalogosService: CatalogosService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.catalogo = new Catalogo();
  }

  ngOnInit() {
    this.getCatalogos();
  }

  getCatalogos() {
    this.spinner.show();    
    this.catalogosService.getCatalogsByPage(this.paginador).subscribe((res: Catalogo[]) => {
      this.catalogos = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron catálogos.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener catálogos', '', true); });
  }
  
  openModal(content, catalogo: Catalogo) {
    if (catalogo != null) {

      //obtiene marca de bd
      this.spinner.show();

      this.catalogosService.getCatalogo(catalogo.idCatalogo).subscribe((res: Catalogo) => {
        this.catalogo = res;
        this.modalTitle = 'Editar Catálogo';
        this.spinner.hide();        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener Catálogo' ,'' , true); });
      
    } else {
      this.catalogo = new Catalogo();
      this.modalTitle = 'Nuevo Catálogo';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  onChange(event: any) {
    this.defaultImage = event.srcElement.files;
  }

  onChangeFile(event: any) {
    this.defaultImageFile = event.srcElement.files;
  }

  async uploadFile (idCatalogo: number, file: FileList, isImage: boolean) {
    await new Promise((resolve, reject) => {
      this.catalogosService.subirArchivo(file, idCatalogo, isImage).then(res => {
        resolve();
      }).catch(err => {
        reject();
      });
    });
  }

  async upload (idCatalogo: number) {
    if ((this.defaultImage != null && this.defaultImage.length > 0) && (this.defaultImageFile != null && this.defaultImageFile.length > 0)) {
      await this.uploadFile(idCatalogo, this.defaultImage, true);
      await this.uploadFile(idCatalogo, this.defaultImageFile, false);
    }
    else if ((this.defaultImage != null && this.defaultImage.length > 0) && (this.defaultImageFile == null || this.defaultImageFile.length == 0)) {
      await this.uploadFile(idCatalogo, this.defaultImage, true);
    } 
    else if ((this.defaultImageFile != null && this.defaultImageFile.length > 0) && (this.defaultImage == null || this.defaultImage.length == 0)) {
      await  this.uploadFile(idCatalogo, this.defaultImageFile, false);
    }

    this.notificationService.success('Guardado correctamente', '', true);
    this.getCatalogos();
    this.modalService.dismissAll();
    this.spinner.hide();
    this.defaultImage = null;
    this.defaultImageFile = null;

  }

  async save(content: any) {
    this.catalogo.estado = this.catalogo.estado ? 1 : 0;

    if (this.catalogo.idCatalogo == 0) {

      this.spinner.show();

      this.catalogosService.save(this.catalogo).subscribe((res: Catalogo) => {

        if ((this.defaultImage == null || this.defaultImage.length == 0) && (this.defaultImageFile == null || this.defaultImageFile.length == 0)) {

          this.notificationService.success('Guardado correctamente', '', true);
          this.getCatalogos();
          this.modalService.dismissAll();
          this.spinner.hide();

        } else {
          this.upload(res.idCatalogo);
        }
        
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar catálogo', '', true); });

    } else {

      let message: string = '';
      if (((this.defaultImage != null && this.defaultImage.length > 0) && (this.catalogo.rutaImagen != null && this.catalogo.rutaImagen != '')) &&
          ((this.defaultImageFile != null && this.defaultImageFile.length > 0) && (this.catalogo.rutaArchivo != null && this.catalogo.rutaArchivo != ''))) {
            message = 'La imagen y archivo de este catálogo serán reemplazados por los seleccionados, ¿Desea continuar?';
      } else if ((this.defaultImage != null && this.defaultImage.length > 0) && (this.catalogo.rutaImagen != null && this.catalogo.rutaImagen != '')) {
        message = 'La imagen actual será reemplazada por la seleccionada, ¿Desea continuar?';
      } else if ((this.defaultImageFile != null && this.defaultImageFile.length > 0) && (this.catalogo.rutaArchivo != null && this.catalogo.rutaArchivo != '')) {
        message = 'El archivo actual será reemplazadp por el seleccionado, ¿Desea continuar?';
      } else {
        message = "ok";
      }

      if (message.length > 0) {
        if (message == "ok") {

          this.spinner.show();
          this.catalogosService.edit(this.catalogo).subscribe(res => {
            this.upload(this.catalogo.idCatalogo);
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar la catálogo', '', true); });

        } else {

          const response = await this.notificationService.confirmation('Catálogo', message);
          if (response.isConfirmed) {
            this.spinner.show();

            this.catalogosService.edit(this.catalogo).subscribe(res => {
              this.upload(this.catalogo.idCatalogo);
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar la catálogo', '', true); });
          }

        }        

      } else {

        this.spinner.show();

        this.catalogosService.edit(this.catalogo).subscribe(res => {
          this.notificationService.success('Editado correctamente', '', true);
          this.getCatalogos();
          this.modalService.dismissAll();
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar la catálogo', '', true); });

      }
    }    
  }

  async delete (item: Catalogo) {
    const response = await this.notificationService.confirmation('Eliminar Catálogo', '¿Confirma eliminar este catálogo?');
    if (response.isConfirmed) {
      this.catalogosService.delete(item.idCatalogo).subscribe(res => {
        this.notificationService.success('Catálogo eliminado', '', true);
        this.getCatalogos();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar la catálogo', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getCatalogos()
  }

  openFile(item: Catalogo, tipo: number) {
    if (tipo == 1) {
      if (item.rutaImagen) {
        var win = window.open(item.rutaImagen, '_blank');
        win.focus();
      }
    } else if (tipo == 2) {
      if (item.rutaArchivo) {
        var win = window.open(item.rutaArchivo, '_blank');
        win.focus();
      }
    }    
  }

}
