import { Component, OnInit, ViewChild } from '@angular/core';
import { NotificationService } from '../../../shared/services/notificacion.service';
import { NgxSpinnerService } from "ngx-spinner";
import { Utils } from 'src/app/shared/utils';
import { ConfiguracionSoftlandService } from 'src/app/shared/services/configuracionsoftland.service';
import { ColumnMode, SelectionType } from '@swimlane/ngx-datatable';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { Paginator } from 'src/app/shared/models/paginator.model';
import { AuthService } from 'src/app/shared/services/auth.service';
import { ConfiguracionCorreoService } from 'src/app/shared/services/configuracioncorreo.service';



@Component({
  selector: 'app-send-access',
  templateUrl: './send-access.component.html',
  styleUrls: ['./send-access.component.scss']
})
export class SendAccessComponent implements OnInit {

  @ViewChild('modalErrores') modalErrores: any;
  searchRut: string = '';
  searchCodAux: string = '';
  searchNombre: string = '';
  tipoBusqueda: number = 1;
  clientesRes: any = [];
  clientes: any = [];
  showDetail: boolean = false;
  clientesSeleccionados: any = [];
  selectedVendedor: any;
  vendedores: any = [];
  selectedCondVenta: any;
  condVentas: any = [];
  cargos: any = [];
  selectedCatCliente: any;
  catClientes: any = [];
  selectListaPrecio: any;
  listasPrecio: any = [];
  contactosCliente: any = [];
  checkAll: boolean = false
  contactosSeleccionados: any = [];
  showAlert: boolean = false;
  checkCorreoManual: number = 0;
  emailManual: string = '';
  cantidadCorreosDia: number = 0;
  cantidadCorreosEnviados: number = 0;
  clienteContacto: any = null;
  ClientecontactosRes: any = [];
  selectedCargos: any = [];
  enviarTodosContactos: boolean = false;
  enviarFicha: boolean = false;
  enviarTodosCargos: boolean = true;
  clientesSinDatos: any = [];

  public config: any;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  public configContactos: any;
  public paginadorContactos: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };


  constructor(private notificationService: NotificationService,
    private spinner: NgxSpinnerService,
    private utils: Utils,
    private softlandService: ConfiguracionSoftlandService,
    private modalService: NgbModal,
    private clienteService: ClientesService, private authService: AuthService, private configuracionCorreoService: ConfiguracionCorreoService) { }

  ngOnInit(): void {
    this.cargarSelects();
  }

  validaRut() {
    if (this.searchRut != "" && this.searchRut != null) {
      if (this.utils.isValidRUT(this.searchRut)) {
        this.searchRut = this.utils.checkRut(this.searchRut);
      } else {
        this.notificationService.warning('Rut ingresado no es valido', '', true);
      }
    }
  }


  cargarSelects() {
    this.spinner.show();
    this.softlandService.getCatClientes().subscribe((res: any) => {
      this.catClientes = res;
      this.catClientes.forEach(element => {
        element.catDes = element.catCod + ' - ' + element.catDes;
      });
      this.softlandService.getCondVentas().subscribe((res2: any) => {

        this.condVentas = res2;
        this.condVentas.forEach(element => {
          element.cveDes = element.cveCod + ' - ' + element.cveDes;
        });

        this.softlandService.getListasPrecio().subscribe((res3: any) => {
          this.listasPrecio = res3;
          this.listasPrecio.forEach(element => {
            element.desLista = element.codLista + ' - ' + element.desLista;
          });
          this.softlandService.getVendedores().subscribe((res4: any) => {
            this.vendedores = res4;
            this.vendedores.forEach(element => {
              element.venDes = element.venCod + ' - ' + element.venDes;
            });
            this.softlandService.getCargos().subscribe((res5: any) => {
              this.cargos = res5;
              this.cargos.forEach(element => {
                element.carNom = element.carCod + ' - ' + element.carNom;
              });
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar cargos', '', true); });
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar vendedores', '', true); });
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar listas de precio', '', true); });
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar condiciones de venta', '', true); });
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar categorias de cliente', '', true); });
  }
  search() {
    this.clientesSeleccionados = [];
    const model = { rut: this.searchRut, codAux: this.searchCodAux, nombre: this.searchNombre, tipoBusqueda: this.tipoBusqueda }
    this.spinner.show();
    this.softlandService.getClientesAccesos2(model).subscribe((res: any) => {
      this.clientesRes = res;
      this.clientes = res;

      
      if (this.selectedCatCliente != null) {
        this.clientesRes = this.clientesRes.filter(x => x.codCatCliente == this.selectedCatCliente)
      }
      if (this.selectListaPrecio != null) {
        this.clientesRes = this.clientesRes.filter(x => x.codLista == this.selectListaPrecio)
      }
      if (this.selectedCondVenta != null) {
        
        this.clientesRes = this.clientesRes.filter(x => x.codCondVenta == this.selectedCondVenta)
      }
      if (this.selectedVendedor != null) {
        this.clientesRes = this.clientesRes.filter(x => x.codVendedor == this.selectedVendedor)
      }

      this.paginador.startRow = 0;
      this.paginador.endRow = 10;
      this.paginador.sortBy = 'desc';
      this.config = {
        itemsPerPage: this.paginador.endRow,
        currentPage: 1,
        totalItems: this.clientesRes.length
      };

      this.clientes = this.clientesRes.slice(this.paginador.startRow, this.paginador.endRow);
      this.showDetail = true;
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
  }

  onSelAll(val: any) {
    this.clientesSeleccionados = [];

    this.clientesRes.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.clientesSeleccionados.push(element);
      }
    });
  }

  openModalContacto(content, cliente: any) {

    this.ClientecontactosRes = [];
    this.contactosCliente = [];
    this.clienteContacto = cliente;
    this.ClientecontactosRes = cliente.contactos;
    this.paginadorContactos.startRow = 0;
    this.paginadorContactos.endRow = 10;
    this.paginadorContactos.sortBy = 'desc';
    this.configContactos = {
      itemsPerPage: this.paginadorContactos.endRow,
      currentPage: 1,
      totalItems: this.clienteContacto.contactos.length
    };

    if (this.ClientecontactosRes.length == 0 || this.ClientecontactosRes == null) {
      this.checkCorreoManual = 1;
    }
    this.contactosCliente = this.ClientecontactosRes.slice(this.paginadorContactos.startRow, this.paginadorContactos.endRow);
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });

  }

  onSel(val: any, c: any) {
    const added = this.clientesSeleccionados.find(x => x.rut == c.rut && x.codAux == c.codAux);
    if (added != null) {
      //remueve
      for (let i = 0; i <= this.clientesSeleccionados.length - 1; i++) {
        if (this.clientesSeleccionados[i].rut == c.rut && this.clientesSeleccionados[i].codAux == c.codAux) {
          this.clientesSeleccionados.splice(i, 1);
          break;
        }
      }
    } else {
      this.clientesRes.forEach(element => {
        if (c.rut == element.rut && c.codAux == element.codAux) {
          element.checked = val.target.checked
        }
      });
      this.clientesSeleccionados.push(c);
    }

    if (this.clientesSeleccionados.length == 0) {
      this.checkAll = false;
    }
  }

  onChange(item: any) {

    const added = this.contactosSeleccionados.find(x => x.correoContacto == item.correoContacto);
    if (added == null) {
      this.contactosSeleccionados.push(item)
    } else {
      for (let i: number = 0; i <= this.contactosSeleccionados.length - 1; i++) {
        if (this.contactosSeleccionados[i].correoContacto == item.correoContacto) {
          this.contactosSeleccionados.splice(i, 1);
          break;
        }
      }
    }
  }


  openModalSend(content) {
    if (this.clientesSeleccionados.length == 0) {
      this.notificationService.warning('Debe seleccionar un cliente para enviar accesos.', '', true);
      return;
    }
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  send() {
    this.clientesSinDatos = [];
    if (!this.enviarTodosCargos && this.selectedCargos.length == 0) {
      this.notificationService.warning('Debe seleccionar almenos un cargo para el envio.', '', true);
      return;
    }



    this.spinner.show();
    const user = this.authService.getuser();
    this.clientesSeleccionados.forEach(element => {
      element.encriptadoCliente = btoa(element.codAux + ";" + element.correo);
      element.correoUsuario = user.email;
    });

    let cargos = '';
    if (!this.enviarTodosCargos) {
      cargos = this.selectedCargos.length > 0 ? this.selectedCargos.reduce((accumulator, item) => {
        return `${accumulator};${item}`;
      }) : null;
    } else {
      this.cargos.forEach(element => {
        if (cargos == '') {
          cargos = element.carCod;
        } else {
          cargos = cargos + ';' + element.carCod;
        }

      });
    }




    this.clientesSeleccionados[0].codCargo = cargos;
    this.clientesSeleccionados[0].enviarTodosContactos = this.enviarTodosContactos;
    this.clientesSeleccionados[0].enviarFicha = this.enviarFicha;

    this.configuracionCorreoService.getConfigCorreos().subscribe((res: any) => {

      this.cantidadCorreosDia = res[0].cantidadCorreosAcceso;
      this.configuracionCorreoService.getCantidadEnviadaDia().subscribe((res: number) => {

        this.cantidadCorreosEnviados = res;
        if ((this.clientesSeleccionados.length + this.cantidadCorreosEnviados) > this.cantidadCorreosDia) {
          this.spinner.hide();
          this.notificationService.warning('Solo dispone de ' + (this.cantidadCorreosDia - this.cantidadCorreosEnviados) + ' correos para enviar accesos el día de hoy', '', true);
        } else {
          this.clienteService.enviaAcceso(this.clientesSeleccionados).subscribe(res => {
            this.spinner.hide();
            if (res != "-1") {
              this.clientesSinDatos = res;
              this.clientes = [];
              this.clientesSeleccionados = [];
              this.limpiar();
              this.modalService.dismissAll();
              if(this.clientesSinDatos.length > 0){
                this.notificationService.warning('Clientes sin correos electronicos para envio de accesos', '', true);
                this.modalService.open(this.modalErrores, { ariaLabelledBy: 'modal-basic-title' });
              }else{
                this.notificationService.success('Envío de accesos correcto', '', true);
              }
              
            } else {
              this.notificationService.warning('Ocurrio un problema en el envio de accesos, favor revisar el log de envios.', '', true);
            }
            
            this.spinner.hide();
          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cantidad de correos enviados.', '', true); });
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener cantidad de correos por dia', '', true); });



  }

  changePageClientes(event: any) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    this.clientes = this.clientesRes.slice(this.paginador.startRow, this.paginador.endRow);
  }


  limpiar() {
    this.searchCodAux = '';
    this.searchNombre = '';
    this.selectListaPrecio = null;
    this.selectedCatCliente = null;
    this.selectedCondVenta = null;
    this.selectedVendedor = null;
    this.searchRut = '';
    this.tipoBusqueda = 1;
    this.clientes = [];
  }

  guardarCorreoContacto() {

    if (this.checkCorreoManual == 1) {
      if (this.emailManual == '') {
        this.notificationService.warning('Debe ingresar un correo', '', true);
      } else {
        const emailRegex = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;
        if (emailRegex.test(this.emailManual)) {
          this.clienteContacto.correo = this.emailManual;
          this.clientes.forEach(element => {
            if (element.codAux == this.clienteContacto.codAux && element.rut == this.clienteContacto.rut) {
              element.correo = this.clienteContacto.correo;
            }
          });
          this.paginadorContactos.startRow = 0;
          this.paginadorContactos.endRow = 10;
          this.paginadorContactos.sortBy = 'desc';
          this.configContactos.currentPage = 1;
          this.modalService.dismissAll();
        } else {
          this.notificationService.warning('Debe ingresar un correo valido', '', true);
        }


      }
    }
    else {
      if (this.contactosSeleccionados == null) {
        this.notificationService.warning('Debe seleccionar un correo', '', true);
      } else {
        this.clienteContacto.correo = this.contactosSeleccionados[0].correo;
        this.clientes.forEach(element => {
          if (element.codAux == this.clienteContacto.codAux && element.rut == this.clienteContacto.rut) {
            element.correo = this.clienteContacto.correo;
          }
        });
        this.paginadorContactos.startRow = 0;
        this.paginadorContactos.endRow = 10;
        this.paginadorContactos.sortBy = 'desc';
        this.configContactos.currentPage = 1;
        this.modalService.dismissAll();
      }

    }
  }
}
