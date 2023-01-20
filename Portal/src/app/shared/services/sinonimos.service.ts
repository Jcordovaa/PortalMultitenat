import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductoSinonimos } from '../models/productos.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class SinonimosService {

    private apiUrl: string = '';
    private apiUrlFile: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'productoSinonimos';
        this.apiUrlFile = this.utils.ServerWithApiUrl + 'fileUpload';
    }

    getSinonimo(idSinonimo: number): Observable<ProductoSinonimos> {
        return this.http.get<ProductoSinonimos>(`${this.apiUrl}/${idSinonimo}`);
    }

    getSinonimos(): Observable<ProductoSinonimos[]> {
        return this.http.get<ProductoSinonimos[]>(this.apiUrl);
    }

    save(cupon: ProductoSinonimos): Observable<ProductoSinonimos> {
        const body = JSON.stringify(cupon);
        return this.http.post<ProductoSinonimos>(this.apiUrl, body, this.utils.getHeaders(false));
    }

    edit(sinonimo: ProductoSinonimos): Observable<ProductoSinonimos> {
        const body = JSON.stringify(sinonimo);
        return this.http.put<ProductoSinonimos>(`${this.apiUrl}/${sinonimo.idSinonimo}`, body, this.utils.getHeaders(false));
    }

    delete(idSinonimo: number) {
        return this.http.delete(`${this.apiUrl}/${idSinonimo}`, this.utils.getHeaders(false))
    }

    getSinonimosByPage(paginador: Paginator): Observable<ProductoSinonimos[]> {
        const body = JSON.stringify(paginador);
        return this.http.post<ProductoSinonimos[]>(`${this.apiUrl}/GetSinonimosByPage`, body, this.utils.getHeaders(false));
    }

    procesarExcel(archivos: FileList) {

        return new Promise((resolve, reject) => {
    
            let formData = new FormData();
            let xhr = new XMLHttpRequest();
    
            for (let i = 0; i <= archivos.length - 1; i++) {
                formData.append(i.toString(), archivos[i], archivos[i].name);
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
    
            xhr.open('POST', `${this.apiUrlFile}/importExcelSinonimos`, true);
            xhr.send(formData);
    
        });
    }

}
