import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TipoPago } from '../models/tipopago.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class TipoPagoService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'TipoPagos';
  }

  getAll(): Observable<TipoPago[]> {
    return this.http.get<TipoPago[]>(`${this.apiUrl}`);
  }

}
