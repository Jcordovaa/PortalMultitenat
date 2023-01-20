export class CondicionDeVenta {
    idCondicionDeVenta?: number;
    nombre?: string;
    descripcion?: string;
    estado?: number;
    codCondVtaSoftland?: string;

    constructor() {
        this.idCondicionDeVenta = 0;
        this.nombre = '';
        this.descripcion = '';
        this.estado = 0;
        this.codCondVtaSoftland = '';
    }

}