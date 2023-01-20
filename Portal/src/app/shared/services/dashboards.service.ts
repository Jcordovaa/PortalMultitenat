import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardEcommerce, DashboardVentas } from '../models/dashboards.model';
import { Utils } from '../../shared/utils'

@Injectable({
  providedIn: 'root'
})
export class DashboardsService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) { 
    this.apiUrl = this.utils.ServerWithApiUrl + 'dashboards';
  }

  getDashboardEcommerce (dash: DashboardEcommerce): Observable<DashboardEcommerce> {
    const body = JSON.stringify(dash);
    return this.http.post<DashboardEcommerce>(`${this.apiUrl}/GetDashboardEcommerce`, body, this.utils.getHeaders(true));
  }

  getDashboardVentas (dash: DashboardVentas): Observable<DashboardVentas> {
    const body = JSON.stringify(dash);
    return this.http.post<DashboardVentas>(`${this.apiUrl}/GetDashboardVentas`, body, this.utils.getHeaders(true));
  }

}
