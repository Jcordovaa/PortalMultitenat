import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfiguracionSoftland } from '../models/configuracion.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class ConfiguracionSoftlandService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) {
    this.apiUrl = this.utils.ServerWithApiUrl + 'Softland';
  }

  getConfig() {
    return this.http.get(`${this.apiUrl}/getConfiguracionSoftlandData`);
  }

  edit(data: ConfiguracionSoftland) {
    const body = JSON.stringify(data);
    return this.http.put(`${this.apiUrl}/${data.idConfiguracionSoftland}`, body, this.utils.getHeaders(true));
  }

  get() {
    return this.http.get(`${this.apiUrl}`, this.utils.getHeaders(true));
  }

  getAllTipoDocSoftland() {
    return this.http.get(`${this.apiUrl}/getAllTipoDocSoftland`, this.utils.getHeaders(true));
  }

  getAllCuentasContablesSoftland() {
    return this.http.get(`${this.apiUrl}/getAllCuentasContablesSoftland`, this.utils.getHeaders(true));
  }

  getClientesAccesos(filter: any): Observable<any[]> {
    const body = JSON.stringify(filter);
    return this.http.post<any[]>(`${this.apiUrl}/GetClientesAcceso`, body, this.utils.getHeaders(true));
  }

  getGirosSoftland(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetGiros`, this.utils.getHeaders(true));
  }

  getUbicaciones(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetUbicaciones`, this.utils.getHeaders(true));
  }

  getVendedores(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetVendedores`, this.utils.getHeaders(true));
  }

  getCatClientes(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetCategoriasCliente`, this.utils.getHeaders(true));
  }

  getMonedas(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetMonedas`, this.utils.getHeaders(true));
  }

  getCondVentas(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetCondVentas`, this.utils.getHeaders(true));
  }

  getListasPrecio(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetListasPrecio`, this.utils.getHeaders(true));
  }

  getClientesAccesos2(filter: any): Observable<any[]> {
    const body = JSON.stringify(filter);
    return this.http.post<any[]>(`${this.apiUrl}/GetClientesAcces`, body, this.utils.getHeaders(true));
  }

  //FCA 09-08-2022 OBTIENE CARGOS
  getCargos(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetCargos`, this.utils.getHeaders(true));
  }

   //FCA 09-08-2022 OBTIENE CENTROS DE COSTOS ACTIVOS
   getCentrosCostosActivos(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetCentrosCostosActivos`, this.utils.getHeaders(true));
  }

   //FCA 09-08-2022 OBTIENE AREAS DE NEGOCIO
   getAreasNegocio(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetAreasNegocio`, this.utils.getHeaders(true));
  }


  generaComprobantePago (idPago: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/ReprocesarPago/${idPago}`, null, this.utils.getHeaders(true));
  }


  actualizaComprobantePago (pago: any): Observable<any> {
    const body = JSON.stringify(pago);
    return this.http.post<any>(`${this.apiUrl}/ActualizaComprobante`, body, this.utils.getHeaders(true));
  }


}
