export class Usuario {
    idUsuario?: number;
    nombres?: string;
    apellidos?: string;
    email?: string;
    password?: string;
    activo?: number;
    idPerfil?: number;
    fechaCreacion?: Date;
    FechaEnvioValidacion?: Date;
    CuentaActivada?: number;
    nombrePerfil?: string;

    constructor() {
        this.idUsuario = 0;
        this.nombres = '';
        this.apellidos = '';
        this.email = '';
        this.password = '';
        this.activo = 0;
        this.idPerfil = 0;
        this.fechaCreacion = new Date;
        this.FechaEnvioValidacion = new Date;
        this.CuentaActivada = 0;
        this.nombrePerfil = '';
    }
}