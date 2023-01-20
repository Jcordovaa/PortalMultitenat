import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utils } from '../../shared/utils'

@Injectable({
    providedIn: 'root'
})
export class CondicionVentaService {

    private apiUrl: string = '';

    constructor(private utils: Utils, private http: HttpClient) {
        this.apiUrl = this.utils.ServerWithApiUrl + 'CondicionDeVentas';
    }

    getAll() {
        return this.http.get(`${this.apiUrl}`, this.utils.getHeaders(true));
    }

}
