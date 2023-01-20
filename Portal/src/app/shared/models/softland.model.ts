export class CondicionVentaDTO {
    cveCod?: string;
    cveDes?: string;
    cveDias?: number;
    cveNvCto?: number;
    constructor() {
        this.cveCod = '';
        this.cveDes = '';
        this.cveDias = 0;
        this.cveNvCto = 0;
    }
}


export class ListaPrecioDTO {
    codLista?: string;
    desLista?: string;
    constructor() {
        this.codLista = '';
        this.desLista = '';
    }
}

export class CategoriaClienteDTO {
    catCod?: string;
    catDes?: string;
    constructor() {
        this.catCod = '';
        this.catDes = '';
    }
}

export class VendedorDTO {
    venCod?: string;
    venDes?: string;
    codTipV?: string;
    email?: string;
    usuario?: string;

    constructor() {
        this.venCod = '';
        this.venDes = '';
        this.codTipV = '';
        this.email = '';
        this.usuario = '';
    }
}


export class TipoDocumento {
    codDoc?: string;
    desDoc?: string;

    constructor() {
        this.codDoc = '';
        this.desDoc = '';
    }
}