export class PasarelaPago {
    idPasarela?: number;
    nombre?: string;
    codigoComercio?: string;
    secretKey?: string;
    protocolo?:string;
    ambiente?: string;
    tipoDocumento?: String;
    cuentaContable?: string;
    logo?: string;
    estado?: number;
    monedaPasarela?: string;
    ambienteConsultarPago?: string;
    esProduccion?: number

    constructor() {
        this.idPasarela = 0;
        this.nombre = '';
        this.codigoComercio = '';
        this.secretKey = '';
        this.protocolo = '';
        this.ambiente = '';
        this.tipoDocumento = '';
        this.cuentaContable = '';
        this.logo = '';
        this.estado = 0;
        this.monedaPasarela = '';
        this.ambienteConsultarPago = '';
        this.esProduccion = 0
    }
}