import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ubicaciones } from '../models/ubicaciones.model';
import { Utils } from '../../shared/utils';

@Injectable({
    providedIn: 'root'
  })
export class Ubicacioneservice {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'ubicaciones';
  }

  getUbicaciones(): Observable<Ubicaciones> {
    return this.http.get<Ubicaciones>(`${this.apiUrl}/GetUbicaciones`);
  }
  getUbicacionesSoftland(): Observable<Ubicaciones> {
    return this.http.get<Ubicaciones>(`${this.apiUrl}/GetUbicacionesSoftland`);
  }

}
