import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { ProductoSinonimos } from '../../../shared/models/productos.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { SinonimosService } from 'src/app/shared/services/sinonimos.service';
import { Utils } from '../../../shared/utils'

@Component({
  selector: 'app-sinonimos',
  templateUrl: './sinonimos.component.html',
  styleUrls: ['./sinonimos.component.scss'],
  animations: [SharedAnimations]
})
export class SinonimosComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Sinónimo';
  public sinonimos: ProductoSinonimos[] = [];
  public sinonimo: ProductoSinonimos = null;
  public noResultsText: string = '';
  private fileImport: FileList = null;
  private btnLoading: boolean = false;

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

  constructor(private sinonimosService: SinonimosService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService, private utils: Utils) {
      this.sinonimo = new ProductoSinonimos();
  }

  ngOnInit() {
    this.getSinonimos();
  }

  getSinonimos() {
    this.spinner.show();    
    this.sinonimosService.getSinonimosByPage(this.paginador).subscribe((res: ProductoSinonimos[]) => {
      this.sinonimos = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron sinónimos.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener sinónimos', '', true); });
  }

  openModal(content, sinonimo: ProductoSinonimos) {
    if (sinonimo != null) {

      //obtiene cupon de bd
      this.spinner.show();

      this.sinonimosService.getSinonimo(sinonimo.idSinonimo).subscribe((res: ProductoSinonimos) => {
        this.sinonimo = res;
        this.modalTitle = 'Editar Sinónimo';
        this.spinner.hide();      
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener sinónimo' ,'' , true); });
      
    } else {
      this.sinonimo = new ProductoSinonimos();
      this.modalTitle = 'Nuevo Sinónimo';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', backdrop: 'static' });
  }

  async save(content: any) {
    if (this.sinonimo.idSinonimo == 0) {
      this.spinner.show();

      this.sinonimosService.save(this.sinonimo).subscribe((res: ProductoSinonimos) => {
        this.notificationService.success('Correcto', '', true);
        this.getSinonimos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar sinónimo', '', true); });

    } else {
      this.spinner.show();

      this.sinonimosService.edit(this.sinonimo).subscribe(res => {
        this.notificationService.success('Editado correctamente', '', true);
        this.getSinonimos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar sinónimo', '', true); });

    }    
  }

  async delete (item: ProductoSinonimos) {
    const response = await this.notificationService.confirmation('Eliminar Sinónimo', '¿Confirma eliminar este sinónimo?');
    if (response.isConfirmed) {
      this.sinonimosService.delete(item.idSinonimo).subscribe(res => {
        this.notificationService.success('Sinónimo eliminado', '', true);
        this.getSinonimos();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar sinónimo', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getSinonimos()
  }

  onChange(event: any) {
    this.fileImport = event.srcElement.files;
  }

  procesarExcel() {
    this.btnLoading = true

    if (this.fileImport == null || this.fileImport.length == 0) {
      this.notificationService.error('Debe seleccionar un archivo Excel.', '', true);
      return;
    }

    this.spinner.show();

    this.sinonimosService.procesarExcel(this.fileImport).then(res => {
        
        this.notificationService.success('Correcto', '', true);
        this.spinner.hide();
        this.btnLoading = false
        this.modalService.dismissAll();
        this.getSinonimos();        

    })
    .catch((err) => {
      this.btnLoading = false;
      this.spinner.hide();
      if (err && err.length > 0) {
        this.notificationService.error(err, '', true);
      } else {
        this.notificationService.error('Ocurrió un error al procesar archivo, verifique el formato y datos del archivo.', '', true);
      }
    });

  }

  downloadFiles() {
    const url: string = `${this.utils.Server}/Uploads/Plantillas/SinonimosProductos.xlsx`;
    this.downloadURI(url, 'Plantilla Sinonimos');
  }

  downloadURI(uri: string, name: string) {
      var link = document.createElement("a");
      link.download = name;
      link.href = uri;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      link.remove();
  }

}
