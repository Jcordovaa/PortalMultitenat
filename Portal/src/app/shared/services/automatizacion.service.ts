import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cliente, Cargos, ContactoCliente } from '../models/clientes.model';
import { EnviaCobranza } from '../models/enviacobranza.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'
import { Usuarios } from '../models/security.model';
import { Documento } from '../models/documentos.model';
import { ResumenContable } from '../models/resumencontable.model';
import { Automatizacion, TipoAutomatizacion } from '../models/automatizacion.model';

@Injectable({
  providedIn: 'root'
})
export class AutomatizacionService {

  private apiUrl: string = '';
  private apiTipos: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'Automatizacion';
    this.apiTipos = this.utils.ServerWithApiUrl + 'tipoAutomatizacion';
  }

 
  //Obtiene tipos de automatización
  getTipoAutomatizaciones(): Observable<TipoAutomatizacion[]> {
    return this.http.get<TipoAutomatizacion[]>(`${this.apiTipos}/getTipos`, this.utils.getHeaders(true));
  }

  
  save (aut: Automatizacion): Observable<any> {
    const body = JSON.stringify(aut);
    return this.http.post<any>(`${this.apiUrl}/GuardaAutomatizacion`, body, this.utils.getHeaders(true));
  }


  edit (aut: Automatizacion): Observable<any> {
    const body = JSON.stringify(aut);
    return this.http.put<Cliente>(`${this.apiUrl}/${aut.idAutomatizacion}`, body, this.utils.getHeaders(true));
  }

  delete (idAutomatizacion: number) {
    return this.http.delete(`${this.apiUrl}/${idAutomatizacion}`, this.utils.getHeaders(true))
  }

  getAutomatizaciones(): Observable<Automatizacion[]> {
    return this.http.get<Automatizacion[]>(`${this.apiUrl}/GetAutomatizaciones`, this.utils.getHeaders(true));
  }


  enviaAutomatizaciones() {
    return this.http.get(`${this.apiUrl}/EnviaAutomatizaciones`, this.utils.getHeaders(true));
  }

}
