import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cliente, Cargos, ContactoCliente } from '../models/clientes.model';
import { EnviaCobranza } from '../models/enviacobranza.model';
import { Paginator } from '../models/paginator.model';
import { Utils } from '../../shared/utils'
import { Usuarios } from '../models/security.model';
import { Documento } from '../models/documentos.model';
import { ResumenContable } from '../models/resumencontable.model';

@Injectable({
  providedIn: 'root'
})
export class ClientesService {

  private apiUrl: string = '';
  private apiUrlClientesExcluidos: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'ClientesPortal';
    this.apiUrlClientesExcluidos = this.utils.ServerWithApiUrl + 'clientesExcluidos';
  }

  //Todo lo nuevo para el portal
  enviaAcceso (data: any): Observable<any> {
    const body = JSON.stringify(data);
    return this.http.post<any>(`${this.apiUrl}/SendAccesosCliente`, body, this.utils.getHeaders(true));
  }

  canActivateAccount (usuario: any): Observable<any> {
    const body = JSON.stringify(usuario);
    return this.http.post<any>(`${this.apiUrl}/CanActivateAccount`, body, this.utils.getHeaders(false));
  }

  activateAccount (usuario: any): Observable<Usuarios> {
    const body = JSON.stringify(usuario);
    return this.http.post<Usuarios>(`${this.apiUrl}/ActivateAccount`, body, this.utils.getHeaders(false));
  }

  getClienteByMailAndRut (cliente: any): Observable<any> {
    const body = JSON.stringify(cliente);
    return this.http.post<any>(`${this.apiUrl}/GetClienteByMailAndRut`, body, this.utils.getHeaders(true));
  }


  enviaCorreoComprobante (cliente: any): Observable<any> {
    const body = JSON.stringify(cliente);
    return this.http.post<any>(`${this.apiUrl}/enviaCorreoComprobante`, body, this.utils.getHeaders(true));
  }

  actualizaClienteSoftland(cliente: any): Observable<any> {
    const body = JSON.stringify(cliente);
    return this.http.post<Cliente[]>(`${this.apiUrl}/ActualizaClienteSoftland`, body, this.utils.getHeaders(true));
  }

  changePassword (cliente: Cliente): Observable<Cliente> {
    const body = JSON.stringify(cliente);
    return this.http.post<Cliente>(`${this.apiUrl}/ChangePassword`, body, this.utils.getHeaders(true));
  }

  getDashboardCompras(codAux: string) {
    return this.http.get(`${this.apiUrl}/GetDashboardCompras/${codAux}`, this.utils.getHeaders(true));
  }

  getClienteComprasFromSoftland (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getClienteComprasFromSoftland`, body, this.utils.getHeaders(true));
  }

  getClienteGuiasDespacho (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getClienteGuiasDespacho`, body, this.utils.getHeaders(true));
  }

  getDetalleCompra (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetDetalleCompra`, body, this.utils.getHeaders(true));
  }

  getDetallePago (data: any) :Observable<any> {
    const body = JSON.stringify(data);
    return this.http.post<any>(`${this.apiUrl}/getDetallePago`, body, this.utils.getHeaders(true));
  }

  getDatosPagoRapido (data: any) :Observable<any> {
    const body = JSON.stringify(data);
    return this.http.post<any>(`${this.apiUrl}/getDatosPagoRapido`, body, this.utils.getHeaders(true));
  }

  getClienteDocumento (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getClienteDocumento`, body, this.utils.getHeaders(true));
  }

  getClienteXML (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getDocumentoXML`, body, this.utils.getHeaders(true));
  }

  getEliminaDocumentoCliente (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getEliminaDocumentoCliente`, body, this.utils.getHeaders(true));
  }

  getDespachoDocumeto (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getDetalleDespacho`, body, this.utils.getHeaders(true));
  }

  enviaDocumentos (data: any): Observable<any> {
    const body = JSON.stringify(data);
    return this.http.post<any>(`${this.apiUrl}/envioDocumentos`, body, this.utils.getHeaders(true));
  }

  getContactosClienteSoftland (codAux: string) {
    return this.http.get(`${this.apiUrl}/getContactosClienteSoftland/${codAux}`, this.utils.getHeaders(true));
  }

  getNotasVentaCliente(codAux: string) {
    return this.http.get(`${this.apiUrl}/GetNotaVentaCliente/${codAux}`, this.utils.getHeaders(true));
  }

  getDetalleCompraNv (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getDetalleCompraNv`, body, this.utils.getHeaders(true));
  }

  getProductosCliente (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getProductosComprados`, body, this.utils.getHeaders(true));
  }

  getClienteEstadoComprasFromSoftland(data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetClienteEstadoComprasFromSoftland`, body, this.utils.getHeaders(true));
  }


  getClientesByCodAux(data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/GetClientesByCodAux`, body, this.utils.getHeaders(false));
  }


  getEstadoConexionSoftland() {
    return this.http.get(`${this.apiUrl}/GetEstadoConexionSoftland`,this.utils.getHeaders(true));
  }

  postSavePago (data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/SavePago`, body, this.utils.getHeaders(true));
  }


  getPDFPago(numComprobante: string) {
    return this.http.get(`${this.apiUrl}/GetPDFPago/${numComprobante}`,this.utils.getHeadersType(false));
  }

  postRecuperarContrasena (data: any) {
    
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/RecuperarContrasena`, body, this.utils.getHeaders(false));
  }

  getTopDeudores() {
    return this.http.get(`${this.apiUrl}/GetTopDeudores`, this.utils.getHeaders(true));
  }

  getDeudaVsPagos() {
    return this.http.get(`${this.apiUrl}/GetDeudaVsPagos`, this.utils.getHeaders(true));
  }





  //Todo lo antiguo
  getCliente(idCliente: number): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.apiUrl}/${idCliente}`);
  }

  getClientes(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.apiUrl);
  }

  getCargos(): Observable<Cargos[]> {
    return this.http.get<Cargos[]>(`${this.apiUrl}/getCargos`, this.utils.getHeaders(true));
  }

  save (cliente: Cliente): Observable<Cliente> {
    const body = JSON.stringify(cliente);
    return this.http.post<Cliente>(this.apiUrl, body, this.utils.getHeaders(false));
  }

  saveContacto (contactoCliente: ContactoCliente): Observable<ContactoCliente> {
    const body = JSON.stringify(contactoCliente);
    return this.http.post<ContactoCliente>(`${this.apiUrl}/postContactoCliente`, body, this.utils.getHeaders(false));
  }

  edit (cliente: Cliente): Observable<Cliente> {
    const body = JSON.stringify(cliente);
    return this.http.put<Cliente>(`${this.apiUrl}/${cliente.idCliente}`, body, this.utils.getHeaders(false));
  }

  delete (idCliente: number) {
    return this.http.delete(`${this.apiUrl}/${idCliente}`, this.utils.getHeaders(false))
  }

  getClientesByPage(paginador: Paginator): Observable<Cliente[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<Cliente[]>(`${this.apiUrl}/GetClientesByPage`, body, this.utils.getHeaders(false));
  }

  

  changeEmail (cliente: Cliente): Observable<Cliente> {
    const body = JSON.stringify(cliente);
    return this.http.put<Cliente>(`${this.apiUrl}/ChangeEmail`, body, this.utils.getHeaders(true));
  }



  getClientFromSoftland(data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/getClientFromSoftland2`, body, this.utils.getHeaders(true));
  }

  getSII(model: any) {
    const body = JSON.stringify(model);
    return this.http.post(`${this.apiUrl}/getDatosSIILibreDTE`, body, this.utils.getHeaders(true));
  }

  

 




 

 

  newClientePortal (data: Cliente) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/newClientePortal`, body, this.utils.getHeaders(false));
  }

  enviaCobranza (data: EnviaCobranza): Observable<EnviaCobranza> {
    const body = JSON.stringify(data);
    return this.http.post<EnviaCobranza>(`${this.utils.ServerWithApiUrl}/enviaCobranzas`, body, this.utils.getHeaders(true));
  }

  getCobranza (id: number): Observable<EnviaCobranza> {
    return this.http.get<EnviaCobranza>(`${this.utils.ServerWithApiUrl}/enviaCobranzas/${id}`, this.utils.getHeaders(true));
  }

 

  desbloquearClienteSoftland(data: any) {
    const body = JSON.stringify(data);
    return this.http.post(`${this.apiUrl}/desbloquearClienteSoftland`, body, this.utils.getHeaders(true));
  }

  
 

  getTopCompras(codAux: string) {
    return this.http.get(`${this.apiUrl}/GetUltimasCompras/${codAux}`, this.utils.getHeaders(true));
  }

  getDashboardDocumentos(codAux: string) {
    return this.http.get(`${this.apiUrl}/GetDashboardDocumentosVencidos/${codAux}`, this.utils.getHeaders(true));
  }

  getDocumentosVencidos(codAux: string): Observable<Documento[]> {
    return this.http.get<Documento[]>(`${this.apiUrl}/GetDocumentosVencidos/${codAux}`, this.utils.getHeaders(true));
  }

  getDocumentosPorVencer(codAux: string): Observable<Documento[]> {
    return this.http.get<Documento[]>(`${this.apiUrl}/GetDocumentosPorVencer/${codAux}`, this.utils.getHeaders(true));
  }

  getDocumentosPendientes(codAux: string): Observable<Documento[]> {
    return this.http.get<Documento[]>(`${this.apiUrl}/GetDocumentosPendientes/${codAux}`, this.utils.getHeaders(true));
  }

  
  getAllDocumentosContabilizados(codAux: string): Observable<Documento[]> {
    return this.http.get<Documento[]>(`${this.apiUrl}/getAllDocumentosContabilizados/${codAux}`, this.utils.getHeaders(true));
  }

  getEstadoBloqueoCliente(codAux: string) {
    return this.http.get(`${this.apiUrl}/GetEstadoBloqueoCliente/${codAux}`, this.utils.getHeaders(true));
  }


  getDashboardAdministrador(data: any): Observable<any> {
    const body = JSON.stringify(data);
    return this.http.post<any>(`${this.apiUrl}/GetDashboardAdministrador`, body, this.utils.getHeaders(true));
  }

  getExistCompras(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetExistCompras`, this.utils.getHeaders(true));
  }


  getDocumentosDashboardAdministrador(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/GetDocumentosDashboarddAdmin`, this.utils.getHeaders(true));
  }
  
  getDocumentosAdministrador(codaux: string, estado: number): Observable<any> {
    debugger
    return this.http.get<any>(`${this.apiUrl}/GetDocumentosAdmin/${codaux}/${estado}`, this.utils.getHeaders(true));
  }


  getDocumentosPagadosAdministrador(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/GetDocumentosPagados`, null, this.utils.getHeaders(true));
  }

 //FCA 05-07-2022
 getdocumentoPDF(cabecera : any): Observable<any> {
  const body = JSON.stringify(cabecera);
  return this.http.post<any>(`${this.apiUrl}/GetDocumentoPDF`, body, this.utils.getHeaders(true));
}


enviaDocumentoPDF(envio : any): Observable<any> {
  const body = JSON.stringify(envio);
  return this.http.post<any>(`${this.apiUrl}/enviaDocumentoPDF`, body, this.utils.getHeaders(true));
}

getResumenContable(codAux: string): Observable<ResumenContable> {
  return this.http.get<ResumenContable>(`${this.apiUrl}/getResumenContable/${codAux}`, this.utils.getHeaders(true));
}


//FCA 05-07-2022
getdocumentoPDFNv(cabecera : any): Observable<any> {
  const body = JSON.stringify(cabecera);
  return this.http.post(`${this.apiUrl}/GetDocumentoPDFNv`, body, this.utils.getHeadersType(true));
}

getClientesExcluidos(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrlClientesExcluidos}`, this.utils.getHeaders(true));
}

deleteExcluido (cliente: any): Observable<any> {
  return this.http.delete<any>(`${this.apiUrlClientesExcluidos}/${cliente.idExcluido}`, this.utils.getHeaders(true));
}


getClientesFiltros(data: any) {
  const body = JSON.stringify(data);
  return this.http.post(`${this.apiUrl}/GetClientesFiltro`, body, this.utils.getHeaders(true));
}

saveExcluido (cliente: any): Observable<any> {
  const body = JSON.stringify(cliente);
  return this.http.post<any>(this.apiUrlClientesExcluidos, body, this.utils.getHeaders(true));
}


getCorreosDTE(rut: string): Observable<any[]> {
  const body = JSON.stringify(rut);
  return this.http.get<any[]>(`${this.apiUrl}/GetCorreosDTE/${rut}`,this.utils.getHeaders(true));
}
}
