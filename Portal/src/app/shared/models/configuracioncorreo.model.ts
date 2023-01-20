
export class ConfiguracionCorreo {
    idConfiguracionCorreo?: number;
    smtpServer?: string;
    usuario?: string;
    clave?: string;
    puerto?: number;
    ssl?: number;
    correoAvisoPago?: string;
    asuntoPagoCliente?: string;
    cantidadCorreosAcceso?: number;
    cantidadCorreosNotificacion?: number;
    correoOrigen?: string;

    constructor() {
        this.idConfiguracionCorreo = 0;
        this.smtpServer = '';
        this.usuario = '';
        this.clave = '';
        this.puerto = 0;
        this.ssl = 0;
        this.correoAvisoPago = '';
        this.asuntoPagoCliente = '';  
        this.cantidadCorreosAcceso = 0;
        this.cantidadCorreosNotificacion = 0;
        this.correoOrigen = '';
    }
  
  }