import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UnidadMedida } from '../models/unidadmedida.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class UnidadMedidaService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'unidadmedidas';
  }

  getUnidadesMedida(): Observable<UnidadMedida[]> {
    return this.http.get<UnidadMedida[]>(this.apiUrl);
  }

  save (undMedida: UnidadMedida): Observable<UnidadMedida> {
    const body = JSON.stringify(undMedida);
    return this.http.post<UnidadMedida>(this.apiUrl, body, this.utils.getHeaders(false));
  }

  edit (undMedida: UnidadMedida): Observable<UnidadMedida> {
    const body = JSON.stringify(undMedida);
    return this.http.put<UnidadMedida>(`${this.apiUrl}/${undMedida.idUMed}`, body, this.utils.getHeaders(false));
  }

  delete (idUMed: number) {
    return this.http.delete(`${this.apiUrl}/${idUMed}`, this.utils.getHeaders(false))
  }

  getUnidadesMedidaByPage(paginador: Paginator): Observable<UnidadMedida[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<UnidadMedida[]>(`${this.apiUrl}/GetUnidadesMedidaByPage`, body, this.utils.getHeaders(false));
  }

}
