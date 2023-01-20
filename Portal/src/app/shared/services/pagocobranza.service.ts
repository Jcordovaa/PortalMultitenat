import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Marcas } from '../models/marcas.model';
import { PagoCobranza } from '../models/pagos.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class PagosCobranzaService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'PagosCobranza';
  }

  save (pagos: PagoCobranza): Observable<PagoCobranza> {
    const body = JSON.stringify(pagos);
    return this.http.post<PagoCobranza>(`${this.apiUrl}/savePagoCobranza`, body, this.utils.getHeaders(true));
  }

}
