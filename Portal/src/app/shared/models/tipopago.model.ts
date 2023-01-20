export class TipoPago {
    idTipoPago?: number;
    nombre?: string;
    estado?: number;
    tipoDocumento?: string;
    cuentaContable?: string;
    muestraMonto?: number;
    muestraBanco?: number;
    muestraSerie?: number;
    muestraFecha?: number;
    muestraComprobante?: number;
    muestraCantidad?: number;
    generaDTE?: number;
    fechaMod?: Date;
    idUsuarioMod?: number;

    constructor() {
        this.idTipoPago = 0;
        this.nombre = '';
        this.estado = 0;
        this.tipoDocumento = '';
        this.cuentaContable = '';
        this.muestraMonto = 0;
        this.muestraBanco = 0;
        this.muestraSerie = 0;
        this.muestraFecha = 0;
        this.muestraComprobante = 0;
        this.muestraCantidad = 0;
        this.generaDTE = 0;
        this.fechaMod = new Date();
        this.idUsuarioMod = 0;
    }
}