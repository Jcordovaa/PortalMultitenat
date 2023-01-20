export class Categorias {
    idCategoria?: number;
    nombre: string;
    imagen: string;
    estado: number;
    orden: number;
    codGrupoSoftland: string;
    categoriaDestacada: number;
    destacarSeccion: number;
    idCategoriaPadre: number;
    esCategoriaPadre: number;
    totalFilas?: number;
  
    constructor() {
        this.idCategoria = 0;
        this.nombre = "";
        this.imagen = "";
        this.estado = 1;
        this.orden = 1;
        this.codGrupoSoftland = "";
        this.categoriaDestacada = 0;
        this.destacarSeccion = 0;
        this.idCategoriaPadre = 0;
        this.esCategoriaPadre = 0;
        this.totalFilas = 0;
    }
  
  }