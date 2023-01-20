export class Region {
    idRegion?: number;
    nombre?: string;
    estado?: number;
    codSoftland?: string;

    constructor() {
        this.idRegion = 0;
        this.nombre = '';
        this.estado = 0;
        this.codSoftland = '';
    }
}

export class Ciudad {
    idCiudad?: number;
    nombre?: string;
    estado?: number;
    idRegion?: number;
    codCiudadSoftland?: string;


    constructor() {
        this.idCiudad = 0;
        this.nombre = '';
        this.estado = 0;
        this.idRegion = 0;
        this.codCiudadSoftland = '';
    }
}

export class Comuna {
    idComuna?: number;
    nombre?: string;
    estado?: number;
    idCiudad?: number;
    idRegion?: number;
    codComunaSoftland?: string;

    constructor() {
        this.idComuna = 0;
        this.nombre = '';
        this.estado = 0;
        this.idCiudad = 0;
        this.idRegion = 0;
        this.codComunaSoftland = '';
    }
}

export class Ubicaciones {
    regiones?: Region[];
    ciudades?: Ciudad[];
    comunas?: Comuna[];

    constructor() {
        this.regiones = [];
        this.ciudades = [];
        this.comunas = [];
    }
}