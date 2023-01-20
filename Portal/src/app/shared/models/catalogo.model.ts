export class Catalogo {
    idCatalogo?: number;
    nombre?: string;
    rutaArchivo?: string;
    rutaImagen?: string;
    estado?: number;
    totalFilas?: number;

    constructor() {
        this.idCatalogo = 0;
        this.nombre = '';
        this.rutaArchivo = '';
        this.rutaImagen = '';
        this.estado = 1;
        this.totalFilas = 0;
    }

}