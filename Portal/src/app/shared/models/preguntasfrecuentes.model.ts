export class PreguntasFrecuentes {
    idPregunta?: number;
    pregunta?: string;
    respuesta?: string;
    estado?: number;
    totalFilas?: number;

    constructor() {
        this.idPregunta = 0;
        this.pregunta = '';
        this.respuesta = '';
        this.estado = 1;
        this.totalFilas = 0;
    }

}