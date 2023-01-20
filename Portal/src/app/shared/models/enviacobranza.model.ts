export class EnviaCobranza {
    id?: number;
    rut?: string;
    codAux?: string;
    documentosACobrar?: string;
    fechaEnvio?: Date;
    estado?: number;
    correos?: string;

  
    constructor() {
        this.id = 0;
        this.rut = "";
        this.codAux = "";
        this.estado = 1;
        this.correos = "";
        this.documentosACobrar = "";
        this.fechaEnvio = new Date();
    }
  
  }