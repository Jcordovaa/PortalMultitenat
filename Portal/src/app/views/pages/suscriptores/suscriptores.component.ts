import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { SuscriptoresService } from '../../../shared/services/suscriptores.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Suscripciones } from '../../../shared/models/suscripciones.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-suscriptores',
  templateUrl: './suscriptores.component.html',
  styleUrls: ['./suscriptores.component.scss'],
  animations: [SharedAnimations]
})
export class SuscriptoresComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public suscriptores: Suscripciones[] = [];
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

  constructor(private suscriptoresService: SuscriptoresService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
  }

  ngOnInit(): void {
    this.getSuscriptores();
  }

  getSuscriptores() {
    this.spinner.show();    
    this.suscriptoresService.getSuscripcionesByPage(this.paginador).subscribe((res: Suscripciones[]) => {
      this.suscriptores = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron suscriptores.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los suscriptores', '', true); });
  }

  exportarExcel() {

  }

}
