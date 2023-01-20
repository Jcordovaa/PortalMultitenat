export class Pagos {
    idTipoPago?: number;
    nombre?: string;
    tipoDocumento?: string;
    cuentaContable?: string;
    montoPago?: number;
    codBanco?: string;
    banco?: string;
    cantidad?: number;
    serie?: string;
    fecha?: string;
    fechaFom?: Date;
    comprobante?: string;
    idBanco?: number;
    folio?: number;

    constructor() {
        this.idTipoPago = 0;
        this.nombre = '';
        this.tipoDocumento = '';
        this.cuentaContable = '';
        this.montoPago = 0;
        this.codBanco = '';
        this.banco = '';
        this.cantidad = 0;
        this.serie = '';
        this.fecha = '';
        this.fechaFom = new Date();
        this.comprobante = '';
        this.idBanco = 0;
        this.folio = 0;
    }
}

export class PagosCabecera {
    idPago?: number;
    rutCliente?: string;
    fechaPago?: Date;
    horaPago?: string;
    montoPago?: number;
    comprobanteContable?: string;
    fechaEnvioCobranza?: Date;
    horaEnvioCobranza?: string;
    idEstadoEnvioCobranza?: number;

    constructor() {
        this.idPago = 0;
        this.rutCliente = '';
        this.fechaPago = new Date();
        this.horaPago = '';
        this.montoPago = 0;
        this.comprobanteContable = '';
        this.fechaEnvioCobranza = new Date();
        this.horaEnvioCobranza = '';
        this.idEstadoEnvioCobranza = 0;
    }
}

export class PagosDetalle {
    idPagoDetalle?: number;
    idPago?: number;
    idTipoPago?: number;
    montoPago?: number;
    idBanco?: number;
    serie?: string;
    fechaEmisionDocumento?: Date;
    fechaVencimientoDocumento?: Date;
    cantidadDocumentos?: number;
    nroComprobante?: string;
    cuentaContable?: string;
    tipoDoc?: string;
    fechaTransaccion?: Date;
    horaTransaccion?: string;
    idUsuario?: number;

    constructor() {
        this.idPagoDetalle = 0;
        this.idPago = 0;
        this.idTipoPago = 0;
        this.montoPago = 0;
        this.idBanco = 0;
        this.serie = '';
        this.fechaEmisionDocumento = new Date();
        this.fechaVencimientoDocumento = new Date();
        this.cantidadDocumentos = 0;
        this.nroComprobante = '';
        this.cuentaContable = '';
        this.tipoDoc = '';
        this.fechaTransaccion = new Date();
        this.horaTransaccion = '';
        this.idUsuario = 0;
    }
}

export class PagosDocumentos {
    idPagoDocumento?: number;
    idPago?: number;
    idTipoDocumento?: number;
    folioDocumento?: string;
    tipoDocumento?: string;
    estadoPago?: number;
    fechaEnvioCobranza?: Date;
    horaEnvioCobranza?: string;
    correosEnvio?: string;

    constructor() {
        this.idPagoDocumento = 0;
        this.idPago = 0;
        this.idTipoDocumento = 0;
        this.folioDocumento = '';
        this.tipoDocumento = '';
        this.estadoPago = 0;
        this.fechaEnvioCobranza = new Date();
        this.horaEnvioCobranza = '';
        this.correosEnvio = '';
    }
}

export class PagoCobranza {
    pagosCabecera?: PagosCabecera;
    pagosDetalle?: PagosDetalle[];
    pagosDocumentos?: PagosDocumentos[];

    constructor() {
        this.pagosCabecera = new PagosCabecera();
        this.pagosDetalle = [];
        this.pagosDocumentos = [];
    }
}