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
  clientesEliminados: any = [];
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
  cantidadSeleccionados: number = 0;

  listaDePrecio: string = '';
  categoriaCliente: string = '';
  vendedorCliente: string = '';
  condicionDeVenta: string = '';

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

      if (this.catClientes.length == 0) {
        this.catClientes.push({ catDes: 'SIN DATOS', catCod: '' })
      }
      this.softlandService.getCondVentas().subscribe((res2: any) => {

        this.condVentas = res2;
        this.condVentas.forEach(element => {
          element.cveDes = element.cveCod + ' - ' + element.cveDes;
        });

        if (this.condVentas.length == 0) {
          this.condVentas.push({ cveDes: 'SIN DATOS', cveCod: '' })
        }

        // this.softlandService.getListasPrecio().subscribe((res3: any) => {
        //   this.listasPrecio = res3;
        //   this.listasPrecio.forEach(element => {
        //     element.desLista = element.codLista + ' - ' + element.desLista;
        //   });

        //   if (this.listasPrecio.length == 0) {
        //     this.listasPrecio.push({ desLista: 'SIN DATOS', codLista: '' })
        //   }

        this.softlandService.getVendedores().subscribe((res4: any) => {
          this.vendedores = res4;
          this.vendedores.forEach(element => {
            element.venDes = element.venCod + ' - ' + element.venDes;
          });

          if (this.vendedores.length == 0) {
            this.vendedores.push({ venDes: 'SIN DATOS', venCod: '' })
          }

          this.softlandService.getCargos().subscribe((res5: any) => {
            this.cargos = res5;
            this.cargos.forEach(element => {
              element.carNom = element.carCod + ' - ' + element.carNom;
            });

            if (this.cargos.length == 0) {
              this.cargos.push({ carNom: 'SIN DATOS', carCod: '' })
            }

          }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar cargos', '', true); });
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar vendedores', '', true); });
        // }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar listas de precio', '', true); });
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar condiciones de venta', '', true); });
      this.spinner.hide();
    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al cargar categorías de cliente', '', true); });
  }
  search() {
    this.clientesSeleccionados = [];
    this.clientesEliminados = [];
    this.cantidadSeleccionados = 0;
    this.checkAll = false;
    this.listaDePrecio = this.selectListaPrecio != null ? this.selectListaPrecio : '';
    this.categoriaCliente = this.selectedCatCliente != null ? this.selectedCatCliente : '';
    this.vendedorCliente = this.selectedVendedor != null ? this.selectedVendedor : '';
    this.condicionDeVenta = this.selectedCondVenta != null ? this.selectedCondVenta : '';
    const model = {
      rut: this.searchRut, codAux: this.searchCodAux, nombre: this.searchNombre, tipoBusqueda: 1, listaPrecio: this.listaDePrecio, categoriaCliente: this.categoriaCliente,
      condicionVenta: this.condicionDeVenta, vendedor: this.vendedorCliente, pagina: 1
    }
    this.spinner.show();
    this.softlandService.getClientesAccesos2(model).subscribe((res: any) => {
      debugger
      this.clientesRes = res;
      this.clientes = res;

      this.paginador.startRow = 0;
      this.paginador.endRow = 10;
      this.paginador.sortBy = 'desc';
      this.config = {
        itemsPerPage: this.paginador.endRow,
        currentPage: 1,
        totalItems: this.clientesRes.length > 0 ? this.clientesRes[0].total : 0
      };

      this.clientes = this.clientesRes.slice(this.paginador.startRow, this.paginador.endRow);
      this.showDetail = true;
      this.checkAll = false;
      this.cantidadSeleccionados = 0;
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
  }

  onSelAll(val: any) {
    this.clientesSeleccionados = [];
    this.clientesEliminados = [];

    this.clientesRes.forEach(element => {
      element.checked = val.target.checked
      if (val.target.checked) {
        this.clientesSeleccionados.push(element);
        this.clientesEliminados = [];
      }
    });

    if (this.checkAll) {
      this.cantidadSeleccionados = this.clientesRes[0].total - this.clientesEliminados.length;
    } else {
      this.cantidadSeleccionados = this.clientesSeleccionados.length;
    }
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

  async onSel(val: any, c: any) {
    if (this.checkAll) {
      if (!val.target.checked) {
        let exist = this.clientesEliminados.find(x => x.rutAux == c.rutAux && x.codAux == c.codAux);
        if (exist == null) {
          this.clientesEliminados.push(c);
        }
      } else {
        let exist = this.clientesEliminados.find(x => x.rutAux == c.rutAux && x.codAux == c.codAux);
        if (exist != null) {
          for (let i = 0; i <= this.clientesEliminados.length - 1; i++) {
            if (this.clientesEliminados[i].rutAux == c.rutAux && this.clientesEliminados[i].codAux == c.codAux) {
              this.clientesEliminados.splice(i, 1);
              break;
            }
          }
        }
      }
    }
    const added = this.clientesSeleccionados.find(x => x.rutAux == c.rutAux && x.codAux == c.codAux);

    if (added != null) {
      //remueve
      for (let i = 0; i <= this.clientesSeleccionados.length - 1; i++) {
        if (this.clientesSeleccionados[i].rutAux == c.rutAux && this.clientesSeleccionados[i].codAux == c.codAux) {
          this.clientesSeleccionados.splice(i, 1);
          break;
        }
      }
    } else {
      if (val.target.checked == true) {
        this.clientesRes.forEach(async element => {

          if (c.rutAux == element.rutAux && c.codAux == element.codAux) {
            if (element.accesoEnviado == 1) {
              const response = await this.notificationService.confirmation('', 'El cliente ya posee credenciales de acceso, al ejecutar el proceso de envío, estas se sobrescribirán por las nuevas. ¿Desea añadirlo a la lista de envío?', 'SI', 'NO');
              if (response.isConfirmed) {

                element.checked = val.target.checked
                this.clientesSeleccionados.push(c);
                this.cantidadSeleccionados = this.clientesSeleccionados.length;
              } else {
                element.checked = false;
                val.target.checked = false;
                this.cantidadSeleccionados = this.clientesSeleccionados.length;
              }
            } else {
              element.checked = val.target.checked
              this.clientesSeleccionados.push(c);
              this.cantidadSeleccionados = this.clientesSeleccionados.length;
            }
          }

        });

      }


    }

    if (this.clientesSeleccionados.length == 0) {
      this.checkAll = false;
    }

    if (this.checkAll) {
      this.cantidadSeleccionados = this.clientesRes[0].total - this.clientesEliminados.length;
    } else {
      this.cantidadSeleccionados = this.clientesSeleccionados.length;
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

  async send() {
    this.clientesSinDatos = [];
    if (!this.enviarTodosCargos && this.selectedCargos.length == 0) {
      this.notificationService.warning('Debe seleccionar al menos un cargo para el envío.', '', true);
      return;
    }



    this.spinner.show();
    const user = this.authService.getuser();
    this.clientesSeleccionados.forEach(element => {
      element.encriptadoCliente = btoa(element.codAux + ";" + element.email);
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


    this.spinner.hide();
    let msg = this.checkAll ? 'Se ejecutará el proceso de envío de accesos, se le notificará cuando este finalice. Si alguno de los clientes ya posee credenciales de acceso, estas se sobrescribirán por las nuevas. ¿Desea continuar?' : 'Se ejecutará el proceso de envío de accesos, se le notificará cuando este finalice. ¿Desea continuar?';
    const response = await this.notificationService.confirmation('Enviar accesos', msg);
    if (response.isConfirmed) {
      this.spinner.show();
      let model = {
        value: this.clientesSeleccionados,
        enviaTodos: this.checkAll ? 1 : 0,
        eliminados: this.clientesEliminados
      }
      this.clienteService.enviaAcceso(model).subscribe(res => {
        this.clientes = [];
        this.clientesSeleccionados = [];
        this.clientesEliminados = [];
        this.limpiar();
        this.spinner.hide();
        this.modalService.dismissAll();
        this.notificationService.success('Enviando accesos...', '', true);

        // if (res != "-1") {
        //   this.clientesSinDatos = res;
        //   this.clientes = [];
        //   this.clientesSeleccionados = [];
        //   this.limpiar();
        //   this.modalService.dismissAll();
        //   if(this.clientesSinDatos.length > 0){
        //     this.notificationService.warning('Clientes sin correos electronicos para envio de accesos', '', true);
        //     this.modalService.open(this.modalErrores, { ariaLabelledBy: 'modal-basic-title' });
        //   }else{
        //     this.notificationService.success('Envío de accesos correcto', '', true);
        //   }

        // } else {
        //   this.notificationService.warning('Ocurrio un problema en el envio de accesos, favor revisar el log de envios.', '', true);
        // }

        this.spinner.hide();
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al ejecutar el proceso.', '', true); });
    }

  }

  changePageClientes(event: any) {
    this.spinner.show();
    //this.clientesSeleccionados = [];
    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    const model = {
      rut: this.searchRut, codAux: this.searchCodAux, nombre: this.searchNombre, tipoBusqueda: 1, listaPrecio: this.listaDePrecio, categoriaCliente: this.categoriaCliente,
      conidcionVenta: this.condicionDeVenta, vendedor: this.vendedorCliente, pagina: this.config.currentPage
    }
    this.softlandService.getClientesAccesos2(model).subscribe((res: any) => {
      this.clientesRes = res;
      this.clientes = res;
      this.clientes = this.clientesRes.slice(this.paginador.startRow, this.paginador.endRow);
      this.showDetail = true;
      if (this.checkAll) {

        this.clientes.forEach(element => {

          let eliminado = this.clientesEliminados.filter(x => x.codAux == element.codAux);
          if (eliminado.length > 0) {
            element.checked = false;
          } else {
            element.checked = true;
          }
        });
      } else {
        this.clientes.forEach(element => {
          this.clientesSeleccionados.forEach(elementSelect => {
            if (element.codAux == elementSelect.codAux) {
              element.checked = true;
            }
          });
        });

      }
      this.spinner.hide();
    }, err => { this.spinner.hide(); });
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
    this.clientesEliminados = [];
    this.clientesSeleccionados = [];
    this.cantidadSeleccionados = 0;
    this.checkAll = false;
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
