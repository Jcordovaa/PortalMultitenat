import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utils } from '../../shared/utils'
import { PasarelaPago } from '../models/pasarelapago.model';

@Injectable({
  providedIn: 'root'
})
export class PasarelaPagoService {

  private apiUrl: string = '';
  private apiPago: string = ''

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'PasarelaPago';
    this.apiPago = this.utils.ServerWithApiUrl + 'ProcesaPagos';
  }

  getPasarelasPago(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
  
  getAllPasarelasPago(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetAllPasarelas`,this.utils.getHeaders(true));
  }
  
  // edit (pasarela: PasarelaPago): Observable<PasarelaPago> {
  //   const body = JSON.stringify(pasarela);
  //   return this.http.put<PasarelaPago>(`${this.apiUrl}/${pasarela.idPasarela}`, body, this.utils.getHeaders(false));
  // }

  edit (pasarela: PasarelaPago): Observable<PasarelaPago> {
    const body = JSON.stringify(pasarela);
    return this.http.post<PasarelaPago>(`${this.apiUrl}/actualizaPasarelas`, body, this.utils.getHeaders(true));
  }
  
  getLogPasarela(idPago: number) {
    return this.http.get(`${this.apiUrl}/getLog/${idPago}`,this.utils.getHeaders(false));
  }

  generaPagoElectronico(idPago:number, idPasarela:number, rutCliente:string, idCobranza:number, datosPago:string, redirectTo: number ): Observable<PasarelaPago> {
    return this.http.post<PasarelaPago>(`${this.apiPago}/GeneraPagoElectronico?idPago=${idPago}&idPasarela=${idPasarela}&rutCliente=${rutCliente}&idCobranza=${idCobranza}&datosPago=${datosPago}&redirectTo=${redirectTo}`, this.utils.getHeaders(true));
  }

}
