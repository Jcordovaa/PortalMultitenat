export class Marcas {
    idMarca?: number;
    nombre?: string;
    rutaImagen?: string;
    estado?: number;
    orden?: number;
    url?: string;
    totalFilas?: number = 0;

    constructor() {
        this.idMarca = 0;
        this.nombre = '';
        this.rutaImagen = '';
        this.estado = 1;
        this.orden = 0;
        this.url = '';
        this.totalFilas = 0;
    }

}