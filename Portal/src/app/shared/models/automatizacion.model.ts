export class Automatizacion {
    idAutomatizacion?: number;
    nombre?: string;
    idTipoAutomatizacion?: number;
    anio?: number;
    tipoDocumentos?: string;
    diasVencimiento?: number;
    idHorario?: number;
    idPerioricidad?: number;
    excluyeFestivos?: number;
    excluyeClientes?: number;
    codCategoriaCliente?: string;
    codListaPrecios?: string;
    codCondicionVenta?: string;
    codVendedor?: string;
    muestraSoloVencidos?: number;
    agrupaCobranza?: number;
    estado?: number;
    totalFilas?: number;
    diasRecordatorio?: number;
    diaEnvio?: number;
    codCanalVenta?: string;
    codCobrador?: string;
    codCargo?: string;
    enviaTodosContactos?: number;
    enviaCorreoFicha?: number;
    horaEnvio?: string;

    constructor() {
        this.idAutomatizacion = 0;
        this.nombre = '';
        this.idTipoAutomatizacion = null;
        this.anio = 0;
        this.tipoDocumentos = '';
        this.diasVencimiento = 0;
        this.idHorario = null;
        this.idPerioricidad = null;
        this.excluyeFestivos = 0;
        this.excluyeClientes = 0;
        this.codListaPrecios = '';
        this.codCategoriaCliente = '';
        this.codCondicionVenta = '';
        this.codVendedor = '';
        this.muestraSoloVencidos = 0;
        this.agrupaCobranza = 0;
        this.estado = 0;
        this.totalFilas = 0;
        this.diasRecordatorio = 0;
        this.diaEnvio = 0;
        this.codCanalVenta = '';
        this.codCobrador = '';
        this.codCargo = '';
        this.enviaCorreoFicha = 0;
        this.enviaTodosContactos = 0;
        this.horaEnvio = '';
    }

}


export class TipoAutomatizacion {
    idTipo?: number;
    nombre?: string;
    esPosterior?: number;
    estado?: number;


    constructor() {
        this.idTipo = 0;
        this.nombre = '';
        this.esPosterior = 0;
        this.estado = 0;
    }

}