import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Venta, VentaDetalle } from '../models/ventas.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class VentasService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ventas';
    }

    getVenta(idVenta: number): Observable<Venta> {
        return this.http.get<Venta>(`${this.apiUrl}/${idVenta}`);
    }

    getDetallesVenta(idVenta: number): Observable<VentaDetalle[]> {
        return this.http.get<VentaDetalle[]>(`${this.apiUrl}/GetDetallesVenta/${idVenta}`);
    }

    getVentas(): Observable<Venta[]> {
        return this.http.get<Venta[]>(this.apiUrl);
    }

    save(venta: Venta): Observable<Venta> {
        const body = JSON.stringify(venta);
        return this.http.post<Venta>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    delete(idVenta: number) {
        return this.http.delete(`${this.apiUrl}/${idVenta}`, this.utils.getHeaders(false))
    }

    getVentasByPage(paginador: Paginator): Observable<Venta[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<Venta[]>(`${this.apiUrl}/GetVentasByPage`, body, this.utils.getHeaders(false));
    }

    getLogTbk(idPago: number) {
        return this.http.get(`${this.apiUrl}/getLogTbk/${idPago}`);
    }

}
