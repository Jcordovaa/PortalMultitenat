import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Mail } from '../models/mail.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class MailService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'mail';
  }

  send (mail: Mail): Observable<Mail> {
    const body = JSON.stringify(mail);
    return this.http.post<Mail>(this.apiUrl, body, this.utils.getHeaders(false));
  }


  getCorreosDisponibles() {
    return this.http.get(`${this.apiUrl}/GetCorreosDisponiblesCobranza`, this.utils.getHeaders(true));
  }

}
