import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Producto } from '../models/productos.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

export interface IPreviewImgs {
    name?: string;
    url?: string;
  }
@Injectable({
    providedIn: 'root'
})
export class ProductosService {

    private apiUrl: string = '';
    private apiUrlTipo: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'productos';
    }

    getProducto(codProducto: string): Observable<Producto> {
        return this.http.get<Producto>(`${this.apiUrl}/${codProducto}`);
    }

    getProductos(): Observable<Producto[]> {
        return this.http.get<Producto[]>(this.apiUrl);
    }

    save(producto: Producto): Observable<Producto> {
        const body = JSON.stringify(producto);
        return this.http.post<Producto>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(producto: Producto): Observable<Producto> {
        const body = JSON.stringify(producto);
        return this.http.put<Producto>(`${this.apiUrl}/${producto.codProducto}`, body, this.utils.getHeaders(false));
    }

    delete(codProducto: string) {
        return this.http.delete(`${this.apiUrl}/${codProducto}`, this.utils.getHeaders(false))
    }

    getProductosByPage(paginador: Paginator): Observable<Producto[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<Producto[]>(`${this.apiUrl}/GetProductosByPage`, body, this.utils.getHeaders(false));
    }

    deleteImage(codProducto: string, idImagenProducto: number) {
        return this.http.delete(`${this.apiUrl}/deleteImageOnly/${codProducto}/${idImagenProducto}`, this.utils.getHeaders(false))
    }

    subirImagen(archivos: FileList, previewImgs: IPreviewImgs[], codProducto: string, isNew: number) {

        return new Promise((resolve, reject) => {

            let formData = new FormData();
            let xhr = new XMLHttpRequest();

            for (let i = 0; i <= archivos.length - 1; i++) {
                let add = previewImgs.find(x => x.name == archivos[i].name)
                if (add) {
                    formData.append(codProducto, archivos[i], archivos[i].name);
                }                
            }

            xhr.onreadystatechange = function () {

                if (xhr.readyState === 4) {

                    if (xhr.status === 200) {
                        resolve();
                    } else {
                        reject(xhr.response);
                    }

                }
            };

            xhr.open('POST', this.utils.ServerWithApiUrl + 'fileUpload/uploadProducto/' + codProducto + '/' + isNew, true);
            xhr.send(formData);

        });
    }

}
