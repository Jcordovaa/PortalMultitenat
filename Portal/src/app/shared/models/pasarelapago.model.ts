export class PasarelaPago {
    idPasarela?: number;
    nombre?: string;
    codigoComercioTBK?: string;
    apiKeyPasarela?: string;
    secretKeyPasarela?: string;
    protocolo?:string;
    ambiente?: string;
    tipoDocumento?: String;
    cuentaContable?: string;
    logo?: string;
    estado?: number;
    monedaPasarela?: string;

    constructor() {
        this.idPasarela = 0;
        this.nombre = '';
        this.codigoComercioTBK = '';
        this.apiKeyPasarela = '';
        this.secretKeyPasarela = '';
        this.protocolo = '';
        this.ambiente = '';
        this.tipoDocumento = '';
        this.cuentaContable = '';
        this.logo = '';
        this.estado = 0;
        this.monedaPasarela = '';
    }
}