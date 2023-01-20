import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bancos } from '../models/bancos.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class BancosService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'bancos';
  }

  getAll(): Observable<Bancos[]> {
    return this.http.get<Bancos[]>(this.apiUrl);
  }

}
