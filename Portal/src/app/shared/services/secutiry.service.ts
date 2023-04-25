import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Perfil, Acceso, Permisos, Usuarios } from '../models/security.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'


@Injectable({
  providedIn: 'root'
})
export class SecurityService {

  private apiUrlPerfiles: string = '';
  private apiUrlAccesos: string = '';
  private apiUrlPermisos: string = '';
  private apiUrlUsuarios: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrlPerfiles = this.utils.ServerWithApiUrl + 'perfiles';
    this.apiUrlAccesos = this.utils.ServerWithApiUrl + 'accesos';
    this.apiUrlPermisos = this.utils.ServerWithApiUrl + 'permisos';
    this.apiUrlUsuarios = this.utils.ServerWithApiUrl + 'usuarios';
    
  }

  //ACCESOS
  getAccesos(): Observable<Acceso[]> {
    return this.http.get<Acceso[]>(`${this.apiUrlAccesos}`, this.utils.getHeaders(true));
  }

  getAcceso(idAcceso: number): Observable<Acceso> {
    return this.http.get<Acceso>(`${this.apiUrlAccesos}/${idAcceso}`, this.utils.getHeaders(true));
  }

  saveAcceso (acceso: Acceso): Observable<Acceso> {
    const body = JSON.stringify(acceso);
    return this.http.post<Acceso>(this.apiUrlAccesos, body, this.utils.getHeaders(true));
  }

  editAcceso (acceso: Acceso): Observable<Acceso> {
    const body = JSON.stringify(acceso);
    return this.http.put<Acceso>(`${this.apiUrlAccesos}/${acceso.idAcceso}`, body, this.utils.getHeaders(true));
  }

  deleteAcceso (idAcceso: number) {
    return this.http.delete(`${this.apiUrlAccesos}/${idAcceso}`, this.utils.getHeaders(true))
  }

  getAccesosByPage(paginador: Paginator): Observable<Acceso[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Acceso[]>(`${this.apiUrlAccesos}/GetAccesosByPage`, body, this.utils.getHeaders(true));
  }

  //PERFILES
  getPerfil(idPerfil: number): Observable<Perfil> {
    return this.http.get<Perfil>(`${this.apiUrlPerfiles}/getPerfilId/${idPerfil}`, this.utils.getHeaders(true));
  }

  getPerfiles(): Observable<Perfil[]> {
    return this.http.get<Perfil[]>(`${this.apiUrlPerfiles}/getPerfiles`, this.utils.getHeaders(true));
  }


  savePerfil (perfil: Perfil): Observable<Perfil> {
    const body = JSON.stringify(perfil);
    return this.http.post<Perfil>(`${this.apiUrlPerfiles}/GuardarPerfil`, body, this.utils.getHeaders(true));
  }

  editPerfil (perfil: Perfil): Observable<Perfil> {
    const body = JSON.stringify(perfil);
    return this.http.put<Perfil>(`${this.apiUrlPerfiles}/ActualizaPerfil`, body, this.utils.getHeaders(true));
  }

  deletePerfil(idPerfil: number) {
    return this.http.delete(`${this.apiUrlPerfiles}/EliminaPerfilId/${idPerfil}`, this.utils.getHeaders(true))
  }

  getPerfilesByPage(paginador: Paginator): Observable<Perfil[]> {
      const body = JSON.stringify(paginador);
    return this.http.post<Perfil[]>(`${this.apiUrlPerfiles}/GetPerfilesByPage`, body, this.utils.getHeaders(true)); 
  }

  //PERMISOS
  getPermiso(idPermiso: number): Observable<Permisos> {
    return this.http.get<Permisos>(`${this.apiUrlPermisos}/${idPermiso}`, this.utils.getHeaders(true));
  }

  getPermisosByEmail(data: any): Observable<Permisos[]> {
    const body = JSON.stringify(data);
    return this.http.post<Permisos[]>(`${this.apiUrlPermisos}/GetPermisosByEmail/`, body, this.utils.getHeaders(true));
  }

  savePermiso(permiso: Permisos): Observable<Perfil> {
    const body = JSON.stringify(permiso);
    return this.http.post<Permisos>(this.apiUrlPermisos, body, this.utils.getHeaders(true));
  }

  savePermisoMasivo(idPerfil: number, permisos: Permisos[]): Observable<Perfil> {
    const body = JSON.stringify(permisos);
    return this.http.post<Permisos>(`${this.apiUrlPermisos}/PostPermisosMasivo/${idPerfil}`, body, this.utils.getHeaders(true));
  }

  editPermiso (permiso: Permisos): Observable<Perfil> {
    const body = JSON.stringify(permiso);
    return this.http.put<Permisos>(`${this.apiUrlPermisos}/${permiso.idPermiso}`, body, this.utils.getHeaders(true));
  }

  deletePermiso(idPermiso: number) {
    return this.http.delete(`${this.apiUrlPermisos}/${idPermiso}`, this.utils.getHeaders(true))
  }

  getPermisosByPage(paginador: Paginator): Observable<Permisos[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Permisos[]>(`${this.apiUrlPermisos}/GetPermisosByPage`, body, this.utils.getHeaders(true));
  }

  getPermisosByPerfil(idPerfil: number): Observable<Permisos[]> {
    return this.http.get<Permisos[]>(`${this.apiUrlPermisos}/GetPermisosByPerfil/${idPerfil}`, this.utils.getHeaders(true));
  }

  //USUARIOS
  getUsuario (idUsuario: number): Observable<Usuarios> {
    return this.http.get<Usuarios>(`${this.apiUrlUsuarios}/${idUsuario}`);
  }


  getUsuarioId (idUsuario: number): Observable<Usuarios> {
    return this.http.get<Usuarios>(`${this.apiUrlUsuarios}/getUsuarioId/${idUsuario}`, this.utils.getHeaders(true));
  }

  saveUsuario (usuario: Usuarios): Observable<any> {
    const body = JSON.stringify(usuario);
    return this.http.post<any>(`${this.apiUrlUsuarios}/GuardarUsuario`, body, this.utils.getHeaders(true));
  }

  editUsuario (usuario: Usuarios): Observable<any> {
    const body = JSON.stringify(usuario);
    return this.http.put<any>(`${this.apiUrlUsuarios}/ActualizaUsuario`, body, this.utils.getHeaders(true));
  }

  restableceContraseñaUsuario (usuario: Usuarios): Observable<any> {
    const body = JSON.stringify(usuario);
    return this.http.post<any>(`${this.apiUrlUsuarios}/restablecerContraseña`, body, this.utils.getHeaders(true));
  }



  deleteUsuario(idUsuario: number) {
    return this.http.delete(`${this.apiUrlUsuarios}/EliminaUsuarioId/${idUsuario}`, this.utils.getHeaders(true))
  }

  getUsuarioByPage(paginador: Paginator): Observable<Usuarios[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Usuarios[]>(`${this.apiUrlUsuarios}/GetUsuariosByPage`, body, this.utils.getHeaders(true));
  }

  canActivateAccount (usuario: Usuarios): Observable<Usuarios> {
    const body = JSON.stringify(usuario);
    return this.http.post<Usuarios>(`${this.apiUrlUsuarios}/CanActivateAccount`, body, this.utils.getHeaders(false));
  }

  activateAccount (usuario: Usuarios): Observable<Usuarios> {
    const body = JSON.stringify(usuario);
    return this.http.post<Usuarios>(`${this.apiUrlUsuarios}/activateAccount`, body, this.utils.getHeaders(false));
  }

  // changePasswordRequest (usuario: Usuarios): Observable<Usuarios> {
  //   const body = JSON.stringify(usuario);
  //   return this.http.post<Usuarios>(`${this.apiUrlUsuarios}/ChangePasswordRequest`, body, this.utils.getHeaders(false));
  // }
  
}
