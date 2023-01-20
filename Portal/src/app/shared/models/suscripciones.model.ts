export class Suscripciones {
    idSuscripcion?: number;
    email?: string;
    fechaSuscripcion?: Date;
    totalFilas?: number;

    constructor() {
        this.idSuscripcion = 0;
        this.email = '';
        this.fechaSuscripcion = new Date();
        this.totalFilas = 0;
    }
}