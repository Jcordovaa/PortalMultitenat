import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PreguntasFrecuentes } from '../models/preguntasfrecuentes.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class PreguntasFrecuentesService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'preguntasfrecuentes';
    }

    getPreguntaFrecuente(idSinonimo: number): Observable<PreguntasFrecuentes> {
        return this.http.get<PreguntasFrecuentes>(`${this.apiUrl}/${idSinonimo}`);
    }

    getPreguntasFrecuentes(): Observable<PreguntasFrecuentes[]> {
        return this.http.get<PreguntasFrecuentes[]>(this.apiUrl);
    }

    save(pf: PreguntasFrecuentes): Observable<PreguntasFrecuentes> {
        const body = JSON.stringify(pf);
        return this.http.post<PreguntasFrecuentes>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(pf: PreguntasFrecuentes): Observable<PreguntasFrecuentes> {
        const body = JSON.stringify(pf);
        return this.http.put<PreguntasFrecuentes>(`${this.apiUrl}/${pf.idPregunta}`, body, this.utils.getHeaders(false));
    }

    delete(idPregunta: number) {
        return this.http.delete(`${this.apiUrl}/${idPregunta}`, this.utils.getHeaders(false))
    }

    getFaqByPage(paginador: Paginator): Observable<PreguntasFrecuentes[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<PreguntasFrecuentes[]>(`${this.apiUrl}/GetFaqByPage`, body, this.utils.getHeaders(false));
    }
}
