import { UnidadMedida } from './unidadmedida.model';
import { Categorias } from './categorias.model';

export class Producto {
    codProducto?: string;
    nombre?: string;
    codigoBarras?: string;
    precioVenta?: number;
    idCategoria?: number;
    categoriaN1?: number;
    categoriaN2?: number;
    categoriaN3?: number;
    idUmed?: number;
    estado?: number;
    esSoftland?: number;
    pesokg?: number;
    productoDestacado?: number;
    ordenDestacado?: number;
    productoAPedido?: number;
    codGrupoSoftland?: string;
    codSubGrupoSoftland?: string;
    productoOferta?: number;
    productoCategoriaDestacada?: number;
    totalFilas?: number;
    unidadMedida?: UnidadMedida;
    categorias?: Categorias;
    productosImagenes?: ProductosImagenes[];
    productoAtributos?: ProductoAtributos[];
  
    constructor() {
        this.codProducto = "";
        this.nombre = "";
        this.codigoBarras = "";
        this.precioVenta = 0;
        this.idCategoria = 0;
        this.categoriaN1 = 0;
        this.categoriaN2 = 0;
        this.categoriaN3 = 0;
        this.idUmed = 0;
        this.estado = 1;
        this.esSoftland = 0;
        this.pesokg = 0;
        this.productoDestacado = 0;
        this.ordenDestacado = 0;
        this.productoAPedido = 0;
        this.codGrupoSoftland = "";
        this.codSubGrupoSoftland = "";
        this.productoOferta = 0;
        this.productoCategoriaDestacada = 0;
        this.unidadMedida = null;
        this.categorias = null;
        this.productosImagenes = [];
        this.productoAtributos = [];
    }
  
  }

export class ProductosImagenes {
    idProductoImagen: number;
    codProducto?: string;
    rutaImagen?: string;
    estado?: number;
    orden?: number;

    constructor() {
        this.idProductoImagen = 0;
        this.codProducto = '';
        this.rutaImagen = '';
        this.estado = 0;
        this.orden = 0;
    }
}

export class ProductoSinonimos {
    idSinonimo?: number;
    palabra?: string;
    sinonimo?: string;
    totalFilas?: number;

    constructor() {
        this.idSinonimo = 0;
        this.palabra = '';
        this.sinonimo = '';
        this.totalFilas = 0;
    }
}

export class ProductoAtributos {
    idAtributo?: number;
    nombre?: string;
    categoria?: string;
    codProducto?: string;
    valor?: string;
    origen?: string;
    codAtributoSoftland?: number;

    constructor() {
        this.idAtributo = 0;
        this.nombre = '';
        this.categoria = '';
        this.codProducto = '';
        this.valor = '';
        this.origen = '';
        this.codAtributoSoftland = 0;
    }
}