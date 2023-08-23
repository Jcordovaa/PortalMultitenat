import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from "../../../shared/services/auth.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
// import { ClientesService } from '../../../shared/services/clientes.service';
import { VentasService } from '../../../shared/services/ventas.service';
import { Utils } from '../../../shared/utils';
import { TbkRedirect } from '../../../shared/enums/TbkRedirect';
import { NgxSpinnerService } from "ngx-spinner";
import { date } from 'ngx-custom-validators/src/app/date/validator';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Paginator } from 'src/app/shared/models/paginator.model';
import { ClientesService } from 'src/app/shared/services/clientes.service';
import { Cliente } from 'src/app/shared/models/clientes.model';
import { CategoriaClienteDTO, CondicionVentaDTO, ListaPrecioDTO, VendedorDTO } from 'src/app/shared/models/softland.model';
import { SoftlandService } from 'src/app/shared/services/softland.service';

const I18N_VALUES = {
    en: {
        weekdays: ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'],
        months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    },
    es: {
        weekdays: ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa', 'Do'],
        months: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
    }
};

@Component({
    selector: 'app-excluded',
    templateUrl: './excluded.component.html',
    styleUrls: ['./excluded.component.scss'],
    animations: [SharedAnimations]
})
export class ExcludedComponent implements OnInit {

    public viewMode: 'list' | 'grid' = 'list';
    public img: string = 'assets/images/Clientes-excluidos.png';
    showDetail: boolean = false;
    searchRut: string = '';
    searchRutCliente: string = '';
    searchNombreCliente: string = '';
    selectedEstado: number;
    selectedEstadoSearch: number;
    selectedCurso: number;
    selectedCursoSearch: number;
    selectedEstadoCliente: number;
    selectedCursoCliente: number;
    loading: boolean = false;
    fechaNacimiento: NgbDateStruct;
    esNuevo: boolean = false;
    esNuevoTexto: boolean = false;
    tituloMantenedor: string = 'Cliente';
    public totalItems: number = 0;
    ClientesExcludios: any = [];
    ClientesExcluidosRes: any = [];
    noResultsText: string = 'No existen Clientes excluidos';
    selectedCondicionVenta: any = null;
    selectedListaPrecio: any = null;
    selectedCatCliente: any = null;
    selectedVendedor: any = null;
    clientesResp: any[] = [];
    condicionesVenta : CondicionVentaDTO[] = [];
    listasPrecio : ListaPrecioDTO[] = [];
    categoriasCliente : CategoriaClienteDTO[] = [];
    vendedores : VendedorDTO[] = [];

    public config: any;
    public paginador: Paginator = {
        startRow: 0,
        endRow: 10,
        sortBy: 'desc',
        search: ''
    };



    constructor(private ngbDatepickerConfig: NgbDatepickerConfig, private authService: AuthService, private activatedRoute: ActivatedRoute,
        private notificationService: NotificationService, private ngbDatepickerI18n: NgbDatepickerI18n, private utils: Utils, private spinner: NgxSpinnerService,
        private clienteService: ClientesService, private softlandService : SoftlandService,
        private modalService: NgbModal, private router: Router) {

        this.ngbDatepickerConfig.firstDayOfWeek = 1;

        this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
            return I18N_VALUES['es'].weekdays[weekday - 1];
        };

        this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
            return I18N_VALUES['es'].months[months - 1];
        };
    }

    ngOnInit(): void {
        // const user = this.authService.getUserPortal();
        // if (user) {
        //     this.spinner.show();

        //     const rut: string[] = user.rut.split('-');
        //     const rut2: string = rut[0].replace('.', '').replace('.', '')
        //     const model = { nombre: rut2 }
            this.getClientesExcluidos();
            this.getCategoriasCliente();
            //this.getListasPrecio();
            this.getCondicionesVenta();
            this.getVendedores();
        //}

    }

    getCategoriasCliente(){
        this.spinner.show();
        this.softlandService.getCategoriasCliente().subscribe((resp: CategoriaClienteDTO[]) => {
          this.categoriasCliente = resp;
        }, err => {
          this.spinner.hide();
        });
      }
    
      getListasPrecio(){
        this.spinner.show();
        this.softlandService.getListasPrecio().subscribe((resp: ListaPrecioDTO[]) => {         
          this.listasPrecio = resp;
        }, err => {
          this.spinner.hide();
        });
      }
    
      getVendedores(){
        this.spinner.show();
        this.softlandService.getVendedores().subscribe((resp: VendedorDTO[]) => {
          this.vendedores = resp;
        }, err => {
          this.spinner.hide();
        });
      }
    
      getCondicionesVenta(){
        this.spinner.show();
        this.softlandService.getCondicionesVenta().subscribe((resp: CondicionVentaDTO[]) => {
          this.condicionesVenta = resp;
        }, err => {
          this.spinner.hide();
        });
      }

    getClientesExcluidos() {
        this.spinner.show();
        this.clienteService.getClientesExcluidos().subscribe((res: any[]) => {
            this.ClientesExcluidosRes = res;
            this.paginador.endRow = 10;
            this.paginador.startRow = 0;
            this.config = {
                itemsPerPage: this.paginador.endRow,
                currentPage: 1,
                totalItems: this.ClientesExcluidosRes.length
            };

            this.ClientesExcludios = this.ClientesExcluidosRes.slice(this.paginador.startRow, this.paginador.endRow);
            this.totalItems = res.length;
            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.success('Ocurrió un problema al obtener  clientes, intente nuevamente.', '', true); });
    }

    delete(Cliente: any) {
        this.spinner.show();
        this.clienteService.deleteExcluido(Cliente).subscribe((res: any[]) => {
            this.getClientesExcluidos();
            this.notificationService.success('Cliente removido de la lista de excluidos', '', true);
            this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.success('Ocurrió un problema al eliminar , intente nuevamente.', '', true); });
    }

    changePage(event: any) {
        this.paginador.startRow = ((event - 1) * 10);
        this.paginador.endRow = (event * 10);
        this.paginador.sortBy = 'desc';
        this.config.currentPage = event;
        this.ClientesExcludios = this.ClientesExcluidosRes.slice(this.paginador.startRow, this.paginador.endRow);
    }


    limpiarFiltros() {
        this.selectedEstado = 0;
        this.searchRut = '';
        this.searchRutCliente = '';
        this.searchNombreCliente= '';
        this.showDetail = false;
        this.clientesResp = [];
    }

    validaRutCliente() {

        if (this.searchRut != "" && this.searchRut != null) {
            if (this.utils.isValidRUT(this.searchRut)) {
                this.searchRut = this.utils.checkRut(this.searchRut);
            } else {
                this.notificationService.warning('Rut ingresado no es valido', '', true);
            }
        }
    }


    search() {

        var validaFiltro = false;
        // if (this.searchRut != "") { validaFiltro = true; }
        if (this.searchRut != "") { validaFiltro = true; }
        if (this.searchNombreCliente != "") { validaFiltro = true; }
        if (this.selectedCursoSearch != null) { validaFiltro = true; }
        if (this.selectedEstadoSearch != null) { validaFiltro = true; }
        if (this.selectedCatCliente != null) { validaFiltro = true; }
        if (this.selectedCondicionVenta != null) { validaFiltro = true; }
        if (this.selectedListaPrecio != null) { validaFiltro = true; }
        if (this.selectedVendedor != null) { validaFiltro = true; }

        if (!validaFiltro) {
            this.notificationService.warning('Debe ingresar al menos un filtro de búsqueda', '', true);
            return;
        }
        this.spinner.show();
        this.showDetail = false;

        var codaux: string = "";


        var nombreCliente: string = "";
        if (this.searchNombreCliente != "") {
            nombreCliente = this.searchNombreCliente;
        }

        var condicionVenta: string = '';
        if (this.selectedCondicionVenta != null) {
            condicionVenta = this.selectedCondicionVenta;
        }

        var listaPrecio: string = '';
        if (this.selectedListaPrecio != null) {
            listaPrecio = this.selectedListaPrecio;
        }

        var vendedor: string = '';
        if (this.selectedVendedor != null) {
            vendedor = this.selectedVendedor;
        }

        var catCliente: string = '';
        if (this.selectedCatCliente != null) {
            catCliente = this.selectedCatCliente;
        }

        const model = { codAux: codaux, nombre: nombreCliente, listaPrecio: listaPrecio, categoriaCliente: catCliente, vendedor: vendedor, condicionVenta: condicionVenta, rut: this.searchRut }

        this.clienteService.getClientesFiltros(model).subscribe((res: any) => {

            this.limpiarFiltros();
            
            this.clientesResp = res;
            
            if (this.clientesResp.length > 0) {
                this.showDetail = true;
            } else {
                this.notificationService.info('No se encontró información para los filtros seleccionados.', '', true);
            }

            this.spinner.hide();

        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un problema al obtener cliente, intente nuevamente.', '', true); });

    }

    excluir(cliente: any) {
        debugger
        const model = { rutCliente: cliente.rut, codAuxCliente: cliente.codAux, nombreCliente: cliente.nombre }
        this.spinner.show();
        var existe: boolean;
        this.ClientesExcluidosRes.forEach(element => {
            if (model.rutCliente == element.rutCliente && model.codAuxCliente == element.codAuxCliente) {

                existe = true;
            }
        });
        if (existe == true) {
            this.notificationService.warning('Cliente ya excluido.', '', true);
            this.spinner.hide();
        } else {
            this.clienteService.saveExcluido(model).subscribe((res: any) => {
                if (res != null) {
                    this.getClientesExcluidos();
                    this.notificationService.success('Cliente añadido a la lista de excluidos.', '', true);
                    this.spinner.hide();
                } else {
                    this.notificationService.warning('Ocurrió un problema al excluir cliente, intente nuevamente.', '', true);
                    this.spinner.hide();
                }

            }, err => { this.spinner.hide(); this.notificationService.success('Ocurrió un problema al excluir cliente, intente nuevamente.', '', true); });
        }

    }



    openModal(content) {
        this.searchRut = '';
        this.searchNombreCliente = '';
        this.selectedCondicionVenta = null;
        this.selectedListaPrecio = null;
        this.selectedCatCliente = null;
        this.selectedVendedor = null;
        this.clientesResp = [];
        this.showDetail = false;
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg', backdrop: 'static' });

    }

    closeModal() {
        this.modalService.dismissAll();
        window.location.href = window.location.origin + '/#/payment/accounts-state'

    }

}
