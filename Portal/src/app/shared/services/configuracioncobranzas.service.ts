import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfiguracionCobranzas, ConfiguracionCargos, ListaClientesExcluidos } from '../models/configuracioncobranza.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionCobranzaService {

    private apiUrl: string = '';
    private apiUrlCargos: string = '';
    private apiUrlClientes: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfigracionCobranzas';
        this.apiUrlCargos = this.utils.ServerWithApiUrl + 'ConfiguracionCargos';
        this.apiUrlClientes = this.utils.ServerWithApiUrl + 'ListaClientesExcluidos';
    }

    getConfigCObranzas() {
        return this.http.get(`${this.apiUrl}`, this.utils.getHeaders(true));
    }

    save(data: ConfiguracionCobranzas) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrl}`, body, this.utils.getHeaders(true));
    }

    edit(data: ConfiguracionCobranzas) {
        const body = JSON.stringify(data);
        return this.http.put(`${this.apiUrl}/${data.idConfigCobranza}`, body, this.utils.getHeaders(true));
    }

    saveConfigCargos(data: ConfiguracionCargos[], tipo: number) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrlCargos}/SaveConfigCargos/${tipo}`, body, this.utils.getHeaders(true));
    }

    saveConfigClientes(data: ListaClientesExcluidos[]) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrlClientes}/saveConfigClientes`, body, this.utils.getHeaders(true));
    }

}