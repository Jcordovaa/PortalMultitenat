import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Banners, TipoBanners } from '../models/banners.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class BannersService {

  private apiUrl: string = '';
  private apiUrlTipo: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'banners';
    this.apiUrlTipo = this.utils.ServerWithApiUrl + 'tipoBanners';
  }

  getBanner(idBanner: number): Observable<Banners> {
    return this.http.get<Banners>(`${this.apiUrl}/${idBanner}`);
  }

  getBanners(): Observable<Banners[]> {
    return this.http.get<Banners[]>(this.apiUrl);
  }

  getTipoBanners(): Observable<TipoBanners[]> {
    return this.http.get<TipoBanners[]>(this.apiUrlTipo);
  }

  save (banner: Banners): Observable<Banners> {
    const body = JSON.stringify(banner);
    return this.http.post<Banners>(this.apiUrl, body, this.utils.getHeaders(false));
  }

  edit (banner: Banners): Observable<Banners> {
    const body = JSON.stringify(banner);
    return this.http.put<Banners>(`${this.apiUrl}/${banner.idBanner}`, body, this.utils.getHeaders(false));
  }

  delete (idBanner: number) {
    return this.http.delete(`${this.apiUrl}/${idBanner}`, this.utils.getHeaders(false))
  }

  getBannersByPage(paginador: Paginator): Observable<Banners[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Banners[]>(`${this.apiUrl}/GetBannerByPage`, body, this.utils.getHeaders(false));
}

  subirImagen(archivos: FileList, idReceta: number) {

    return new Promise((resolve, reject) => {

        let formData = new FormData();
        let xhr = new XMLHttpRequest();

        for (let i = 0; i <= archivos.length - 1; i++) {
            formData.append(idReceta.toString(), archivos[i], archivos[i].name);
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

        xhr.open('POST', this.utils.ServerWithApiUrl + 'fileUpload/uploadBanner/' + idReceta, true);
        xhr.send(formData);

    });
}

}
