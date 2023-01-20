export class ResumenContable {
    montoAutorizado?: number;
    montoUtilizado?: number;
    disponible?: number;
    estadoBloqueo?: string;
    estadoSobregiro?: string;

    constructor() {
        this.montoAutorizado = 0;
        this.montoUtilizado = 0;
        this.disponible = 0;
        this.estadoBloqueo = '';
        this.estadoSobregiro = '';
    }
}