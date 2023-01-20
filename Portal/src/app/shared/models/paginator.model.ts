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
    }
  
  }