import { Region, Ciudad, Comuna } from '../models/ubicaciones.model';
import { CondicionDeVenta } from '../models/condicionventa.model';

export class Cliente {
    idCliente?: number;
    rut?: string;
    nombre?: string;
    email?: string;
    idGiro?: number;
    direccion?: string;
    direccionPosicionGPS?: string;
    telefono?: string;
    esReceptorDTE?: number;
    correoDTE?: string;
    estado?: number;
    codAuxSoftland?: string;
    claveAcceso?: string;
    esSoftland?: number;
    fechaRegistro?: Date;
    horaRegistro?: string;
    idRegion?: number;
    idCiudad?: number;
    idComuna?: number;
    idVendedor?: number;
    idCanalDeVenta?: number;
    idCondicionVenta?: number;
    idListaPrecios?: number;
    giro?: Giro;
    region?: Region;
    ciudad?: Ciudad;
    comuna?: Comuna;
    condicionDeVenta?: CondicionDeVenta;
    contactoCliente?: ContactoCliente;
    totalFilas?: number;

    constructor() {
        this.idCliente = 0;
        this.rut = '';
        this.nombre = '';
        this.email = '';
        this.idGiro = 0;
        this.direccion = '';
        this.direccionPosicionGPS = '';
        this.telefono = '';
        this.esReceptorDTE = 0;
        this.correoDTE = '';
        this.estado = 1;
        this.codAuxSoftland = '';
        this.claveAcceso = '';
        this.esSoftland = 0;
        this.fechaRegistro = new Date();
        this.horaRegistro = '';
        this.idRegion = 0;
        this.idCiudad = 0;
        this.idComuna = 0;
        this.idVendedor = null;
        this.idCanalDeVenta = null;
        this.idCondicionVenta = null;
        this.idListaPrecios = null;
        this.giro = new Giro();
        this.totalFilas = 0;
        this.region = new Region();
        this.ciudad = new Ciudad();
        this.comuna = new Comuna();
        this.condicionDeVenta = new CondicionDeVenta();
        this.contactoCliente = new ContactoCliente();
    }
}

export class ContactoCliente {
    idContactoCliente?: number;
    rutCliente?: string;
    nombreContacto?: string;
    apellidoContacto?: string;
    idCargo?: number;
    telefono?: string;
    correoContacto?: string;

    constructor() {
        this.idContactoCliente = 0;
        this.rutCliente = '';
        this.nombreContacto = '';
        this.apellidoContacto = '';
        this.idCargo = null;
        this.telefono = '';
        this.correoContacto = '';
    }

}

export class Cargos {
    idCargo?: number;
    nombre?: string;
    estado?: number;
    codCargoSoftland?: string;

    constructor() {
        this.idCargo = 0;
        this.nombre = '';
        this.estado = 1;
        this.codCargoSoftland = '';
    }

}

export class Giro {
    idGiro?: number;
    nombre?: string;
    estado?: number;
    codGiroSoftland?: string;

    constructor() {
        this.idGiro = 0;
        this.nombre = '';
        this.estado = 0;
        this.codGiroSoftland = '';
    }

}