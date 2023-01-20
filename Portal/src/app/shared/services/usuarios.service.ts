import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Venta, VentaDetalle } from '../models/ventas.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'
import { Usuario } from '../models/usuarios.model';

@Injectable({
  providedIn: 'root'
})
export class UsuariosService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) {
    this.apiUrl = this.utils.ServerWithApiUrl + 'Usuarios';
  }

  getUsuarioByMail(cliente: any): Observable<any> {
    const body = JSON.stringify(cliente);
    return this.http.post<any>(`${this.apiUrl}/GetUsuarioByMail`, body, this.utils.getHeaders(true));
  }

  getEmpresa(): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/GetEmpresa`,this.utils.getHeaders(true));
  }

  changePassword(usuario: Usuario): Observable<Usuario> {
    const body = JSON.stringify(usuario);
    return this.http.post<Usuario>(`${this.apiUrl}/changePassword`, body, this.utils.getHeaders(true));
  }

  changeCorreo(usuario: Usuario): Observable<Usuario> {
    const body = JSON.stringify(usuario);
    return this.http.post<Usuario>(`${this.apiUrl}/ChangeCorreo`, body, this.utils.getHeaders(true));
  }

}
