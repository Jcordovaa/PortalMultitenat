export class Configuracion {
    idConfiguracion?: number;
    mensajeBienvenida?: string;
    muestraComparador?: number;
    muestraCotizacion?: number;
    muestraCatalogos?: number;
    muestraMarcas?: number;
    muestraSucursales?: number;
    muestraCatalogo?: number;
    tipoBanner?: number;
    categoriasDestacadas?: number;
    sigueTuCompra?: number;
    caracteristicasEmpresa?: number;
    recomendarProducto?: number;
    mostrarUnidadMedida?: number;
    productoAPedido?: number;
    muestraPrecioPedido?: number;
    muestraCupones?: number;
    muestraTerminos?: number;
    muestraEstadoCuenta?: number;
    permitePagoEstadoCuenta?: number;
    actualizaDatosAuxiliar?: number;
    muestraDocVenta?: number;
    muestraObservaciones?: number;
    generaVenta?: number;
    muestraDocumentoCompra?: number;
    stockPorSucursal?: number;
    consultaSII?: number;
    precioOfertaProductos?: number;

    constructor() {
        this.idConfiguracion = 0;
        this.mensajeBienvenida = 'Bienvenido a Berrylion Chile';
        this.muestraComparador = 1;
        this.muestraCotizacion = 1;
        this.muestraCatalogos = 1;
        this.muestraMarcas = 1;
        this.muestraSucursales = 1;
        this.muestraCatalogo = 1;
        this.tipoBanner = 1;
        this.categoriasDestacadas = 1;
        this.sigueTuCompra = 1;
        this.caracteristicasEmpresa = 1;
        this.recomendarProducto = 1;
        this.mostrarUnidadMedida = 1;
        this.productoAPedido = 1;
        this.muestraPrecioPedido = 1;
        this.muestraCupones = 1;
        this.muestraTerminos = 1;
        this.muestraEstadoCuenta = 1;
        this.permitePagoEstadoCuenta = 1;
        this.actualizaDatosAuxiliar = 1;
        this.muestraDocVenta = 1;
        this.muestraObservaciones = 1;
        this.generaVenta = 1;
        this.muestraDocumentoCompra = 1;
        this.stockPorSucursal = 1;
        this.consultaSII = 1;
        this.precioOfertaProductos = 1;
    }

}

export class ConfiguracionSoftland {
    idConfiguracionSoftland?: number;
    codListaPrecios?: string;
    codListaPreciosOferta?: string;
    codCondicionVenta?: string;
    codCanalVenta?: string;
    codCargo?: string;
    codCategoriaCliente?: string;
    codGiro?: string;
    stockMinimo?: number;
    codVendedor?: string;
    clienteBoleta?: string;
    codCentroCosto?: string;
    letraGiro?: string;
    numeroGiro?: number;
    cuentaContable?: string;
    areaNegocioPago?: string;
    tipoDocumentoPago?: string;
    cantidadDecimales?: number;
    cuentasContablesDocVtas?: string;

    constructor() {
        this.idConfiguracionSoftland = 0;
        this.codListaPrecios = '';
        this.codListaPreciosOferta = '';
        this.codCondicionVenta = '';
        this.codCanalVenta = '';
        this.codCargo = '';
        this.codCategoriaCliente = '';
        this.codGiro = '';
        this.stockMinimo = 0;
        this.codVendedor = '';
        this.clienteBoleta = '';
        this.codCentroCosto = '';
        this.letraGiro = '';
        this.numeroGiro = 0;
        this.cuentaContable = '';
        this.areaNegocioPago = '';
        this.tipoDocumentoPago = '';
        this.cantidadDecimales = 0;
        this.cuentasContablesDocVtas = '';
    }
}


export class ConfiguracionEmpresa {
    idConfiguracionEmpresa?: number;
    rutEmpresa?: string;
    nombreEmpresa?: string;
    direccion?: string;
    rutaGoogleMaps?: string;
    telefono?: string;
    correoContacto?: string;
    facebook?: string;
    instagram?: string;
    twitter?: string;
    youtube?: string;
    linkedin?: string;
    urlPortal?: string; //FCA IMPLEMENTACION
    logo?: string;
    web?: string;

    constructor() {
        this.idConfiguracionEmpresa = 0;
        this.rutEmpresa = '';
        this.nombreEmpresa = '';
        this.direccion = '';
        this.rutaGoogleMaps = '';
        this.telefono = '';
        this.correoContacto = '';
        this.facebook = '';
        this.instagram = '';
        this.twitter = '';
        this.youtube = '';
        this.linkedin = '';
        //FCA IMPLEMENTACION
        this.urlPortal = '';
        this.logo = '';
        this.web = '';
    }
}