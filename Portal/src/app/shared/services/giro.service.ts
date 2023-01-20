import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Giro } from '../models/clientes.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class GirosService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'giros';
  }

  getGiros(): Observable<Giro[]> {
    return this.http.get<Giro[]>(`${this.apiUrl}`);
  }

  getGirosSoftland(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetGirosSoftland`);
  }

}
