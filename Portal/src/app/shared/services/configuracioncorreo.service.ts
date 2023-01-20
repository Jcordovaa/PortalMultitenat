import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfiguracionCorreo } from '../models/configuracioncorreo.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class ConfiguracionCorreoService {

    private apiUrl: string = '';
    private apiURL: string = '';
    private apiMail : string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfiguracionCorreo';
        this.apiURL = this.utils.ServerWithApiUrl + 'ConfiguracionCorreoCasillas';
        this.apiMail = this.utils.ServerWithApiUrl + 'Mail';
    }

    getConfigCorreo (idConfiguracionCorreo: number): Observable<ConfiguracionCorreo> {
        return this.http.get<ConfiguracionCorreo>(`${this.apiUrl}/${idConfiguracionCorreo}`, this.utils.getHeaders(true));
    }

    getConfigCorreos(): Observable<ConfiguracionCorreo[]> {
        return this.http.get<ConfiguracionCorreo[]>(`${this.apiUrl}/GetConfiguracionCorreo`, this.utils.getHeaders(true));
    }

    // edit(config: ConfiguracionCorreo): Observable<ConfiguracionCorreo> {
    //     const body = JSON.stringify(config);
    //     return this.http.put<ConfiguracionCorreo>(`${this.apiUrl}/${config.idConfiguracionCorreo}`, body, this.utils.getHeaders(false));
    // }

    edit(config: ConfiguracionCorreo): Observable<ConfiguracionCorreo> {
        const body = JSON.stringify(config);
        return this.http.post<ConfiguracionCorreo>(`${this.apiUrl}/actualizaConfiguracionCorreo`, body, this.utils.getHeaders(true));
    }

    getCantidad(casilla: string): Observable<number> {
        // const body = JSON.stringify(casilla);
        return this.http.get<number>(`${this.apiMail}/GetCantidadPorDia/${casilla}`, this.utils.getHeaders(true));
    }

    getCantidadEnviadaDia(): Observable<number> {
        return this.http.get<number>(`${this.apiMail}/GetCantdidEnviada`, this.utils.getHeaders(true));
    }
    

}
