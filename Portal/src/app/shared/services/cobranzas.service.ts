import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utils } from '../utils'

@Injectable({
  providedIn: 'root'
})
export class CobranzasService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'Cobranza';
  }

  getDocumentosPendientes(filter: any) {
    const body = JSON.stringify(filter);
    return this.http.post(`${this.apiUrl}/GetDocumentosPendientes`, body, this.utils.getHeaders(true));
  }

  getAniosPagos() {
    return this.http.get(`${this.apiUrl}/GetAnioPagos`, this.utils.getHeaders(true));
  }

  getHorasEnvio() {
    return this.http.get(`${this.apiUrl}/GetHorariosEnvio`, this.utils.getHeaders(true));
  }

  getExcelDocumentosPendientes(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetExcelDocumentosPendientes`, body, this.utils.getHeaders(true));
  }

  getCantidadDocumentos(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetCantidadDocumentosCobranza`, body, this.utils.getHeaders(true));
  }

  getDocumentosFiltros(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetDocumentosCobranzaFiltro`, body, this.utils.getHeaders(true));
  }

  deleteCobranza(id: any) {   
    return this.http.delete(`${this.apiUrl}/DeleteCobranza/${id}`, this.utils.getHeaders(true));
  }

  getDocumentosClientes(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetDocumentosClientes`, body, this.utils.getHeaders(true));
  }

  getDocumentosPorCliente(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetDocumentosPorCliente`, body, this.utils.getHeaders(true));
  }

  getTiposCobranza() {
    return this.http.get(`${this.apiUrl}/GetTipoCobranza`, this.utils.getHeaders(true));
  }

  saveCobranza(data: any) {   
    debugger
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/SaveCobranza`, body, this.utils.getHeaders(true));
  }

  enviaCobranza() {
    return this.http.get(`${this.apiUrl}/EnviaCobranza`, this.utils.getHeaders(true));
  }

  getCobranzaCliente(data: any) {   
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetCobranzaCliente`, body, this.utils.getHeaders(true));
  }

  getEstadoCobranza() {
    return this.http.get(`${this.apiUrl}/GetEstadosCobranza`,  this.utils.getHeaders(true));
  }

  getCobranzasTipo(filter: any) {
    const body = JSON.stringify(filter);
    return this.http.post(`${this.apiUrl}/GetCobranzasTipo`, body, this.utils.getHeaders(true));
  }

  getCobranzaGraficos(id: any) {
    return this.http.get(`${this.apiUrl}/GetCobranzaGraficos/${id}`, this.utils.getHeaders(true));
  }

  getCobranzasDetalle(filter: any) {
    const body = JSON.stringify(filter);
    return this.http.post(`${this.apiUrl}/GetCobranzasDetalle`, body, this.utils.getHeaders(true));
  }

  getCobranzaPeriocidad() {
    return this.http.get(`${this.apiUrl}/GetCobranzaPeriocidad`, this.utils.getHeaders(true));
  }

  

  //FCA 13-12-2021
  modificarEstado(idCobranza: number, estado : number) {
    return this.http.post(`${this.apiUrl}/ModificarEstadoCobranzaInteligente/${idCobranza}/${estado}`,null, this.utils.getHeaders(true));
  }

  getTipoDocumentosPagos() {
    return this.http.get(`${this.apiUrl}/GetTiposDocumentosPago`, this.utils.getHeaders(true));
  }
}