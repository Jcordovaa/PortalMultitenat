import { Cliente } from './clientes.model';

export class Venta {
    idVenta?: number;
    fechaVenta?: Date;
    rutCliente?: string;
    emailCliente?: string;
    idCupon?: number;
    idVendedor?: number;
    idEstadoVenta?: number;
    idCondicionVenta?: number;
    idTipoPago?: number;
    idTipoDocumento?: number;
    subTotal?: number;
    descuento?: number;
    porcDescuento?: number;
    despacho?: number;
    iva?: number;
    total?: number;
    nvNumero?: number;
    codAux?: string;
    observaciones?: string;
    estadoVenta?: EstadoVenta;
    cliente?: Cliente;
    totalFilas?: number;
    facturada?: number;
    despachada?: number;
    ventaDetalle?: VentaDetalle[];

    constructor() {
        this.idVenta = 0;
        this.fechaVenta = new Date();
        this.rutCliente = '';
        this.emailCliente = '';
        this.idCupon = 0;
        this.idVendedor = 0;
        this.idEstadoVenta = 0;
        this.idCondicionVenta
        this.idTipoPago = 0;
        this.idTipoDocumento = 0;
        this.subTotal = 0;
        this.descuento = 0;
        this.porcDescuento = 0;
        this.despacho = 0;
        this.iva = 0;
        this.total = 0;
        this.nvNumero = 0;
        this.codAux = '';
        this.observaciones = '';
        this.estadoVenta = new EstadoVenta();
        this.cliente = new Cliente();
        this.totalFilas = 0;
        this.facturada = 0;
        this.despachada = 0;
        this.ventaDetalle = [];
    }
}

export class VentaDetalle {
    idVentaDetalle?: number;
    idVenta?: number;
    codProducto?: string;
    correlativo?: number;
    cantidad?: number;
    precio?: number;
    subTotal?: number;
    descuento?: number;
    total?: number;
    fechaVenta?: Date;

    constructor() {
        this.idVentaDetalle = 0;
        this.idVenta = 0;
        this.codProducto = '';
        this.correlativo = 0;
        this.cantidad = 0;
        this.precio = 0;
        this.subTotal = 0;
        this.descuento = 0;
        this.total = 0;
        this.fechaVenta = new Date();
    }
}

export class EstadoVenta {
    idEstadoVenta?: number;
    nombre?: string;
    estado?: number;

    constructor() {
        this.idEstadoVenta = 0;
        this.nombre = '';
        this.estado = 0;
    }
}