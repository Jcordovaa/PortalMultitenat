export class Banners {
    idBanner: number;
    idTipoBanner: number;
    nombre?: string;
    estado?: number;
    rutaImagen?: string;
    orden?: number;
    isSelected?: boolean;
    totalFilas?: number = 0;

    constructor() {
      this.idBanner = 0;
      this.idTipoBanner = 1;
      this.nombre = '';
      this.estado = 1;
      this.rutaImagen = '';
      this.orden = 1;
      this.isSelected = false;
      this.totalFilas = 0;
    }

}

export class TipoBanners {
  idTipoBanner: number;
  nombre: string;
  estado: number;

  constructor() {
    this.idTipoBanner = 0;
    this.nombre = '';
    this.estado = 0;
  }

}