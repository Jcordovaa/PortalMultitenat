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
    private apiRegistro: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'ConfiguracionDiseno';
    }

    getConfigDiseno(): Observable<ConfiguracionDiseno> {
        return this.http.get<ConfiguracionDiseno>(`${this.apiUrl}/GetConfiguracion`, this.utils.getHeaders(false));
    }

    saveConfigDiseno(data: ConfiguracionDiseno, seccion: number) {
        const body = JSON.stringify(data);
        return this.http.post(`${this.apiUrl}/saveConfiguracion/${seccion}`, body, this.utils.getHeaders(true));
    }

    subirImagen(archivos: FileList, numeroImagen: number) {

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

                xhr.open('POST', `${this.apiUrl}/UploadImages/${numeroImagen}`,true);
                xhr.setRequestHeader('Authorization', this.utils.getHeaders(true).headers.get('Authorization'));
                xhr.send(formData);

            });    

    }


}
