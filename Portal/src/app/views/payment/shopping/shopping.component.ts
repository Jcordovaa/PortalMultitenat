import { Component, OnInit } from '@angular/core';
import { NgbCalendar, NgbModal, NgbDatepickerI18n, NgbDatepickerConfig, NgbDateStruct, NgbDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from "../../../shared/services/auth.service";
import { NotificationService } from '../../../shared/services/notificacion.service';
import { ClientesService } from '../../../shared/services/clientes.service';
import { Utils } from '../../../shared/utils';
import { NgxSpinnerService } from "ngx-spinner";
import * as FileSaver from "file-saver";
import * as XLSX from 'xlsx';
import { ConfiguracionPagoClientesService } from 'src/app/shared/services/configuracionpagoclientes.service';
import { ConfiguracionPagoCliente, ConfiguracionPortal } from 'src/app/shared/models/configuracioncobranza.model';
import { fakeAsync } from '@angular/core/testing';
import { formatDate } from '@angular/common';
import { Paginator } from 'src/app/shared/models/paginator.model';
import { ConfiguracionDiseno } from 'src/app/shared/models/configuraciondiseno.model';
import { ConfiguracionDisenoService } from 'src/app/shared/services/configuraciondiseno.service';
import { SoftlandService } from 'src/app/shared/services/softland.service';
import { Documento } from 'src/app/shared/models/documentos.model';


interface AutoCompleteModel {
  value: any;
  display: string;
}


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

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Component({
  selector: 'app-shopping',
  templateUrl: './shopping.component.html',
  styleUrls: ['./shopping.component.scss']
})
export class ShoppingComponent implements OnInit {

  tituloDetalle: string = '';
  loading: boolean = false;
  loadingMisCompras: boolean = false;
  configuracion: ConfiguracionPortal = new ConfiguracionPortal();
  configuracionPagos: ConfiguracionPagoCliente = new ConfiguracionPagoCliente();
  compras: any = [];
  selectedTabMonedas: number = 1;
  existComprasSegundaMoneda: boolean = false;
  notasVenta: any = [];
  notasVentaResp: any = [];
  comprasResp: any = [];
  guiasResp: any = [];
  guias: any = [];
  despachos: any = [];
  detalleDespacho: any = [];
  productos: any = [];
  productosResp: any = [];
  dateDesde: NgbDateStruct;
  dateHasta: NgbDateStruct;
  valorUfActrual: number = 0;
  fechaActual = new Date();
  selected: any = [];
  tiposDocs: any = [];
  estados: any = [];
  selectedEstado: any = null;
  notaVenta: number = null;
  folio: number = null;
  total: string = "$ 0";
  totalPagar: number = 0;
  showDetail: boolean = false;
  showDetailProducto: boolean = false;
  showDetailNv: boolean = false;
  showDetailGuia: boolean = false;
  detalleCab: any = null;
  detalleDet: any = [];
  estadoDoc: string = '';
  NotaVentaDetalle: any = null;
  estadoGuia: string = '';
  fecha: NgbDateStruct;
  public muestraDocFinalizados: boolean = false;
  public muestraDocPendientes: boolean = false;
  public muestraProductosCompras: boolean = false;
  public muestraGuias: boolean = false;
  public muestraCabecera: boolean = true;
  public cantidadCompras: number = 0;
  public totalCompras: number = 0;
  public cantidadNV: number = 0;
  public totalNV: number = 0;
  public totalGuias: number = 0;
  public CantidadProductos: number = 0;
  public cantidadGuias: number = 0;
  contactosCliente: any = [];
  contactosSeleccionados: any = [];
  tituloEnvio = '';
  public otrosCorreo: AutoCompleteModel[] = [];
  public enviaPdf: boolean = false;
  public enviaXml: boolean = false;
  public codigoProd: string = null;
  public nomProd: string = null;
  esFactuarada: boolean = false;
  tipoDocEnvio: number = 1;
  tipoDescarga: number = 1;
  documentoAEnviar: any = null;
  public tipoFecha: number = 1;
  public config: any;
  public paginador: Paginator = {
    startRow: 0,
    endRow: 10,
    sortBy: 'desc',
    search: ''
  };

  varloUfOrigen: number = 0;


  noResultText: string = 'No se encontraron documentos con los filtros seleccionados.'

  cantidadRecuadrosDocumentos: number = 0;
  existModuloInventario: boolean = true;
  existModuloNotaVenta: boolean = false;
  existModuloContabilidad: boolean = false;

  productoVenta: string;
  public configDiseno: ConfiguracionDiseno = new ConfiguracionDiseno(); //FCA 01-07-2022

  constructor(private ngbDatepickerConfig: NgbDatepickerConfig, private disenoSerivce: ConfiguracionDisenoService,//FCA 01-07-2022
    private clientesService: ClientesService, private softlandService: SoftlandService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private ngbDatepickerI18n: NgbDatepickerI18n,
    private utils: Utils,
    private spinner: NgxSpinnerService,
    private modalService: NgbModal,
    private configuracionService: ConfiguracionPagoClientesService) {

    this.ngbDatepickerConfig.firstDayOfWeek = 1;
    this.ngbDatepickerI18n.getWeekdayShortName = (weekday: number) => {
      return I18N_VALUES['es'].weekdays[weekday - 1];
    };
    this.ngbDatepickerI18n.getMonthShortName = (months: number) => {
      return I18N_VALUES['es'].months[months - 1];
    };

  }

  ngOnInit(): void {
    this.spinner.show();
    const configuracionCompletaPortal = this.configuracionService.getAllConfiguracionPortalLs();
    if (configuracionCompletaPortal != null) {
      this.configDiseno = configuracionCompletaPortal.configuracionDiseno;
      this.configuracion = configuracionCompletaPortal.configuracionPortal;
      this.configuracionPagos = configuracionCompletaPortal.configuracionPagoCliente;
      this.existModuloInventario = configuracionCompletaPortal.existModuloInventario;
      this.existModuloNotaVenta = configuracionCompletaPortal.existModuloNotaVenta;
      this.existModuloContabilidad = configuracionCompletaPortal.existModuloContabilidad;
    }

    const user = this.authService.getuser();
    if (user) {
      if (!this.existModuloInventario) {
        this.clientesService.getAllDocumentosContabilizados(user.codAux).subscribe((res: Documento[]) => {

          this.compras = res.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
          this.comprasResp = res;
          this.config = {
            itemsPerPage: this.paginador.endRow,
            currentPage: 1,
            totalItems: this.comprasResp.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
          };
          let compraSsegundaMoneda = this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);

          if (compraSsegundaMoneda.length > 0) {
            this.existComprasSegundaMoneda = true;
          }
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener compras', '', true); });
      } else {
        this.clientesService.getDashboardCompras(user.codAux).subscribe((res: any) => {

          if (res.length > 0) {
            res.forEach(element => {
              if (element.tipo == "NV") {
                this.cantidadNV = element.cantidadDocumentos;
                this.totalNV = element.montoTotal;
              } else if (element.tipo == "COMPRAS") {
                this.cantidadCompras = element.cantidadDocumentos;
                this.totalCompras = element.montoTotal;
              } else if (element.tipo == "PRODUCTOS") {
                this.CantidadProductos = element.cantidadDocumentos;
              } else if (element.tipo == "GUIAS") {
                this.cantidadGuias = element.cantidadDocumentos;
                this.totalGuias = element.montoTotal;
              }
            });
          }

          this.cantidadRecuadrosDocumentos = 0;
          if (this.configuracion.muestraComprasFacturadas == 1) { this.cantidadRecuadrosDocumentos += 1; }
          if (this.configuracion.muestraPendientesFacturar == 1 && this.existModuloNotaVenta) { this.cantidadRecuadrosDocumentos += 1; }
          if (this.configuracion.muestraProductos == 1) { this.cantidadRecuadrosDocumentos += 1; }
          if (this.configuracion.muestraGuiasPendientes == 1) { this.cantidadRecuadrosDocumentos += 1; }
          this.loadingMisCompras = true;
          this.spinner.hide();
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al datos', '', true); });
      }
    } else {
      this.authService.signoutExpiredToken();
    }


  }


  filterContabilizados(moneda: number) {
    let data: any = Object.assign([], this.comprasResp);

    if (moneda == 1) {
      data = data.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada);
    } else if (moneda == 2) {
      data = data.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
    }

    if (this.notaVenta != null) {
      data = data.filter(x => x.nro == this.notaVenta)
    }

    if (this.selectedEstado != null) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.tipoFecha == 1) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.fechaEmision) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.fechaEmision) <= fHasta)
      }
    }

    if (this.tipoFecha == 2) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.fechaVcto) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.fechaVcto) <= fHasta)
      }
    }



    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: data.length
    };

    this.compras = data.slice(this.paginador.startRow, this.paginador.endRow);
  }



  filter(moneda: number) {
    let data: any = Object.assign([], this.comprasResp);

    if (moneda == 1) {
      data = data.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada);
    } else if (moneda == 2) {
      data = data.filter(x => x.codMon != this.configuracionPagos.monedaUtilizada);
    }

    if (this.notaVenta != null) {
      data = data.filter(x => x.nro == this.notaVenta)
    }
    if (this.selectedEstado != null) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.tipoFecha == 1) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.femision) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.femision) <= fHasta)
      }
    }

    if (this.tipoFecha == 2) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.fvencimiento) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.fvencimiento) <= fHasta)
      }
    }



    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: data.length
    };

    this.compras = data.slice(this.paginador.startRow, this.paginador.endRow);
  }

  filterNv() {
    let data: any = Object.assign([], this.notasVentaResp);

    if (this.notaVenta != null) {
      data = data.filter(x => x.nvNumero == this.notaVenta)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }

    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: data.length
    };
    this.notasVenta = data.slice(this.paginador.startRow, this.paginador.endRow);;
  }

  filterPr() {

    let data: any = Object.assign([], this.productosResp);

    if (this.folio != null) {
      data = data.filter(x => x.folio == this.folio)
    }
    if (this.codigoProd != null) {
      data = data.filter(x => x.codProd == this.codigoProd)
    }
    if (this.nomProd != null) {
      data = data.filter(x => x.desProd == this.nomProd)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }
    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: data.length
    };
    this.productos = data.slice(this.paginador.startRow, this.paginador.endRow);;
  }


  filterGuias() {
    let data: any = Object.assign([], this.guiasResp);

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }

    this.paginador.startRow = 0;
    this.paginador.endRow = 10;
    this.paginador.sortBy = 'desc';
    this.config = {
      itemsPerPage: this.paginador.endRow,
      currentPage: 1,
      totalItems: data.length
    };
    this.guias = data.slice(this.paginador.startRow, this.paginador.endRow);;
  }

  onSelect(val: any) {
    if (val.selected.length > 0) {
      let valor: number = 0;
      val.selected.forEach(element => {
        valor += element.debe
      });
      this.total = `$ ${valor}`;
      this.totalPagar = valor;
    } else {
      this.total = "$ 0";
      this.totalPagar = 0;
    }
  }

  downloadPDF() {
    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      if (this.documentoAEnviar.nro != undefined) {
        obj.Folio = this.documentoAEnviar.nro;
        obj.TipoDoc = this.documentoAEnviar.movtipdocref;//fca 08-07-2022 FALTA TIPO DOCUMENTO NO VIEN EN LA API SI ES PRODUCTO
      } else {
        obj.Folio = this.documentoAEnviar.folio;
        obj.TipoDoc = this.documentoAEnviar.tipo == undefined ? this.documentoAEnviar.tipoDoc : this.documentoAEnviar.tipo + this.documentoAEnviar.subTipo;
      }

      obj.CodAux = user.codAux;



      this.spinner.show();

      //Obtengo ruta
      this.clientesService.getClienteDocumento(obj).subscribe(
        (res: any) => {
          debugger
          if (res.base64 != '' && res.base64 != null) {
            var link = document.createElement("a");
            link.download = res.nombreArchivo;
            link.href = this.utils.transformaDocumento64(res.base64, res.tipo);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            link.remove();
          } else {
            this.notificationService.warning('Documento sin archivo PDF, favor contactar con el administrador.', '', true);
          }
          this.spinner.hide();
        },
        err => { this.notificationService.error('Error al obtener Documento', '', true); this.spinner.hide(); }
      );
    } else {
      this.authService.signoutExpiredToken();
    }
  }

  downloadXML() {

    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      if (this.documentoAEnviar.nro != undefined) {
        obj.Folio = this.documentoAEnviar.nro;
        obj.TipoDoc = this.documentoAEnviar.movtipdocref;//fca 08-07-2022 FALTA TIPO DOCUMENTO NO VIEN EN LA API SI ES PRODUCTO
      } else {
        obj.Folio = this.documentoAEnviar.folio;
        obj.TipoDoc = this.documentoAEnviar.tipo == undefined ? this.documentoAEnviar.tipoDoc : this.documentoAEnviar.tipo + this.documentoAEnviar.subTipo;
      }
      obj.CodAux = user.codAux;


      this.spinner.show();

      //Obtengo ruta
      this.clientesService.getClienteXML(obj).subscribe(
        (res: any) => {

          if (res.base64 != null) {
            var link = document.createElement("a");
            link.download = res.nombreArchivo;
            link.href = this.utils.transformaDocumento64(res.base64, res.tipo);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            link.remove();
          } else {
            this.notificationService.warning('Documento sin archivo xml, favor contactar con el administrador.', '', true);
          }

          this.spinner.hide();
        },
        err => { this.notificationService.error('Error al obtener Documento', '', true); this.spinner.hide(); }
      );
    } else {
      this.authService.signoutExpiredToken();
    }
  }

  verDetalle(compra: any) {

    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      obj.Folio = compra.nro;
      if (obj.Folio == null && obj.Folio == undefined) {
        obj.Folio = compra.folio;
        this.productoVenta = compra.codProd;
      }
      obj.CodAux = user.codAux;
      obj.TipoDoc = compra.movtipdocref;


      this.spinner.show();

      this.clientesService.getDetalleCompra(obj).subscribe((res: any) => {
        this.spinner.hide();
        if (res.cabecera != null && res.cabecera != undefined) {
          this.detalleCab = res.cabecera;
          this.detalleDet = res.detalle;
          this.estadoDoc = this.detalleCab.estado == 'V' ? 'VENCIDO' : 'PENDIENTE';
          this.showDetail = true;
          this.notaVenta = compra.nvNumero;
          this.tituloDetalle = compra.documento;
        } else {
          this.notificationService.info('Compra no posee detalle asociado.', '', true);
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }


  verDetalleProducto(producto: any) {

    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      obj.Folio = producto.folio;
      this.productoVenta = producto.codProd;
      obj.CodAux = user.codAux;
      obj.TipoDoc = producto.tipoDoc;


      this.spinner.show();

      this.clientesService.getDetalleCompra(obj).subscribe((res: any) => {
        this.spinner.hide();
        if (res.cabecera != null && res.cabecera != undefined) {
          this.detalleCab = res.cabecera;
          this.detalleDet = res.detalle;
          this.showDetailProducto = true;
          this.notaVenta = producto.nvNumero;
          this.tituloDetalle = producto.documento;
        } else {
          this.notificationService.info('Compra no posee detalle asociado.', '', true);
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }


  verDetalleAutoRef(folio: number, tipo: string) {
    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      obj.Folio = folio;
      obj.CodAux = user.codAux;
      obj.TipoDoc = tipo;


      this.spinner.show();

      this.clientesService.getDetalleCompra(obj).subscribe((res: any) => {
        this.spinner.hide();
        if (res.cabecera != null && res.cabecera != undefined) {
          this.detalleCab = res.cabecera;
          this.detalleDet = res.detalle;
          this.showDetail = true;
          this.modalService.dismissAll();
          // this.notaVenta = compra.nvNumero;
        } else {
          this.notificationService.info('Compra no posee detalle asociado.', '', true);
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }

  verDetalleNv(nv: any) {
    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      obj.Folio = nv.nvNumero;
      obj.CodAux = user.codAux;


      this.spinner.show();

      this.clientesService.getDetalleCompraNv(obj).subscribe((res: any) => {
        this.spinner.hide();
        if (res != null) {
          this.NotaVentaDetalle = res
          this.estadoDoc = this.NotaVentaDetalle.estado == 'A' ? 'APROBADA' : this.NotaVentaDetalle.estado == 'P' ? 'PENDIENTE' : this.NotaVentaDetalle.estado == 'C' ? 'CONCLUIDA' : 'NULA';
          this.showDetailNv = true;
          this.muestraDocPendientes = false;
        } else {
          this.notificationService.info('Compra no posee detalle asociado.', '', true);
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }

  verDetalleGuia(guia: any) {


    var user = this.authService.getuser();
    var obj = { Folio: 0, TipoDoc: '', CodAux: '' };

    if (user != null) {
      obj.Folio = guia.nro;
      obj.CodAux = user.codAux;
      obj.TipoDoc = guia.movtipdocref;


      this.spinner.show();

      this.clientesService.getDetalleCompra(obj).subscribe((res: any) => {
        this.spinner.hide();
        if (res.cabecera != null && res.cabecera != undefined) {

          this.estadoGuia = guia.estado;
          this.detalleCab = res.cabecera;
          this.detalleDet = res.detalle;
          this.showDetailGuia = true;
          this.muestraGuias = false;
          this.notaVenta = guia.nvNumero;
        } else {
          this.notificationService.info('Compra no posee detalle asociado.', '', true);
        }
      }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener detalle de la compra.', '', true); });
    } else {
      this.authService.signoutExpiredToken();
    }
  }

  limpiarFiltros() {
    this.notaVenta = null
    this.dateDesde = null;
    this.dateHasta = null;
    this.selectedEstado = null;
    this.folio = null;
    this.tipoFecha = 1;

    this.filter(this.selectedTabMonedas);
  }


  limpiarFiltrosContabilizados() {
    this.notaVenta = null
    this.dateDesde = null;
    this.dateHasta = null;
    this.selectedEstado = null;
    this.folio = null;
    this.tipoFecha = 1;

    this.filterContabilizados(this.selectedTabMonedas);
  }

  limpiarFiltrosNv() {
    this.notaVenta = null
    this.dateDesde = null;
    this.dateHasta = null;
    this.filterNv();
  }
  limpiarFiltrosPr() {
    this.folio = null;
    this.codigoProd = null;
    this.nomProd = null;
    this.dateDesde = null;
    this.dateHasta = null;
    this.filterPr();
  }

  limpiarFiltrosGuias() {
    this.folio = null;
    this.dateDesde = null;
    this.dateHasta = null;
    this.filterGuias();
  }

  exportCompras(moneda: number) {
    debugger
    let data: any = Object.assign([], this.comprasResp);

    if (moneda == 1) {
      data = data.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada);
    } else if (moneda == 2) {
      data = data.filter(x => x.codMon != this.configuracionPagos.monedaUtilizada);
    }

    if (this.notaVenta != null) {
      data = data.filter(x => x.nro == this.notaVenta)
    }
    if (this.selectedEstado != null) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.tipoFecha == 1) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.femision) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.femision) <= fHasta)
      }
    }

    if (this.tipoFecha == 2) {
      if (this.dateDesde != null) {
        const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
        data = data.filter(x => new Date(x.fvencimiento) >= fDesde)
      }
      if (this.dateHasta != null) {
        const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
        data = data.filter(x => new Date(x.fvencimiento) <= fHasta)
      }
    }


    var listaExportacion = [];
    data.forEach((element, index) => {
      var fvenc = formatDate(element.fvencimiento, 'yyyy-MM-dd', 'en-US');
      var femi = formatDate(element.femision, 'yyyy-MM-dd', 'en-US');
      const model = { Tipo: element.documento, Folio: element.nro, NotaVenta: element.notaVenta, Emision: femi, Vencimiento: fvenc, Monto: element.monto };
      listaExportacion.push(model);
    });

    this.exportAsExcelFileCompras(listaExportacion, "Documentos Facturados");
  }


  exportComprasContabilizadas(moneda: number) {
    let data: any = Object.assign([], this.comprasResp);

    if (moneda == 1) {
      data = data.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada);
    } else if (moneda == 2) {
      data = data.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada);
    }

    if (this.notaVenta != null) {
      data = data.filter(x => x.nro == this.notaVenta)
    }
    if (this.selectedEstado != null) {
      data = data.filter(x => x.estado == this.selectedEstado.nombre)
    }
    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateHasta.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.femision) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.femision) <= fHasta)
    }

    var listaExportacion = [];
    data.forEach((element, index) => {
      var fvenc = formatDate(element.fvencimiento, 'yyyy-MM-dd', 'en-US');
      var femi = formatDate(element.femision, 'yyyy-MM-dd', 'en-US');
      const model = { Tipo: element.documento, Folio: element.nro, NotaVenta: element.notaVenta, Emision: femi, Vencimiento: fvenc, Monto: element.monto };
      listaExportacion.push(model);
    });

    this.exportAsExcelFileCompras(listaExportacion, "Compras");
  }


  exportNV() {
    let data: any = Object.assign([], this.notasVentaResp);

    if (this.notaVenta != null) {
      data = data.filter(x => x.nvNumero == this.notaVenta)
    }
    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }

    var listaExportacion = [];
    data.forEach((element, index) => {
      var fvenc = formatDate(element.fecha, 'yyyy-MM-dd', 'en-US');
      var femi = formatDate(element.fechaEntrega, 'yyyy-MM-dd', 'en-US');
      const model = { Folio: element.nvNumero, Fecha: fvenc, Fecha_Entrega: femi, Monto: element.monto, Facturado: element.montoFacturado, Pendiente_Facturar: element.montoPendiente };
      listaExportacion.push(model);
    });

    this.exportAsExcelFileNv(listaExportacion, "Notas de venta pendientes");
  }

  exportProductos() {
    let data: any = Object.assign([], this.productosResp);

    if (this.folio != null) {
      data = data.filter(x => x.folio == this.folio)
    }
    if (this.codigoProd != null) {
      data = data.filter(x => x.codProd == this.codigoProd)
    }
    if (this.nomProd != null) {
      data = data.filter(x => x.desProd == this.nomProd)
    }

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }
    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }
    var listaExportacion = [];
    data.forEach((element, index) => {
      var fecha = formatDate(element.fecha, 'yyyy-MM-dd', 'en-US');
      const model = { Fecha: fecha, Codigo: element.codProd, Nombre: element.desProd, Cantidad: element.cantidad, Precio: element.precio, Total: element.totalLinea };
      listaExportacion.push(model);
    });

    this.exportAsExcelFileNv(listaExportacion, "Productos");
  }


  exportGuias() {
    let data: any = Object.assign([], this.guiasResp);

    if (this.folio != null) {
      data = data.filter(x => x.nro == this.folio)
    }

    if (this.dateDesde != null) {
      const fDesde = new Date(this.dateDesde.year, this.dateDesde.month - 1, this.dateDesde.day, 0, 0, 0);
      data = data.filter(x => new Date(x.fecha) >= fDesde)
    }

    if (this.dateHasta != null) {
      const fHasta = new Date(this.dateHasta.year, this.dateHasta.month - 1, this.dateHasta.day, 23, 59, 59);
      data = data.filter(x => new Date(x.fecha) <= fHasta)
    }

    var listaExportacion = [];
    data.forEach((element, index) => {
      var fecha = formatDate(element.fecha, 'yyyy-MM-dd', 'en-US');
      const model = { Folio: element.nro, Fecha: fecha, Monto: element.total, Estado: element.estado };
      listaExportacion.push(model);
    });

    this.exportAsExcelFileGuias(listaExportacion, "Guías de Despacho");
  }

  public exportAsExcelFileGuias(rows: any[], excelFileName: string): void {
    if (rows.length > 0) {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
      const workbook: XLSX.WorkBook = { Sheets: { 'Guías de Despacho': worksheet }, SheetNames: ['Guías de Despacho'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    } else {
      this.notificationService.warning('error al exportar', '', true);
    }
  }

  public exportAsExcelFileCompras(rows: any[], excelFileName: string): void {
    if (rows.length > 0) {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
      const workbook: XLSX.WorkBook = { Sheets: { 'Documentos Facturados': worksheet }, SheetNames: ['Documentos Facturados'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    } else {
      this.notificationService.warning('error al exportar', '', true);
    }
  }

  public exportAsExcelFileNv(rows: any[], excelFileName: string): void {
    if (rows.length > 0) {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
      const workbook: XLSX.WorkBook = { Sheets: { 'Notas de venta pendientes': worksheet }, SheetNames: ['Notas de venta pendientes'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    } else {
      this.notificationService.warning('error al exportar', '', true);
    }
  }

  public exportAsExcelFileProductos(rows: any[], excelFileName: string): void {
    if (rows.length > 0) {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
      const workbook: XLSX.WorkBook = { Sheets: { 'Productos': worksheet }, SheetNames: ['Productos'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    } else {
      this.notificationService.warning('error al exportar', '', true);
    }
  }

  private saveAsExcelFile(buffer: any, baseFileName: string): void {
    const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
    FileSaver.saveAs(data, baseFileName + EXCEL_EXTENSION);
  }

  muestraFinalizadas() {

    if (this.cantidadCompras > 0) {
      this.spinner.show();
      const user = this.authService.getuser();
      if (user) {

        const model = { codAux: user.codAux, rut: user.rut, correo: user.email };
        this.clientesService.getClienteComprasFromSoftland(model).subscribe((res: any) => {

          this.comprasResp = res;
          this.paginador.startRow = 0;
          this.paginador.endRow = 10;
          this.paginador.sortBy = 'desc';
          this.paginador.search = '';
          this.config = {
            itemsPerPage: this.paginador.endRow,
            currentPage: 1,
            totalItems: this.comprasResp.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada).length
          };
          this.compras = res.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
          this.estados = [];
          this.tiposDocs = [];

          let compraSsegundaMoneda = this.comprasResp.filter(x => x.codMon != this.configuracionPagos.monedaUtilizada);

          if (compraSsegundaMoneda.length > 0) {
            this.existComprasSegundaMoneda = true;
          }

          res.forEach((element, index) => {
            const ex1 = this.estados.find(x => x.nombre == element.estado);
            const ex2 = this.tiposDocs.find(x => x.nombre == element.documento);

            if (ex1 == null) {
              this.estados.push({
                id: index + 1,
                nombre: element.estado
              });
            }
            if (ex2 == null) {
              this.tiposDocs.push({
                id: index + 1,
                nombre: element.documento
              });
            }
          });

          this.muestraDocFinalizados = true;
          this.muestraDocPendientes = false;
          this.muestraCabecera = false;
          this.esFactuarada = true;
          this.tipoDocEnvio = 1;
          this.tipoDescarga = 1;
          // this.cambiaColorPaginador(this.configDiseno);
          this.spinner.hide();
        }, err => { this.spinner.hide(); });

      } else {
        this.spinner.hide();
        this.authService.signoutExpiredToken();
      }
    } else {
      this.notificationService.warning('No existen resultados', '', true);
      return;
    }
  }

  muestraPendientes() {


    if (this.cantidadNV > 0) {
      this.spinner.show();
      const user = this.authService.getuser();
      if (user) {

        this.clientesService.getNotasVentaCliente(user.codAux).subscribe((res: any) => {
          this.notasVentaResp = res;
          this.muestraDocPendientes = true;
          this.muestraDocFinalizados = false;
          this.muestraCabecera = false;
          this.paginador.startRow = 0;
          this.paginador.endRow = 10;
          this.paginador.sortBy = 'desc';
          this.paginador.search = '';
          this.config = {
            itemsPerPage: this.paginador.endRow,
            currentPage: 1,
            totalItems: this.notasVentaResp.length
          };
          this.notasVenta = res.slice(this.paginador.startRow, this.paginador.endRow);
          this.esFactuarada = false;
          this.tipoDocEnvio = 2;
          this.tipoDescarga = 2;
          // this.cambiaColorPaginador(this.configDiseno);
          this.spinner.hide();
        }, err => { this.spinner.hide(); });
      } else {
        this.spinner.hide();
        this.authService.signoutExpiredToken();
      }
    } else {
      this.notificationService.warning('No existen resultados', '', true);
      return;
    }

  }

  muestraProductos() {
    if (this.CantidadProductos > 0) {
      this.spinner.show();
      const user = this.authService.getuser();
      if (user) {

        const model = { codAux: user.codAux };
        this.clientesService.getProductosCliente(model).subscribe((res: any) => {

          this.productosResp = res;
          this.paginador.startRow = 0;
          this.paginador.endRow = 10;
          this.paginador.sortBy = 'desc';
          this.paginador.search = '';
          this.config = {
            itemsPerPage: this.paginador.endRow,
            currentPage: 1,
            totalItems: this.productosResp.length
          };
          this.productos = res.slice(this.paginador.startRow, this.paginador.endRow);;
          this.muestraProductosCompras = true;
          this.muestraCabecera = false;
          this.esFactuarada = true;
          this.tipoDocEnvio = 3;
          this.tipoDescarga = 1;
          // this.cambiaColorPaginador(this.configDiseno);
          this.spinner.hide();
        }, err => { this.spinner.hide(); });
      } else {
        this.spinner.hide();
        this.authService.signoutExpiredToken();
      }
    } else {
     
      this.notificationService.warning('No existen resultados', '', true);
      return;
    }

  }

  volver(tipo: number) {
    switch (tipo) {
      case 1:
        this.detalleDet = null;
        this.showDetail = false;
        this.notaVenta = null;
        this.muestraDocFinalizados = true;
        this.detalleCab = null;
        break;

      case 2:
        this.compras = [];
        this.comprasResp = [];
        this.estados = [];
        this.tiposDocs = [];
        this.muestraCabecera = true;
        this.muestraDocFinalizados = false;
        this.tipoFecha = 0;
        this.limpiarFiltros();
        break;
      case 3:
        this.NotaVentaDetalle = null;
        this.muestraDocPendientes = true;
        this.showDetailNv = false;
        break
      case 4:
        this.notasVenta = [];
        this.muestraDocPendientes = false;
        this.muestraCabecera = true;
        this.limpiarFiltrosNv();
        break
      case 5:
        this.limpiarFiltrosPr();
        this.productos = [];
        this.muestraProductosCompras = false;
        this.muestraCabecera = true;
        break

      case 6:
        this.detalleCab = null;
        this.detalleDet = null;
        this.showDetailProducto = false;
        this.notaVenta = null;
        this.muestraDocFinalizados = false;
        break
      case 7:
        this.guias = [];
        this.muestraGuias = false;
        this.muestraCabecera = true;
        this.limpiarFiltrosGuias();
        break
      case 8:
        this.detalleDet = null;
        this.detalleDet = null;
        this.muestraGuias = true;
        this.showDetailGuia = false;
        break
    }


    // this.muestraDocPendientes = false;
    // this.muestraDocFinalizados = false;
    // this.muestraCabecera = true;
    // this.showDetail = false;
  }

  verDespacho(compra: any, content) {
    this.spinner.show();
    const user = this.authService.getuser();
    const model = { Folio: compra.nro, TipoDoc: compra.movtipdocref, CodAux: user.codAux };

    this.clientesService.getDespachoDocumeto(model).subscribe(res => {
      this.despachos = res;
      if (this.despachos.length > 0) {
        this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
      } else {
        this.notificationService.warning('No existen despachos asociados', '', true);
      }
      this.spinner.hide();

    }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener despachos', '', true); });

  }

  verDetalleDespacho(desp: any, content) {

    if (desp.tipo == 'B' || desp.tipo == 'F') {
      this.verDetalleAutoRef(desp.folio, desp.tipo);
    } else {

      this.detalleDespacho = desp.detalle;
      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
    }

  }

  modalPDF(compra: any, content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  print(tipo: number) { //FCA 08-07-2022
    if (window) {
      window.print();
      // this.spinner.show();
      // switch (tipo) {
      //   case 1:
      //     this.clientesService.getdocumentoPDF(this.detalleCab).subscribe(res => {
      //       let a = document.createElement("a");
      //       a.href = "data:application/octet-stream;base64," + res;
      //       a.download = this.detalleCab.folio + '.pdf';
      //       a.click();
      //       this.spinner.hide();
      //     }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
      //     break;

      //   case 2:
      //     this.clientesService.getdocumentoPDFNv(this.NotaVentaDetalle).subscribe(res => {
      //       let a = document.createElement("a");
      //       a.href = "data:application/octet-stream;base64," + res;
      //       a.download = "Nota de Venta #" + this.NotaVentaDetalle.nvNumero + ".pdf";
      //       a.click();
      //       this.spinner.hide();
      //     }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
      //     break;
      // }

    }
  }

  openModalSend(content, tipo: string, doc: any) {
    this.spinner.show();
    this.contactosCliente = [];
    this.enviaPdf = false;
    this.enviaXml = false;
    this.documentoAEnviar = doc;
    if (this.configuracion.muestraContactosEnvio == 1) {
      this.tituloEnvio = "Seleccione los contactos o ingrese los correos a los que desea enviar los documentos."
      var user = this.authService.getuser();
      if (user != null) {
        this.clientesService.getContactosClienteSoftland(user.codAux).subscribe(res => {
          this.contactosCliente = res;
          this.spinner.hide();
          this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener contactos del cliente', '', true); });
      } else {
        this.authService.signoutExpiredToken();
      }
    } else {
      this.tituloEnvio = "Ingrese los correos a los que desea enviar los documentos."
      this.spinner.hide();
      this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
    }


  }

  onChange(item: any) {
    const added = this.contactosSeleccionados.find(x => x.correo == item.correo);
    if (added == null) {
      this.contactosSeleccionados.push(item)
    } else {
      for (let i: number = 0; i <= this.contactosSeleccionados.length - 1; i++) {
        if (this.contactosSeleccionados[i].correo == item.correo) {
          this.contactosSeleccionados.splice(i, 1);
          break;
        }
      }
    }
  }

  descargarDocumentos(tipo: number, doc: any) {

    this.documentoAEnviar = doc;
    switch (tipo) {
      case 1:

        if (!this.enviaPdf && !this.enviaXml && (this.showDetailProducto || this.showDetail || this.showDetailGuia)) {
          this.notificationService.warning('Debe seleccionar documentos a descargar.', '', true);
          return;
        }

        if (this.enviaPdf && (this.showDetailProducto || this.showDetail || this.showDetailGuia)) {
          this.downloadPDF();
        }

        if (this.enviaXml && (this.showDetailProducto || this.showDetail || this.showDetailGuia)) {
          this.downloadXML();
        }

        if (!this.showDetailProducto && !this.showDetail && !this.showDetailGuia) {
          this.downloadPDF();
          this.downloadXML();
        }
        break;

      case 2:
        if (!this.enviaPdf && this.showDetailNv) {
          this.notificationService.warning('Debe seleccionar documentos a descargar.', '', true);
          return;
        }
        this.spinner.show();
        let nv = this.NotaVentaDetalle;
        if (nv == null) {
          nv = doc;
          const user = this.authService.getuser();

          if (user != null) {
            nv.codAux = user.codAux;

            this.clientesService.getdocumentoPDFNv(nv).subscribe((res: any) => {
              debugger
              if (res.pdfBase64 != '' && res.pdfBase64 != null) {
                var link = document.createElement("a");
                link.download = "Nota de Venta " + nv.nvNumero + ".pdf";
                link.href = this.utils.transformaDocumento64(res.pdfBase64, ".pdf");
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                link.remove();
              } else {
                this.notificationService.warning('No existen archivos asociados al documento', '', true);
              }

              this.spinner.hide();
            }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al obtener documento', '', true); });
          } else {
            this.authService.signoutExpiredToken();
          }
        }

        break;
      case 3:
        if (!this.enviaPdf && this.showDetailGuia) {
          this.notificationService.warning('Debe seleccionar documentos a descargar.', '', true);
          return;
        }
        this.downloadPDF();
        break;
    }

  }

  public onAdd(tag: AutoCompleteModel, type: number) {
    this.removeFromArrayIfMailIsInvalid(tag.value, this.otrosCorreo);
    this.toLowerMails(this.otrosCorreo);
  }

  toLowerMails(arrayCorreos: AutoCompleteModel[]) {
    for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
      arrayCorreos[i].value = arrayCorreos[i].value.toLowerCase();
      arrayCorreos[i].display = arrayCorreos[i].display.toLowerCase();
    }
  }

  removeFromArrayIfMailIsInvalid(mail: string, arrayCorreos: AutoCompleteModel[]) {
    if (!this.utils.validateMail(mail)) {
      this.notificationService.warning('Debe ingresar un correo válido.', '', true);
      for (let i: number = 0; i <= arrayCorreos.length - 1; i++) {
        if (arrayCorreos[i].value == mail) {
          arrayCorreos.splice(i, 1);
          break;
        }
      }
      return;
    }
  }

  send(tipo: number) {

    if (this.contactosSeleccionados.length == 0 && this.otrosCorreo.length == 0) {
      if (this.configuracion.muestraContactosEnvio == 1) {
        this.notificationService.warning('Debe seleccionar o ingresar un correo.', '', true);
      } else {
        this.notificationService.warning('Debe ingresar un correo.', '', true);
      }

      return;
    }

    if (!this.enviaPdf && !this.enviaXml) {
      this.notificationService.warning('Debe seleccionar almenos un tipo de documento.', '', true);
      return;
    }

    this.spinner.show();

    let correos: string = '';
    this.contactosSeleccionados.forEach(element => {
      if (correos == '') {
        correos = correos + element.correo;
      } else {
        correos = correos + ";" + element.correo;
      }

    });

    this.otrosCorreo.forEach(element => {
      if (correos == '') {
        correos += `${element.value}`;
      } else {
        correos += `;${element.value}`;
      }

    });

    const user = this.authService.getuser();

    switch (tipo) {
      case 1:
        if (!this.enviaPdf && !this.enviaXml) {
          this.notificationService.warning('Debe seleccionar un documento para enviar.', '', true);
          return;
        }

        var tipoEnvio = (this.enviaPdf && this.enviaXml) ? 3 : (this.enviaPdf) ? 1 : (this.enviaXml) ? 2 : 3;

        var envioDocumento = {
          destinatarios: correos,
          folio: this.documentoAEnviar.folio,
          codAux: user.codAux,
          tipoDoc: this.documentoAEnviar.movtipdocref == undefined ? this.documentoAEnviar.tipo : this.documentoAEnviar.movtipdocref,
          subTipoDoc: this.documentoAEnviar.subTipo,
          tipoDocAEnviar: 1,
          docsAEnviar: tipoEnvio
        }


        this.clientesService.enviaDocumentoPDF(envioDocumento).subscribe((res: any) => {
          this.spinner.hide();
          this.otrosCorreo = [];
          this.contactosSeleccionados = [];
          this.enviaPdf = false;
          this.enviaXml = false;
          this.modalService.dismissAll();
          if (res == 0) {
            this.notificationService.warning('No existen archivos asociados al documento.', '', true);
          } else {
            this.notificationService.success('Documentos enviados correctamente.', '', true);
          }

        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });
        break;

      case 2:
        var tipoEnvio = (this.enviaPdf && this.enviaXml) ? 3 : (this.enviaPdf) ? 1 : (this.enviaXml) ? 2 : 3;
        var envioDocumento = {
          destinatarios: correos,
          folio: this.documentoAEnviar.nvNumero,
          codAux: user.codAux,
          tipoDoc: null,
          subTipoDoc: null,
          tipoDocAEnviar: 2,
          docsAEnviar: tipoEnvio
        }
        this.clientesService.enviaDocumentoPDF(envioDocumento).subscribe(res => {
          this.spinner.hide();
          this.otrosCorreo = [];
          this.contactosSeleccionados = [];
          this.enviaPdf = false;
          this.enviaXml = false;
          this.modalService.dismissAll();
          if (res == 0) {
            this.notificationService.warning('No existen archivos asociados al documento.', '', true);
          } else {
            this.notificationService.success('Documentos enviados correctamente.', '', true);
          }
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });
        break;

      case 3: //PRODUCTO SE DEBE REVISAR FOLIO, TIPO Y SUB TIPO
        var tipoEnvio = (this.enviaPdf && this.enviaXml) ? 3 : (this.enviaPdf) ? 1 : (this.enviaXml) ? 2 : 3;
        var envioDocumento = {
          destinatarios: correos,
          folio: this.documentoAEnviar.folio,
          codAux: user.codAux,
          tipoDoc: this.documentoAEnviar.movtipdocref == undefined ? this.documentoAEnviar.tipo : this.documentoAEnviar.movtipdocref,
          subTipoDoc: this.documentoAEnviar.subTipo,
          tipoDocAEnviar: 1,
          docsAEnviar: tipoEnvio
        }
        this.clientesService.enviaDocumentoPDF(envioDocumento).subscribe(res => {
          this.spinner.hide();
          this.otrosCorreo = [];
          this.contactosSeleccionados = [];
          this.enviaPdf = false;
          this.enviaXml = false;
          this.modalService.dismissAll();
          if (res == 0) {
            this.notificationService.warning('No existen archivos asociados al documento.', '', true);
          } else {
            this.notificationService.success('Documentos enviados correctamente.', '', true);
          }
        }, err => { this.spinner.hide(); this.notificationService.error('Ocurrió un error al enviar correo.', '', true); });
        break;
    }

  }


  changePageFacturas(event: any, tipo: number) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;

    switch (tipo) {
      case 1:
        this.compras = this.comprasResp.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
        break;

      case 2:
        this.compras = this.comprasResp.filter(x => x.codMon != this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
        break;
    }

  }

  changePageContabilizados(event: any, tipo: number) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;

    switch (tipo) {
      case 1:
        this.compras = this.comprasResp.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
        break;

      case 2:
        this.compras = this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada).slice(this.paginador.startRow, this.paginador.endRow);
        break;
    }

  }

  changePageNV(event: any) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    this.notasVenta = this.notasVentaResp.slice(this.paginador.startRow, this.paginador.endRow);
  }

  changePageProducto(event: any) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    this.productos = this.productosResp.slice(this.paginador.startRow, this.paginador.endRow);
  }

  changePageGuias(event: any) {
    this.paginador.startRow = ((event - 1) * 10);
    this.paginador.endRow = (event * 10);
    this.paginador.sortBy = 'desc';
    this.config.currentPage = event;
    this.guias = this.guiasResp.slice(this.paginador.startRow, this.paginador.endRow);
  }

  // cambiaColorPaginador(configDise: ConfiguracionDiseno) {
  //   setTimeout(function (configD: any = configDise) {
  //     debugger
  //     var element = document.getElementsByClassName('current ng-star-inserted');
  //     element[0].setAttribute('style', 'background-color: ' + configD.colorPaginador + '; !important');

  //   }, 1000);

  // }

  muestraGuia() {
    if (this.cantidadGuias > 0) {  
      this.spinner.show();

      const user = this.authService.getuser();
      if (user) {

        const model = { codAux: user.codAux, rut: user.rut, correo: user.email };
        this.clientesService.getClienteGuiasDespacho(model).subscribe((res: any) => {

          this.guiasResp = res;
          this.paginador.startRow = 0;
          this.paginador.endRow = 10;
          this.paginador.sortBy = 'desc';
          this.paginador.search = '';
          this.config = {
            itemsPerPage: this.paginador.endRow,
            currentPage: 1,
            totalItems: this.comprasResp.length
          };
          this.guias = res.slice(this.paginador.startRow, this.paginador.endRow);
          this.estados = [];

          res.forEach((element, index) => {
            const ex1 = this.estados.find(x => x.nombre == element.estado);

            if (ex1 == null) {
              this.estados.push({
                id: index + 1,
                nombre: element.estado
              });
            }
          });

          this.muestraGuias = true;
          this.muestraDocPendientes = false;
          this.muestraCabecera = false;
          this.esFactuarada = false;
          this.tipoDocEnvio = 3;
          this.tipoDescarga = 3;
          this.spinner.hide();
        }, err => { this.spinner.hide(); });

      } else {
        this.spinner.hide();
        this.authService.signoutExpiredToken();
      }
    } else {
      this.notificationService.warning('No existen resultados', '', true);
      return;
    }
  }

  changeTabFacturas(event: any) {
    this.selectedTabMonedas = event.nextId;
    switch (event.nextId) {
      case 1:
        this.paginador.startRow = 0;
        this.paginador.endRow = 10;
        this.paginador.sortBy = 'desc';
        this.paginador.search = '';
        this.config = {
          itemsPerPage: this.paginador.endRow,
          currentPage: 1,
          totalItems: this.comprasResp.filter(x => x.codMon == this.configuracionPagos.monedaUtilizada).length
        };
        this.limpiarFiltros();
        break;

      case 2:
        this.paginador.startRow = 0;
        this.paginador.endRow = 10;
        this.paginador.sortBy = 'desc';
        this.paginador.search = '';
        this.config = {
          itemsPerPage: this.paginador.endRow,
          currentPage: 1,
          totalItems: this.comprasResp.filter(x => x.codMon != this.configuracionPagos.monedaUtilizada).length
        };
        this.limpiarFiltros();
        break;
    }
  }


  changeTabContabilizados(event: any) {
    this.selectedTabMonedas = event.nextId;
    switch (event.nextId) {
      case 1:
        this.paginador.startRow = 0;
        this.paginador.endRow = 10;
        this.paginador.sortBy = 'desc';
        this.paginador.search = '';
        this.config = {
          itemsPerPage: this.paginador.endRow,
          currentPage: 1,
          totalItems: this.comprasResp.filter(x => x.codigoMoneda == this.configuracionPagos.monedaUtilizada).length
        };
        this.limpiarFiltrosContabilizados();
        break;

      case 2:
        this.paginador.startRow = 0;
        this.paginador.endRow = 10;
        this.paginador.sortBy = 'desc';
        this.paginador.search = '';
        this.config = {
          itemsPerPage: this.paginador.endRow,
          currentPage: 1,
          totalItems: this.comprasResp.filter(x => x.codigoMoneda != this.configuracionPagos.monedaUtilizada).length
        };
        this.limpiarFiltrosContabilizados();
        break;
    }
  }



}
