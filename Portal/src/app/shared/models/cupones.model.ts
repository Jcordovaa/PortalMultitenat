export class Cupon {
    idCupon?: number;
    nombre?: string;
    descripcion?: string;
    idTipoDescuento?: number;
    descuento?: number;
    codProducto?: string;
    codigoCupon?: string;
    estado?: number;
    tipoDescuento?: TipoDescuento;
    totalFilas?: number;

    constructor() {
        this.idCupon = 0;
        this.nombre = '';
        this.descripcion = '';
        this.idTipoDescuento = 0;
        this.descuento = 0;
        this.codProducto = '';
        this.codigoCupon = '';
        this.estado = 1;
        this.tipoDescuento = new TipoDescuento();
        this.totalFilas = 0;
    }
}

export class TipoDescuento {
    idTipoDescuento?: number;
    nombre?: string;
    estado?: number;

    constructor() {
        this.idTipoDescuento = 0;
        this.nombre = '';
        this.estado = 0;
    }
}