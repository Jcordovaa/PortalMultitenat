export class TiposDespacho {
    idTipoDespacho?: number;
    tipo?: number;
    nombre?: string;
    descripcion?: string;
    precio?: number;
    idRegion?: number;
    idCiudad?: number;
    idComuna?: number;
    compraMinima?: number;
    codProductoSoftland?: string;
    estado?: number;
    tipoDesp?:  TipoDesp;
    totalFilas?: number;

    constructor() {
        this.idTipoDespacho = 0;
        this.tipo = 0;
        this.nombre = '';
        this.descripcion = '';
        this.precio = null;
        this.idRegion = 0;
        this.idCiudad = 0;
        this.idComuna = 0;
        this.compraMinima = null;
        this.codProductoSoftland = '';
        this.estado = 1;
        this.tipoDesp = new TipoDesp();
        this.totalFilas = 0;
    }
}

export class TipoDesp {
    idTipoDes?: number;
    nombre?: string;
    descripcion?: string;
    estado?: number;

    constructor() {
        this.idTipoDes = 0;
        this.nombre = '';
        this.descripcion = '';
        this.estado = 0;
    }
}