import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfiguracionPagoCliente, ConfiguracionPortal, ConfiguracionTiposDocumentos } from '../models/configuracioncobranza.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionPagoClientesService {

    private apiUrl: string = '';
    private apiUrlPortal: string = '';
    private apiUrlTipoDocs: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfiguracionPagoCliente';
        this.apiUrlPortal = this.utils.ServerWithApiUrl + 'ConfiguracionPortal';
        this.apiUrlTipoDocs = this.utils.ServerWithApiUrl + 'ConfiguracionTiposDocumentos';
    }

    getConfigPagoClientes() {
        return this.http.get(`${this.apiUrl}/GetConfiguracion`, this.utils.getHeaders(false));
    }

    save(data: ConfiguracionPagoCliente) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrl}`, body, this.utils.getHeaders(true));
    }


    edit(data: ConfiguracionPagoCliente) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrl}/actualizaConfiguracionPago`, body, this.utils.getHeaders(true));
    }

    actualizaDiasPorVencer(dias: number) {
        return this.http.post(`${this.apiUrl}/ActualizaDiasPorVencer/${dias}`,null, this.utils.getHeaders(true));
    }

    saveConfigTiposDocs(data: ConfiguracionTiposDocumentos[]) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrlTipoDocs}/saveConfigTiposDocs`, body, this.utils.getHeaders(true));
    }


    getConfigPortal(): Observable<ConfiguracionPortal> {
        return this.http.get<ConfiguracionPortal>(`${this.apiUrlPortal}/GetConfiguracionPortal`, this.utils.getHeaders(false));
    }

    // editPortal(data: ConfiguracionPortal) {
    //     const body = JSON.stringify(data);
    //     return this.http.put(`${this.apiUrlPortal}/${data.idConfiguracionPortal}`, body, this.utils.getHeaders(false));
    // }

    editPortal(data: ConfiguracionPortal, dias : number) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrlPortal}/actualizaConfiguracion/${dias}`, body, this.utils.getHeaders(true));
    }
}