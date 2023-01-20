import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Suscripciones } from '../models/suscripciones.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class SuscriptoresService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'suscripciones';
  }

  getSuscriptor(idCliente: number): Observable<Suscripciones> {
    return this.http.get<Suscripciones>(`${this.apiUrl}/${idCliente}`);
  }

  getSuscriptores(): Observable<Suscripciones[]> {
    return this.http.get<Suscripciones[]>(this.apiUrl);
  }

  getSuscripcionesByPage(paginador: Paginator): Observable<Suscripciones[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Suscripciones[]>(`${this.apiUrl}/GetSuscripcionesByPage`, body, this.utils.getHeaders(false));
  }

}
