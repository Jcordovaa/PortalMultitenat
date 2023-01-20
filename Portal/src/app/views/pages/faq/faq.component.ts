import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { PreguntasFrecuentesService } from '../../../shared/services/preguntasfrecuentes.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { PreguntasFrecuentes } from '../../../shared/models/preguntasfrecuentes.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';

@Component({
  selector: 'app-faq',
  templateUrl: './faq.component.html',
  styleUrls: ['./faq.component.scss'],
  animations: [SharedAnimations]
})
export class FaqComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public faqs: PreguntasFrecuentes[] = [];
  public faq: PreguntasFrecuentes = null;
  public noResultsText: string = '';
  public modalTitle: string = 'Nueva Pregunta Frecuente';

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

  constructor(private preguntasFrecuentesService: PreguntasFrecuentesService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.faq = new PreguntasFrecuentes();
  }

  ngOnInit() {
    this.getFaqs();
  }
  
  getFaqs() {
    this.spinner.show();    
    this.preguntasFrecuentesService.getFaqByPage(this.paginador).subscribe((res: PreguntasFrecuentes[]) => {
      this.faqs = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron preguntas frecuentes.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener preguntas frecuentes', '', true); });
  }

  openModal(content, faq: PreguntasFrecuentes) {
    if (faq != null) {

      //obtiene cupon de bd
      this.spinner.show();

      this.preguntasFrecuentesService.getPreguntaFrecuente(faq.idPregunta).subscribe((res: PreguntasFrecuentes) => {
        this.faq = res;
        this.modalTitle = 'Editar Pregunta Frecuente';
        this.spinner.hide();    
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener pregunta frecuente' ,'' , true); });
      
    } else {
      this.faq = new PreguntasFrecuentes();
      this.modalTitle = 'Nueva Pregunta Frecuente';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async save(content: any) {
    this.faq.estado = this.faq.estado ? 1 : 0;

    if (this.faq.idPregunta == 0) {

      this.spinner.show();

      this.preguntasFrecuentesService.save(this.faq).subscribe((res: PreguntasFrecuentes) => {
        this.notificationService.success('Correcto', '', true);
        this.getFaqs();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al grabar pregunta frecuente', '', true); });

    } else {
      this.spinner.show();

      this.preguntasFrecuentesService.edit(this.faq).subscribe(res => {
        this.notificationService.success('Editado correctamente', '', true);
        this.getFaqs();
        this.modalService.dismissAll();
        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al editar pregunta frecuente', '', true); });

    }    
  }

  async delete (item: PreguntasFrecuentes) {
    const response = await this.notificationService.confirmation('Eliminar Pregunta Frecuente', '¿Confirma eliminar esta pregunta frecuente?');
    if (response.isConfirmed) {
      this.preguntasFrecuentesService.delete(item.idPregunta).subscribe(res => {
        this.notificationService.success('Pregunta Frecuente eliminada', '', true);
        this.getFaqs();
      }, err => { this.notificationService.error('Ocurrió un error al eliminar pregunta frecuente', '', true); });
    }
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getFaqs()
  }

}
