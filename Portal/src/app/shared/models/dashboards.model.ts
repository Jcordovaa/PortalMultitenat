import { TipoDashboards } from '../enums/TipoDashboards';
import { Producto } from './productos.model';
import { Venta } from './ventas.model';

export class DashboardEcommerce {
    tipoDashboard?: TipoDashboards;
    fechaDesde?: Date;
    fechaHasta?: Date;
    nuevosClientes?: number;
    ventasNetas?: number;
    notasDeVenta?: number;
    ventasWeb?: number;
    productosMasBuscados?: Producto[];
    productosMasVendidos?: Producto[];
    ventas?: Venta[];

    constructor() {
        this.tipoDashboard = TipoDashboards.Ecommerce,
        this.fechaDesde = new Date(),
        this.fechaHasta = new Date(),
        this.nuevosClientes = 0,
        this.ventasNetas = 0,
        this.notasDeVenta = 0,
        this.ventasWeb = 0
        this.productosMasBuscados = []
        this.productosMasVendidos = []
        this.ventas = []
    }
}

export class DashboardVentas {
    
}