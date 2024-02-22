import { ConfiguracionEmpresa } from "./configuracion.model"
import { ConfiguracionPagoCliente, ConfiguracionPortal } from "./configuracioncobranza.model"
import { ConfiguracionCorreo } from "./configuracioncorreo.model"
import { ConfiguracionDiseno } from "./configuraciondiseno.model"

export class EmpresaImplementacion {
    idEmpresa?: number
    rut?: string
    razonSocial?: string
    idEstado?: number
    tipoCliente?: number


    constructor() {
        this.idEmpresa = 0
        this.rut = ''
        this.razonSocial = ''
        this.idEstado = 1
        this.tipoCliente = 0
    }
}

export class Tenant {
    idTenant?: number
    idEmpresa?: number
    identifier?: string
    dominio?: string
    connectionString?: string
    estado?: number
    otImplementacion?: string
    nombreImplementador?: string
    telefonoImplementador?: string
    correoImplementador?: string
    fechaInicioImplementacion?: Date
    fechaTerminoImplementacion?: Date
    fechaInicioContrato?: Date
    fechaTerminoContrato?: Date
    idPlan?: number
    idImplementador?: number
    idLineaProducto?: number
    idAreaComercial?: number
    datosImplementacion?: DatosImplementacion
    rutEmpresa?: string
    nombreEmpresa?: string
    logoCorreo?: File
    imagenUltimasCompras?: File
    bannerPortal?: File
    imagenUsuario?: File
    iconoContactos?: File
    bannerMisCompras?: File
    iconoMisCompras?: File
    iconoClavePerfil?: File
    iconoEditarPerfil?: File
    iconoEstadoPerfil?: File
    imagenPortada?: File
    logoPortada?: File
    bannerPagoRapido?: File
    logoMinimalistaSidebar?: File
    logoSidebar?:  File

    constructor() {
        this.idTenant = 0
        this.idEmpresa = 0
        this.identifier = ''
        this.dominio = ''
        this.connectionString = ''
        this.estado = 0
        this.otImplementacion = ''
        this.nombreImplementador = ''
        this.telefonoImplementador = ''
        this.correoImplementador = ''
        this.fechaInicioImplementacion = null
        this.fechaTerminoImplementacion = null
        this.fechaInicioContrato = null
        this.fechaTerminoContrato = null
        this.idPlan = null
        this.idImplementador = null
        this.idLineaProducto = null
        this.idAreaComercial = null
        this.datosImplementacion = new DatosImplementacion
        this.rutEmpresa = ''
        this.nombreEmpresa = ''
        this.logoCorreo = null
        this.imagenUltimasCompras = null
        this.bannerPortal = null
        this.imagenUsuario = null
        this.iconoContactos = null
        this.bannerMisCompras = null
        this.iconoMisCompras = null
        this.iconoClavePerfil = null
        this.iconoEditarPerfil = null
        this.iconoEstadoPerfil = null
        this.imagenPortada = null
        this.logoPortada = null
        this.bannerPagoRapido = null
        this.logoMinimalistaSidebar = null
        this.logoSidebar = null
    }
}

export class DatosImplementacion {
    configuracionCorreo?: ConfiguracionCorreo
    apiSoftland?: ApiSoftland
    configuracionDiseno?: ConfiguracionDiseno
    configuracionEmpresa?: ConfiguracionEmpresa
    configuracionPagoCliente?: ConfiguracionPagoCliente
    configuracionPortal?: ConfiguracionPortal
    servidorPortal?: string
    baseDatosPortal?: string
    claveBaseDatosPortal?: string
    usuarioBaseDatosPortal?: string
    utilizaTransbank?: number
    utilizaVirtualPos?: number
    cuentaContableTransbank?: string
    cuentaContableVirtualPos?: string
    codigoComercioTransbank?: string
    ambienteTransbank?: number
    documentoContableTransbank?: string
    documentoContableVirtualPos?: string
    apiKeyTransbank?: string
    idServidorImplementacion?: number
  

    constructor() {
        this.configuracionCorreo = new ConfiguracionCorreo
        this.apiSoftland = new ApiSoftland
        this.configuracionDiseno = new ConfiguracionDiseno
        this.configuracionEmpresa = new ConfiguracionEmpresa
        this.configuracionPagoCliente = new ConfiguracionPagoCliente
        this.configuracionPortal = new ConfiguracionPortal
        this.servidorPortal = ''
        this.baseDatosPortal = ''
        this.claveBaseDatosPortal = ''
        this.usuarioBaseDatosPortal = ''
        this.utilizaTransbank = 0
        this.utilizaVirtualPos = 0
        this.cuentaContableTransbank = ''
        this.cuentaContableVirtualPos = ''
        this.codigoComercioTransbank = ''
        this.documentoContableVirtualPos = ''
        this.ambienteTransbank = 0
        this.apiKeyTransbank = ''
        this.idServidorImplementacion = 0
       
    }
}

export class ApiSoftland {
    id?: number
    ambiente?: string
    url?: string
    token?: string
    areaDatos?: string
    consultaTiposDeDocumentos?: string
    consultaPlanDeCuentas?: string
    consultaRegiones?: string
    consultaComunas?: string
    consultaGiros?: string
    contactosXauxiliar?: string
    consultaCliente?: string
    actualizaCliente?: string
    resumenContable?: string
    capturaComprobantes?: string
    documentosFacturados?: string
    detalleDocumento?: string
    obtenerPdfDocumento?: string
    despachoDeDocumento?: string
    productosComprados?: string
    pendientesPorFacturar?: string
    detalleNotaDeVenta?: string
    obtenerPdf?: string
    obtieneGuiasPendientes?: string
    login?: string
    obtieneVendedores?: string
    obtieneCondicionesVenta?: string
    obtieneListasPrecio?: string
    obtieneCategoriaClientes?: string
    documentosContabilizados?: string
    obtieneModulos?: string
    consultaMonedas?: string
    contabilizaPagos?: string
    consultaCargos?: string
    documentosContabilizadosResumen?: string
    topDeudores?: string
    transbankRegistrarCliente?: string
    docContabilizadosResumenxRut?: string
    pagosxDocumento?: string
    habilitaLogApi?: number
    cadenaAlmacenamientoAzure?: string
    urlAlmacenamientoArchivos?: string
    cuentasPasarelaPagos?: string
    consultaAuxiliar?: string
    reintentosCallback?: number
    reintentosRedirect?: number
    milisegundosReintoCalback?: number
    miliSegundosReintentoRedirect?: number

    constructor() {
        this.id = 0
        this.ambiente = ''
        this.url = ''
        this.token = ''
        this.areaDatos = ''
        this.consultaTiposDeDocumentos = ''
        this.consultaPlanDeCuentas = ''
        this.consultaRegiones = ''
        this.consultaComunas = ''
        this.consultaGiros = ''
        this.contactosXauxiliar = ''
        this.consultaCliente = ''
        this.actualizaCliente = ''
        this.resumenContable = ''
        this.capturaComprobantes = ''
        this.documentosFacturados = ''
        this.detalleDocumento = ''
        this.obtenerPdfDocumento = ''
        this.despachoDeDocumento = ''
        this.productosComprados = ''
        this.pendientesPorFacturar = ''
        this.detalleNotaDeVenta = ''
        this.obtenerPdf = ''
        this.obtieneGuiasPendientes = ''
        this.login = ''
        this.obtieneVendedores = ''
        this.obtieneCondicionesVenta = ''
        this.obtieneListasPrecio = ''
        this.obtieneCategoriaClientes = ''
        this.documentosContabilizados = ''
        this.obtieneModulos = ''
        this.consultaMonedas = ''
        this.contabilizaPagos = ''
        this.consultaCargos = ''
        this.documentosContabilizadosResumen = ''
        this.topDeudores = ''
        this.transbankRegistrarCliente = ''
        this.docContabilizadosResumenxRut = ''
        this.pagosxDocumento = ''
        this.habilitaLogApi = 0
        this.cadenaAlmacenamientoAzure = ''
        this.urlAlmacenamientoArchivos = ''
        this.cuentasPasarelaPagos = ''
        this.consultaAuxiliar = ''
        this.reintentosCallback = 0
        this.reintentosRedirect = 0
        this.milisegundosReintoCalback = 0
        this.miliSegundosReintentoRedirect = 0
    }

}

