
export class Cobranza {
    idCobranza?: number;
    nombre?: string;
    fechaCreacion?: Date;
    horaCreacion?: string;
    idTipoCobranza?: number;
    estado?: number;
    idUsuario?: number;
    tipoProgramacion?: number;
    fechaInicio?: Date;
    fechaFin?: Date;
    horaDeEnvio?: number;
    diaSemanaEnvio?: string;
    diasToleranciaVencimiento?: number;
    idEstado?: number;
    anio?: number;
    tipoDocumento?: string;
    fechaDesde?: Date;
    fechaHasta?: Date;
    aplicaClientesExcluidos?: number;
    esCabeceraInteligente?: number;
    idCabecera?: number;
    cobranzaDetalle?: CobranzaDetalle[];
    enviaEnlacePago?: number;
    estaVencida?: number;
    nombreUsuario?: string;
    nombreEstado?: string;
    nombreTipoCobranza?: string;
    totalRecaudar?: number;
    totalRecaudado?: number;
    porcentajeRecaudacion?: number;
    cantidadDocumentosEnviadosCobrar?: number;
    cantidadDocumentosPagados?: number;
    porcentajePagoDocumentos?: number;
    colorPorcentajeRecaudacion?: string;
    horaEnvioTexto?: string;
    idPeriodicidad?: number; //FCA 07-12-2021
    excluyeFeriados?: number; //FCA 09-12-2021
    excluyeFinDeSemana?: number; //FCA 09-12-2021
    diaEnvio?: number; //FCA 09-12-2021
    listaPrecio?: string;
    condicionVenta?: string;
    categoriaCliente?: string;
    vendedor?: string;
    cargosContactos?: string;
    enviaTodosContactos?: number;
    enviaCorreoFicha?: number;
    enviaTodosCargos?: number;
    canalesVenta?: string;
    cobradores?: string;

    constructor() {
        this.idCobranza = 0;
        this.nombre = '';
        this.fechaCreacion = new Date();
        this.horaCreacion = '';
        this.idTipoCobranza = 0;
        this.estado = 0;
        this.idUsuario = 0;
        this.tipoProgramacion = 0;
        this.fechaInicio = new Date();
        this.fechaFin = new Date();
        this.horaDeEnvio = 0;
        this.diaSemanaEnvio = '';
        this.diasToleranciaVencimiento = 0;
        this.idEstado = 0;
        this.anio = 0;
        this.tipoDocumento = '';
        this.fechaDesde = new Date();
        this.fechaHasta = new Date();
        this.esCabeceraInteligente = 0;
        this.idCabecera = 0;
        this.cobranzaDetalle = [];
        this.enviaEnlacePago = 0;
        this.estaVencida = 0;
        this.nombreUsuario = '';
        this.nombreEstado = '';
        this.nombreTipoCobranza = '';
        this.totalRecaudado = 0;
        this.totalRecaudar = 0;
        this.porcentajeRecaudacion = 0;
        this.cantidadDocumentosEnviadosCobrar = 0;
        this.cantidadDocumentosPagados = 0;
        this.porcentajePagoDocumentos = 0;
        this.colorPorcentajeRecaudacion = 'danger';
        this.horaEnvioTexto = '';
        this.idPeriodicidad = null; //FCA 07-12-2021
        this.excluyeFeriados = 0; //FCA 09-12-2021
        this.excluyeFinDeSemana = 0; //FCA 09-12-2021
        this.diaEnvio = 0; //FCA 09-12-2021
        this.listaPrecio = '';
        this.categoriaCliente = '';
        this.condicionVenta = '';
        this.vendedor = '';
        this.cargosContactos = '';
        this.enviaCorreoFicha = 0;
        this.enviaTodosContactos = 0;
        this.enviaTodosCargos = 0;
        this.canalesVenta = '';
        this.cobradores = '';

    }

}

export class CobranzaDetalle {
    idCobranzaDetalle?: number;
    idCobranza?: number;
    folio?: number;
    fechaEmision?: Date;
    fechaVencimiento?: Date;
    monto?: number;
    rutCliente?: string;
    tipoDocumento?: string;
    idEstado?: number;
    fechaEnvio?: Date;
    horaEnvio?: string;
    fechaPago?: Date;
    horaPago?: string;
    comprobanteContable?: string;
    folioDTE?: string;
    idPago?: number;
    cuentaContable?: string;
    emailCliente?: string;
    selected?: boolean;
    codTipoDocumento?: string;
    nombreCliente?: string;
    nombreEstado?: string;
    fechaPagoTexto?: string;
    codAuxCliente?: string;

    constructor() {
        this.idCobranzaDetalle = 0;
        this.idCobranza = 0;
        this.folio = 0;
        this.fechaEmision = new Date();
        this.fechaVencimiento = new Date();
        this.monto = 0;
        this.rutCliente = '';
        this.tipoDocumento = '';
        this.idEstado = 0;
        this.fechaEnvio = new Date();
        this.horaEnvio = '';
        this.fechaPago = new Date();
        this.horaPago = '';
        this.comprobanteContable = '';
        this.folioDTE = '';
        this.idPago = 0;
        this.cuentaContable = '';
        this.emailCliente = '';
        this.selected = false;
        this.codTipoDocumento = '';
        this.nombreCliente = '';
        this.nombreEstado = '';
        this.fechaPagoTexto = '';
        this.codAuxCliente = '';
    }

}

export class EstadoCobranza {
    idEstadoCobranza?: number;
    nombre?: string;    

    constructor() {
        this.idEstadoCobranza = 0;
        this.nombre = '';        
    }
}

export class CobranzaPeriocidad {
    idPeriocidad?: number;
    nombre?: string;    
    diaMes?: number;
    estado?: number;

    constructor() {
        this.idPeriocidad = 0;
        this.nombre = '';  
        this.diaMes = 0;
        this.estado = 0;      
    }

}

