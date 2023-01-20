import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Configuracion } from '../models/configuracion.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'configuracion';
    }

    getConfig() {
        return this.http.get(`${this.apiUrl}`);
    }

    edit(data: Configuracion) {
        const body = JSON.stringify(data);
        return this.http.put(`${this.apiUrl}/${data.idConfiguracion}`, body, this.utils.getHeaders(false));
    }

}
