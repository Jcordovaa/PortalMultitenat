import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { SecurityService } from '../../../shared/services/secutiry.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Perfil } from '../../../shared/models/security.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-profiles',
  templateUrl: './profiles.component.html',
  styleUrls: ['./profiles.component.scss'],
  animations: [ SharedAnimations ]
})
export class ProfilesComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Perfil';
  public perfiles: Perfil[] = [];
  public perfil: Perfil = null;
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
      this.perfil = new Perfil(); 
    }

  ngOnInit(): void {
    this.getPerfiles();
  }

  getPerfiles() {
    this.spinner.show();    
    this.securityService.getPerfilesByPage(this.paginador).subscribe((res: Perfil[]) => {
      this.perfiles = res;      

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron perfiles.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
      
    }, err => {var error = err; this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los perfiles', '', true); });
  }

  openModal(content, perfil: Perfil) {    
    if (perfil != null) {
      //obtiene banner de bd
      this.spinner.show();

      this.securityService.getPerfil(perfil.idPerfil).subscribe((res: Perfil) => {
        this.perfil = res;
        this.spinner.hide();
        this.modalTitle = 'Editar Perfil';
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener perfil.' ,'' , true); });
      
    } else {
      this.perfil = new Perfil();
      this.modalTitle = 'Nuevo Perfil';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save (content: any) {
    if (this.perfil.idPerfil == 0) {

      this.spinner.show();

      this.securityService.savePerfil(this.perfil).subscribe((res: Perfil) => {
        this.notificationService.success('Perfil guardado correctamente', 'Correcto', true);
        this.getPerfiles();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar Perfil', '', true); });

    } else {

      this.securityService.editPerfil(this.perfil).subscribe(res => {
        this.notificationService.success('Correcto', 'Correcto', true);
        this.getPerfiles();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar Perfil', '', true);  });

    }    
  }

  async delete (item: Perfil) {
    const response = await this.notificationService.confirmation('Eliminar Perfil', '¿Confirma eliminar este Perfil?');
    if (response.isConfirmed) {
      this.securityService.deletePerfil(item.idPerfil).subscribe(res => {
        this.notificationService.success('Perfil eliminado correctamente', 'Correcto', true); 
        this.getPerfiles();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar el Perfil', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getPerfiles()
  }

}
