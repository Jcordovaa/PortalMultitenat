import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClientesService } from '../../../shared/services/clientes.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from "ngx-spinner";
import { Cliente} from '../../../shared/models/clientes.model';
import { Paginator } from '../../../shared/models/paginator.model';
import { NotificationService } from '../../../shared/services/notificacion.service';


@Component({
  selector: 'app-clientes',
  templateUrl: './clientes.component.html',
  styleUrls: ['./clientes.component.scss'],
  animations: [SharedAnimations]
})
export class ClientesComponent implements OnInit {

  public viewMode: 'list' | 'grid' = 'list';
  public modalTitle: string = 'Nuevo Cliente';
  public clientes: Cliente[] = [];
  public cliente: Cliente = null;
  public noResultsText: string = '';

  public newPassword1: string = '';
  public newPassword2: string = '';
  public newEmail1: string = '';
  public newEmail2: string = '';

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

  constructor(private clientesService: ClientesService, private spinner: NgxSpinnerService,
    private modalService: NgbModal, private notificationService: NotificationService) {
      this.cliente = new Cliente();
  }

  ngOnInit(): void {
    this.getClientes();
  }

  getClientes() {
    this.spinner.show();    
    this.clientesService.getClientesByPage(this.paginador).subscribe((res: Cliente[]) => {
      this.clientes = res;

      if (res.length > 0) {
        this.totalItems = res[0].totalFilas;
      } else {
        this.noResultsText = 'No se encontraron clientes.'
        this.totalItems = 0;
      }

      this.config = {
        itemsPerPage: 10,
        currentPage: this.p,
        totalItems: this.totalItems
      };

      this.loaded = true;

      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener los clientes', '', true); });
  }

  openModal(content, cliente: Cliente) {
    if (cliente != null) {

      //obtiene banner de bd
      this.spinner.show();

      this.clientesService.getCliente(cliente.idCliente).subscribe((res: Cliente) => {
        this.cliente = res;
        this.spinner.hide();
        this.modalTitle = 'Detalle de Cliente';
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener Cliente' ,'' , true); });
      
    } else {
      this.cliente = new Cliente();
      this.modalTitle = 'Nuevo Cliente';
    }
    
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  changePage(event: any) {
    this.p = event
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    
    this.getClientes()
  }

  close() {
    this.modalService.dismissAll();
  }

  openModalChange(content, item: Cliente) {
    this.cliente = item;
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  async onChangePassword() {
    if (this.newPassword1 != this.newPassword2) {
      this.notificationService.error('Contraseñas no coinciden', '', true);
      return;
    }

    const response = await this.notificationService.confirmation('Cambiar contraseña', `¿Confirma cambiar la contraseña al cliente ${this.cliente.nombre}?`);
    if (response.isConfirmed) {
      this.spinner.show();
      this.cliente.claveAcceso = this.newPassword1;

      this.clientesService.changePassword(this.cliente).subscribe(res => {
        this.spinner.hide();
        this.notificationService.success('Correcto', '', true);
        this.newPassword1 = '';
        this.newPassword2 = '';
        this.modalService.dismissAll();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cambiar contraseña', '', true); })
    }
  }

  async onChangeEmail() {
    if (this.newEmail1 != this.newEmail2) {
      this.notificationService.error('Correos no coinciden.', '', true);
      return;
    }

    const response = await this.notificationService.confirmation('Cambiar Correo', `¿Confirma cambiar el correo al cliente ${this.cliente.nombre}?`);
    if (response.isConfirmed) {
      this.spinner.show();
      this.cliente.email = this.newEmail1;

      this.clientesService.changeEmail(this.cliente).subscribe(res => {
        this.spinner.hide();
        this.notificationService.success('Correcto', '', true);
        this.newEmail1 = '';
        this.newEmail2 = '';
        this.modalService.dismissAll();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cambiar correo', '', true); })
    }
  }

}
