import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cupon, TipoDescuento } from '../models/cupones.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class CuponesService {

    private apiUrl: string = '';
    private apiTDUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'cupon';
        this.apiTDUrl = this.utils.ServerWithApiUrl + 'tipoDescuento';
    }

    getCupon(idBanner: number): Observable<Cupon> {
        return this.http.get<Cupon>(`${this.apiUrl}/${idBanner}`);
    }

    getCupones(): Observable<Cupon[]> {
        return this.http.get<Cupon[]>(this.apiUrl);
    }

    getTiposDescuento(): Observable<TipoDescuento[]> {
        return this.http.get<TipoDescuento[]>(this.apiTDUrl);
    }

    save(cupon: Cupon): Observable<Cupon> {
        const body = JSON.stringify(cupon);
        return this.http.post<Cupon>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(cupon: Cupon): Observable<Cupon> {
        const body = JSON.stringify(cupon);
        return this.http.put<Cupon>(`${this.apiUrl}/${cupon.idCupon}`, body, this.utils.getHeaders(false));
    }

    delete(idCupon: number) {
        return this.http.delete(`${this.apiUrl}/${idCupon}`, this.utils.getHeaders(false))
    }

    getCuponesByPage(paginador: Paginator): Observable<Cupon[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<Cupon[]>(`${this.apiUrl}/GetCuponesByPage`, body, this.utils.getHeaders(false));
    }

}
