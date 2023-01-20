import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { SecurityService } from '../../../shared/services/secutiry.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Acceso } from '../../../shared/models/security.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-access',
  templateUrl: './access.component.html',
  styleUrls: ['./access.component.scss'],
  animations: [ SharedAnimations ]
})
export class AccessComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Acceso';
  public accesosAll: Acceso[] = [];
  public accesos: Acceso[] = [];
  public acceso: Acceso = null;
  public menusPadre: Acceso[] = [];
  public selectedMenuPadre: Acceso = new Acceso();
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

  constructor(private securityService: SecurityService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.acceso = new Acceso();      
    }

  ngOnInit(): void {
    this.getAccesos();
  }

  getAllAccesos() {
    this.securityService.getAccesos().subscribe((res: Acceso[]) => {
      this.accesosAll = res;
    });
  }

  getAccesos() {
    this.spinner.show();    
    this.securityService.getAccesosByPage(this.paginador).subscribe((res: Acceso[]) => {
      this.accesos = res;      

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron accesos.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.getAllAccesos();
      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los accesos', '', true); });
  }

  openModal(content, acceso: Acceso) {    
    if (acceso != null) {
      this.menusPadre = Object.assign([], this.accesosAll.filter(x => x.idAcceso != acceso.idAcceso));

      if (this.acceso.menuPadre != null && this.acceso.menuPadre != 0) {
        const padre: Acceso = this.accesos.find(x => x.idAcceso == this.acceso.menuPadre);
        if (padre) {
          this.selectedMenuPadre = padre;
        }
      }      

      //obtiene banner de bd
      this.spinner.show();

      this.securityService.getAcceso(acceso.idAcceso).subscribe((res: Acceso) => {
        this.acceso = res;
        this.spinner.hide();
        this.modalTitle = 'Editar Acceso';
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener acceso.' ,'' , true); });
      
    } else {
      this.menusPadre = Object.assign([], this.accesosAll);
      this.acceso = new Acceso();
      this.modalTitle = 'Nuevo Acceso';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save (content: any) {
    this.acceso.activo = this.acceso.activo ? 1 : 0;
    this.acceso.menuPadre = this.selectedMenuPadre.idAcceso != 0 ? this.selectedMenuPadre.idAcceso : null;

    if (this.acceso.idAcceso == 0) {

      this.spinner.show();

      this.securityService.saveAcceso(this.acceso).subscribe((res: Acceso) => {
        this.notificationService.success('Acceso guardado correctamente', 'Correcto', true);
        this.getAccesos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar acceso', '', true); });

    } else {

      this.securityService.editAcceso(this.acceso).subscribe(res => {
        this.notificationService.success('Correcto', 'Correcto', true);
        this.getAccesos();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar acceso', '', true);  });

    }    
  }

  async delete (item: Acceso) {
    const response = await this.notificationService.confirmation('Eliminar Acceso', '¿Confirma eliminar este Acceso?');
    if (response.isConfirmed) {
      this.securityService.deleteAcceso(item.idAcceso).subscribe(res => {
        this.notificationService.success('Acceso eliminado correctamente', 'Correcto', true); 
        this.getAccesos();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar el Acceso', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getAccesos()
  }

  getMenuPadre(acceso: Acceso) {
    if (acceso.menuPadre != null && acceso.menuPadre != 0) {
      const item = this.accesosAll.find(x => x.idAcceso == acceso.menuPadre);
      if (item) {
        return item.nombre;
      }      
    }
    return '-';
  }

}
