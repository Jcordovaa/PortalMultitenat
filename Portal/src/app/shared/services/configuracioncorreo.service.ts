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
    private apiRegistro : string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'configuracionCorreo';
        this.apiURL = this.utils.ServerWithApiUrl + 'ConfiguracionCorreoCasillas';
        this.apiRegistro = this.utils.ServerWithApiUrl + 'RegistroEnvioCorreos';
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

    actualizaTextos(tipo: number, config: ConfiguracionCorreo): Observable<ConfiguracionCorreo> {
        const body = JSON.stringify(config);
        return this.http.post<ConfiguracionCorreo>(`${this.apiUrl}/actualizaTextos/${tipo}`, body, this.utils.getHeaders(true));
    }

    getCantidad(casilla: string): Observable<number> {
        const body = JSON.stringify(casilla);
        return this.http.post<number>(`${this.apiURL}/GetCantidadPorDia`, body, this.utils.getHeaders(true));
    }

    getCantidadEnviadaDia(): Observable<number> {
        return this.http.get<number>(`${this.apiRegistro}/GetCantdidEnviada`, this.utils.getHeaders(true));
    }

    getTemplate(tipo: number, config: ConfiguracionCorreo): Observable<any> {
        const body = JSON.stringify(config);
        return this.http.post<any>(`${this.apiUrl}/getTemplate/${tipo}`, body, this.utils.getHeaders(true));
    }


    subirImagen(archivos: FileList) {
        debugger
        if(archivos == null){
            return Promise.resolve();
        }
            return new Promise((resolve, reject) => {

                let formData = new FormData();
                let xhr = new XMLHttpRequest();

                for (let i = 0; i <= archivos.length - 1; i++) {
                    formData.append(archivos[i].name, archivos[i], archivos[i].name);
                }

                xhr.onreadystatechange = function () {

                    if (xhr.readyState === 4) {

                        if (xhr.status === 200) {
                            resolve(JSON.parse(xhr.response));
                        } else {
                            reject(xhr.response);
                        }

                    }
                };

                xhr.open('POST', `${this.apiUrl}/uploadLogo`, true);
                xhr.setRequestHeader('Authorization', this.utils.getHeaders(true).headers.get('Authorization'));
                xhr.send(formData);

            });    

    }
    

}
