import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Catalogo } from '../models/catalogo.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class CatalogosService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'catalogos';
    }

    getCatalogo(idCatalogo: number): Observable<Catalogo> {
        return this.http.get<Catalogo>(`${this.apiUrl}/${idCatalogo}`);
    }

    getCatalogos(): Observable<Catalogo[]> {
        return this.http.get<Catalogo[]>(this.apiUrl);
    }

    save (catalogo: Catalogo): Observable<Catalogo> {
        const body = JSON.stringify(catalogo);
        return this.http.post<Catalogo>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit (catalogo: Catalogo): Observable<Catalogo> {
        const body = JSON.stringify(catalogo);
        return this.http.put<Catalogo>(`${this.apiUrl}/${catalogo.idCatalogo}`, body, this.utils.getHeaders(false));
    }

    delete (idCatalogo: number) {
        return this.http.delete(`${this.apiUrl}/${idCatalogo}`, this.utils.getHeaders(false))
    }

    getCatalogsByPage(paginador: Paginator): Observable<Catalogo[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<Catalogo[]>(`${this.apiUrl}/GetCatalogsByPage`, body, this.utils.getHeaders(false));
    }

    subirArchivo(archivos: FileList, idCatalogo: number, isImage: boolean) {

        return new Promise((resolve, reject) => {

            let formData = new FormData();
            let xhr = new XMLHttpRequest();

            for (let i = 0; i <= archivos.length - 1; i++) {
                formData.append(idCatalogo.toString(), archivos[i], archivos[i].name);
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

            xhr.open('POST', `${this.utils.ServerWithApiUrl}fileUpload/uploadCatalogo/${idCatalogo}/${isImage}`, true);
            xhr.send(formData);

        });
    }

}
