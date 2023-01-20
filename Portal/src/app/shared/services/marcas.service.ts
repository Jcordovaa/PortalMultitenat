import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Marcas } from '../models/marcas.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class MarcasService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'marcas';
  }

  getMarca(idMarca: number): Observable<Marcas> {
    return this.http.get<Marcas>(`${this.apiUrl}/${idMarca}`);
  }

  getMarcas(): Observable<Marcas[]> {
    return this.http.get<Marcas[]>(this.apiUrl);
  }

  save (marca: Marcas): Observable<Marcas> {
    const body = JSON.stringify(marca);
    return this.http.post<Marcas>(this.apiUrl, body, this.utils.getHeaders(false));
  }

  edit (marca: Marcas): Observable<Marcas> {
    const body = JSON.stringify(marca);
    return this.http.put<Marcas>(`${this.apiUrl}/${marca.idMarca}`, body, this.utils.getHeaders(false));
  }

  delete (idMarca: number) {
    return this.http.delete(`${this.apiUrl}/${idMarca}`, this.utils.getHeaders(false))
  }

  getMarcasByPage(paginador: Paginator): Observable<Marcas[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Marcas[]>(`${this.apiUrl}/GetMarcasByPage`, body, this.utils.getHeaders(false));
}

  subirImagen(archivos: FileList, idMarca: number) {

    return new Promise((resolve, reject) => {

        let formData = new FormData();
        let xhr = new XMLHttpRequest();

        for (let i = 0; i <= archivos.length - 1; i++) {
            formData.append(idMarca.toString(), archivos[i], archivos[i].name);
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

        xhr.open('POST', this.utils.ServerWithApiUrl + 'fileUpload/uploadMarca/' + idMarca, true);
        xhr.send(formData);

    });
}

}
