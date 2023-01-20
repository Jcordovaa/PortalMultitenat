export class Acceso {
    idAcceso?: number;
    nombre?: string;
    menuPadre?: number;
    activo?: number;
    accesoPadre?: Acceso;
    totalFilas?: number;
    checked?: boolean;

    constructor() {
        this.idAcceso = 0;
        this.nombre = '';
        this.menuPadre = 0;
        this.activo = 1;
        this.accesoPadre = null;
        this.totalFilas = 0;
        this.checked = false;
    }
}

export class Perfil {
    idPerfil?: number;
    nombre?: string;
    descripcion?: string;
    totalFilas?: number;

    constructor() {
        this.idPerfil = 0;
        this.nombre = '';
        this.descripcion = '';
        this.totalFilas = 0;
    }
}

export class Usuarios {
    idUsuario?: number;
    nombres?: string;
    apellidos?: string;
    sexo?: string;
    fechaNacimiento?: Date;
    email?: string;
    password?: string;
    activo?: number;
    idPerfil?: number;
    fechaCreacion?: Date;
    telefono?: string;
    codigoArea?: string;
    nombreUsuario?: string;
    cuentaActivada?: number;
    totalFilas?: number;
    perfil?: Perfil;

    constructor() {
        this.idUsuario = 0;
        this.nombres = '';
        this.apellidos = '';
        this.sexo = '';
        this.fechaNacimiento = new Date();
        this.email = '';
        this.password = '';
        this.activo = 1;
        this.idPerfil = 0;
        this.fechaCreacion = new Date();
        this.telefono = '';
        this.codigoArea = '';
        this.nombreUsuario = '';
        this.cuentaActivada = 0;
        this.totalFilas = 0;
        this.perfil = new Perfil();
    }
}

export class Permisos {
    idPermiso?: number;
    idPerfil?: number;
    idAcceso?: number;
    perfil?: Perfil;
    acceso?: Acceso;
    totalFilas?: number;
    checked?: boolean;

    constructor() {
        this.idPermiso = 0;
        this.idPerfil = 0;
        this.idAcceso = 0;
        this.perfil = new Perfil();
        this.acceso = new Acceso();
        this.totalFilas = 0;
        this.checked = false;
    }
}