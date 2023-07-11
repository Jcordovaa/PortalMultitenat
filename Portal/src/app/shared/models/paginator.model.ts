import { SortEntidad } from '../enums/SortEntidad';

export class Paginator {
    startRow: number;
    endRow?: number;
    sortBy? : string;
    search? : string;
    condicionVenta?: string;
    categoriaCliente?: string;
    listaPrecio?: string;
    tiposDocumentos?: string;
    vendedor?: string;
    tipo?: number;
    documentos?: any;
    folio?: number;
    estado?: number;
    fechaDesde?: Date;
    fechaHasta?: Date;

    constructor() {
      this.startRow = 0;
      this.endRow = 10;
      this.sortBy = 'desc';
      this.search = '';
      this.condicionVenta = '';
      this.categoriaCliente = '';
      this.listaPrecio = '';
      this.tiposDocumentos = '';
      this.vendedor = '';
      this.tipo = 0;
      this.documentos = null;
      this.folio = 0;
      this.fechaDesde = null;
      this.fechaHasta = null;
      this.estado = 0;
    }
  
  }