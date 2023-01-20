import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Categorias } from '../models/categorias.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class CategoriasService {

    private apiUrl: string = '';
    private apiUrlTipo: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'categorias';
    }

    getCategoria(idCat: number): Observable<Categorias> {
        return this.http.get<Categorias>(`${this.apiUrl}/${idCat}`);
    }

    getCategorias(): Observable<Categorias[]> {
        return this.http.get<Categorias[]>(this.apiUrl);
    }

    save(categoria: Categorias): Observable<Categorias> {
        const body = JSON.stringify(categoria);
        return this.http.post<Categorias>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(categoria: Categorias): Observable<Categorias> {
        const body = JSON.stringify(categoria);
        return this.http.put<Categorias>(`${this.apiUrl}/${categoria.idCategoria}`, body, this.utils.getHeaders(false));
    }

    delete(idCategoria: number) {
        return this.http.delete(`${this.apiUrl}/${idCategoria}`, this.utils.getHeaders(false))
    }

    getCategoriasByPage(paginador: Paginator): Observable<Categorias[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<Categorias[]>(`${this.apiUrl}/GetCategoriasByPage`, body, this.utils.getHeaders(false));
    }

    subirImagen(archivos: FileList, idCategoria: number) {

        return new Promise((resolve, reject) => {

            let formData = new FormData();
            let xhr = new XMLHttpRequest();

            for (let i = 0; i <= archivos.length - 1; i++) {
                formData.append(idCategoria.toString(), archivos[i], archivos[i].name);
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

            xhr.open('POST', this.utils.ServerWithApiUrl + 'fileUpload/uploadCategoria/' + idCategoria, true);
            xhr.send(formData);

        });
    }

}
