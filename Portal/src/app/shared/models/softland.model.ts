export class CondicionVentaDTO {
    cveCod?: string;
    cveDes?: string;
    cveDias?: number;
    cveNvCto?: number;
    disabled?: boolean;
    constructor() {
        this.cveCod = '';
        this.cveDes = '';
        this.cveDias = 0;
        this.cveNvCto = 0;
        this.disabled = false;
    }
}


export class ListaPrecioDTO {
    codLista?: string;
    desLista?: string;
    disabled?: boolean;
    constructor() {
        this.codLista = '';
        this.desLista = '';
        this.disabled = false;
    }
}

export class CategoriaClienteDTO {
    catCod?: string;
    catDes?: string;
    disabled?: boolean;
    constructor() {
        this.catCod = '';
        this.catDes = '';
        this.disabled = false;
    }
}

export class VendedorDTO {
    venCod?: string;
    venDes?: string;
    codTipV?: string;
    email?: string;
    usuario?: string;
    disabled?: boolean;

    constructor() {
        this.venCod = '';
        this.venDes = '';
        this.codTipV = '';
        this.email = '';
        this.usuario = '';
        this.disabled = false;
    }
}


export class TipoDocumento {
    codDoc?: string;
    desDoc?: string;
    disabled?: boolean;

    constructor() {
        this.codDoc = '';
        this.desDoc = '';
        this.disabled = false;
    }
}