import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfiguracionEmpresa } from '../models/configuracion.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionEmpresaService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfiguracionEmpresa';
    }


    getConfig() {
        return this.http.get(`${this.apiUrl}/GetConfiguracionEmpresa`, this.utils.getHeaders(true));
    }

}
