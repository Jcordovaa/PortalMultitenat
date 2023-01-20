import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Mail } from '../models/mail.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class MailOldService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'mail';
  }

  send (mail: any) {
    const body = JSON.stringify(mail);
    return this.http.post(this.apiUrl, body, this.utils.getHeaders(false));
  }

}
