import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TiposDespacho, TipoDesp } from '../models/tiposdespacho.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class TiposDespachoService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'TiposDespachos';
    }

    getTipoDespacho(idTipoDespacho: number): Observable<TiposDespacho> {
        return this.http.get<TiposDespacho>(`${this.apiUrl}/${idTipoDespacho}`);
    }

    getTipoDesp(): Observable<TipoDesp[]> {
        return this.http.get<TipoDesp[]>(`${this.apiUrl}/getTiposDesp`);
    }

    getTiposDespacho(): Observable<TiposDespacho[]> {
        return this.http.get<TiposDespacho[]>(this.apiUrl);
    }

    save(pf: TiposDespacho): Observable<TiposDespacho> {
        const body = JSON.stringify(pf);
        return this.http.post<TiposDespacho>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(pf: TiposDespacho): Observable<TiposDespacho> {
        const body = JSON.stringify(pf);
        return this.http.put<TiposDespacho>(`${this.apiUrl}/${pf.idTipoDespacho}`, body, this.utils.getHeaders(false));
    }

    delete(idTipoDespacho: number) {
        return this.http.delete(`${this.apiUrl}/${idTipoDespacho}`, this.utils.getHeaders(false))
    }

    getTiposDespachosByPage(paginador: Paginator): Observable<TiposDespacho[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<TiposDespacho[]>(`${this.apiUrl}/GetTiposDespachosByPage`, body, this.utils.getHeaders(false));
    }
}
