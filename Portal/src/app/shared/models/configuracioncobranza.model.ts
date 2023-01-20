export class ConfiguracionCobranzas {
    idConfigCobranza?: number;
    tipoDocumentoCobranza?: string;
    condicionesCredito?: string;
    enviaCobranza?: number;
    cantidadDiasVencimiento?: number;
    idFrecuenciaEnvioCob?: number;
    enviaPreCobranza?: number;
    cantidadDiasPrevios?: number;
    idFrecuenciaEnvioPre?: number;

    constructor() {
        this.idConfigCobranza = 0;
        this.tipoDocumentoCobranza = '';
        this.condicionesCredito = '';
        this.enviaCobranza = 0;
        this.cantidadDiasVencimiento = 0;
        this.idFrecuenciaEnvioCob = 0;
        this.enviaPreCobranza = 0;
        this.cantidadDiasPrevios = 0;
        this.idFrecuenciaEnvioPre = 0;
    }
}

export class ConfiguracionCargos {
    idCargosConfig?: number;
    idConfiguracion?: number;
    nombre?: string;
    codErp?: string;
    tipoConfiguracion?: number;

    constructor() {
        this.idCargosConfig = 0;
        this.idConfiguracion = 0;
        this.nombre = '';
        this.codErp = '';
        this.tipoConfiguracion = 0;
    }
}

export class ListaClientesExcluidos {
    idListaClientes?: number;
    idConfiguracion?: number;
    rut?: string;
    razonSocial?: string;

    constructor() {
        this.idListaClientes = 0;
        this.idConfiguracion = 0;
        this.rut = '';
        this.razonSocial = '';
    }
}

export class ConfiguracionTiposDocumentos {
    idTipoDocConfig?: number;
    idConfiguracion?: number;
    nombre?: string;
    codErp?: string;

    constructor() {
        this.idTipoDocConfig = 0;
        this.idConfiguracion = 0;
        this.nombre = '';
        this.codErp = '';
    }
}

export class ConfiguracionPagoCliente {
    idConfiguracionPago?: number;
    cuentasContablesDeuda?: string;
    tiposDocumentosDeuda?: string;
    anioTributario?: number;
    monedaUtilizada?: string;
    centroCosto?: string;
    areaNegocio?: string;
    glosaComprobante?: string;
    glosaDetalle?: string;
    glosaPago?: string;
    segundaMonedaUtilizada?: string;
    diasPorVencer?: number;

    constructor() {
        this.idConfiguracionPago = 0;
        this.cuentasContablesDeuda = '';
        this.tiposDocumentosDeuda = '';
        this.anioTributario = 0;
        this.monedaUtilizada = '';
        this.centroCosto = '';
        this.areaNegocio = '';
        this.glosaComprobante = '';
        this.glosaDetalle = '';
        this.glosaPago = '';
        this.segundaMonedaUtilizada = '';
        this.diasPorVencer = 0;
    }
}

export class ConfiguracionPortal {
    idConfiguracionPortal?: number;
    crmSoftland?: number;
    inventarioSoftland?: number;
    contabilidadSoftland?: number;
    notaVentaSoftland?: number;
    cantidadUltimasCompras?: number;
    muestraEstadoBloqueo?: number;
    muestraEstadoSobregiro?: number;
    muestraContactoComercial?: number;
    muestraContactosPerfil?: number;
    muestraContactosEnvio?: number
    habilitaPagoRapido?: number;
    imagenCaberaPerfil?: string;
    imagenCabeceraCompras?: string;
    permiteExportarExcel?: number;
    permiteAbonoParcial?: number;   
    utilizaNumeroDireccion?: number;
    //FCA 01-07-2022
    muestraUltimasCompras?: number;
    muestraBotonImprimir?: number;
    muestraBotonEnviarCorreo?: number;
    muestraResumen?: number;
    resumenContabilizado?: number;
    muestraComprasFacturadas?: number;
    muestraPendientesFacturar?: number;
    muestraProductos?: number;
    muestraGuiasPendientes?: number;
    utilizaDocumentoPagoRapido?: number;

    constructor() {
        this.idConfiguracionPortal = 0;
        this.crmSoftland = 0;
        this.inventarioSoftland = 0;
        this.contabilidadSoftland = 0;
        this.notaVentaSoftland = 0;
        this.cantidadUltimasCompras = 0;
        this.muestraEstadoBloqueo = 0;
        this.muestraEstadoSobregiro = 0;
        this.muestraContactoComercial = 0;
        this.muestraContactosPerfil = 0;
        this.habilitaPagoRapido = 0;
        this.imagenCaberaPerfil = '';
        this.imagenCabeceraCompras = '';
        this.permiteExportarExcel = 0;
        this.permiteAbonoParcial = 0;
        this.utilizaNumeroDireccion = 0;
        //FCA 01-07-2022
        this.muestraUltimasCompras = 0;
        this.muestraBotonImprimir = 0;
        this.muestraBotonEnviarCorreo = 0;
        this.muestraResumen = 0;
        this.resumenContabilizado = 0;
        this.muestraComprasFacturadas = 0;
        this.muestraPendientesFacturar = 0;
        this.muestraProductos = 0;
        this.muestraGuiasPendientes = 0;
        this.utilizaDocumentoPagoRapido = 0;
    }
}