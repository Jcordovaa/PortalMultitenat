import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { SecurityService } from '../../../shared/services/secutiry.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Permisos, Perfil, Acceso } from '../../../shared/models/security.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.scss'],
  animations: [SharedAnimations]
})
export class PermissionsComponent implements OnInit {

  viewMode: 'list' | 'grid' = 'list';
  allSelected: boolean;
  page = 1;
  pageSize = 10;
  products: any[] = [];

  public marcarText: string = 'Marcar todo';
  public accesos: Acceso[] = [];
  public permisos: Permisos[] = [];
  public perfiles: Perfil[] = [];
  public selectedPerfil: Perfil = null;
  public totalItems: number = 0;
  public noResultsText: string = '';
  public paginador: Paginator = {
    startRow: 0,
    endRow: 9999,
    sortBy: 'desc',
    search: ''
  };

  constructor(private securityService: SecurityService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) { }

  ngOnInit() {
    this.getPerfiles();
  }

  getPerfiles() {
    this.securityService.getPerfiles().subscribe((res: Perfil[]) => {
      this.perfiles = res;
    }, err => {
      this.notificationService.error('Ocurrió un error al obtener perfiles.', '', true);
    });
  }

  onChangePerfil() {
    const idPerfil: number = this.selectedPerfil.idPerfil;
    this.getPermisosByPerfil(idPerfil);
  }

  getMenuPadre(acceso: Acceso) {
    if (acceso.menuPadre != null && acceso.menuPadre != 0) {
      const item = this.accesos.find(x => x.idAcceso == acceso.menuPadre);
      if (item) {
        return item.nombre;
      }      
    }
    return '-';
  }

  getPermisosByPerfil(idPerfil: number) {
    this.securityService.getPermisosByPerfil(idPerfil).subscribe((res: Acceso[]) => {
      this.accesos = res;

      if (this.accesos.filter(x => x.checked == true).length == this.accesos.length) {
        this.marcarText = 'Desmarcar todo';
        this.allSelected = true;
      } else {
        this.marcarText = 'Marcar todo';
        this.allSelected = false;
      }

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron permisos.'
        this.totalItems = 0;
      }

    }, err => { this.notificationService.error('Ocurrió un error al obtener permisos.', '', true); });
  }

  selectAll(e) {
    this.marcarText = this.allSelected ? 'Desmarcar todo' : 'Marca todo';
    this.accesos = this.accesos.map(p => {
      p.checked = this.allSelected;
      return p;
    });
  }

  async save () {
    const response = await this.notificationService.confirmation('Guardar Permisos', '¿Confirma guardar los cambios realizados?');
    if (response.isConfirmed) {
      this.spinner.show();

      //crea objeto
      let permisos: Permisos[] = [];
      this.accesos.forEach((element: Acceso) => {
        permisos.push(element);
      });

      this.securityService.savePermisoMasivo(this.selectedPerfil.idPerfil, permisos).subscribe(res => {
        this.notificationService.success('Correcto.', '', true);
        this.spinner.hide();
      }, err => {  this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener permisos.', '', true); });
    }
  }

}
