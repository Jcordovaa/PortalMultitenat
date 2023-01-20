import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utils } from '../../shared/utils'
import { ConfiguracionDiseno } from '../models/configuraciondiseno.model';

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionDisenoService {

    private apiUrl: string = '';
    private apiURL: string = '';
    private apiRegistro : string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfiguracionDiseno';
    }

    getConfigDiseno (): Observable<ConfiguracionDiseno> {
        return this.http.get<ConfiguracionDiseno>(`${this.apiUrl}/GetConfiguracion`, this.utils.getHeaders(false));
    }


}
