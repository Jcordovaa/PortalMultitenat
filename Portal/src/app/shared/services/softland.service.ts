import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Catalogo } from '../models/catalogo.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'
import { CategoriaClienteDTO, CondicionVentaDTO, ListaPrecioDTO, VendedorDTO } from '../models/softland.model';
import { CanalVenta } from '../models/canalventa.model';
import { Cobrador } from '../models/cobrador.model';

@Injectable({
    providedIn: 'root'
})
export class SoftlandService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'softland';

   
    }




    getCondicionesVenta(): Observable<CondicionVentaDTO[]> {
        return this.http.get<CondicionVentaDTO[]>(`${this.apiUrl}/GetCondVentas`, this.utils.getHeaders(true));
    }

    getListasPrecio(): Observable<ListaPrecioDTO[]> {
        return this.http.get<ListaPrecioDTO[]>(`${this.apiUrl}/GetListasPrecio`, this.utils.getHeaders(true));
    }

    getCategoriasCliente(): Observable<CategoriaClienteDTO[]> {
        return this.http.get<CategoriaClienteDTO[]>(`${this.apiUrl}/GetCategoriasCliente`, this.utils.getHeaders(true));
    }

    getVendedores(): Observable<VendedorDTO[]> {
        return this.http.get<VendedorDTO[]>(`${this.apiUrl}/GetVendedores`, this.utils.getHeaders(true));
    }

    getCanalesVenta(): Observable<CanalVenta[]> {
        return this.http.get<CanalVenta[]>(`${this.apiUrl}/GetCanalesVenta`, this.utils.getHeaders(true));
    }


    getCobradores(): Observable<Cobrador[]> {
        return this.http.get<Cobrador[]>(`${this.apiUrl}/GetCobradores`, this.utils.getHeaders(true));
    }

    getExistModuloInventario(): Observable<boolean> {
        return this.http.get<boolean>(`${this.apiUrl}/GetExistModuloInventario`, this.utils.getHeaders(true));
    }

    getExistModuloNotaVenta(): Observable<boolean> {
        return this.http.get<boolean>(`${this.apiUrl}/GetExistModuloNotaVenta`, this.utils.getHeaders(true));
    }

    getExistModuloContabilidad(): Observable<boolean> {
        return this.http.get<boolean>(`${this.apiUrl}/GetExistModuloContabilidad`, this.utils.getHeaders(true));
    }
}
