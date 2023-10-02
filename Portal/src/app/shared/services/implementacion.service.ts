import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utils } from '../../shared/utils'
import { stringify } from 'querystring';

@Injectable({
  providedIn: 'root'
})
export class ImplementacionService {

  private apiUrl: string = '';

  constructor(private utils: Utils, private http: HttpClient) {
    this.apiUrl = this.utils.ServerWithApiUrl + 'Implementacion';
  }

  getEmpresasByPage(paginador: any): Observable<any[]> {
    const body = JSON.stringify(paginador);
    return this.http.post<any[]>(`${this.apiUrl}/ObtenerEmpresasPorPagina`, body, this.utils.getHeaders(true));
  }


  getImplementadores(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ObtenerImplementadores`, this.utils.getHeaders(true));
  }

  getAreasComerciales(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ObtenerAreasComerciales`, this.utils.getHeaders(true));
  }


  getLineasProductos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ObtenerLineasProducto`, this.utils.getHeaders(true));
  }

  getPlanes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ObtenerPlanes`, this.utils.getHeaders(true));
  }

  crearEmpresa(empresa: any): Observable<any> {
    debugger
    const body = JSON.stringify(empresa);
    return this.http.post<any>(`${this.apiUrl}/CrearEmpresa`, body, this.utils.getHeaders(true));
  }

  editarEmpresa(empresa: any): Observable<any> {
    const body = JSON.stringify(empresa);
    return this.http.post<any>(`${this.apiUrl}/EditarEmpresa`, body, this.utils.getHeaders(true));
  }

  eliminarEmpresa(idEmpresa: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/EliminarEmpresa/${idEmpresa}`, this.utils.getHeaders(true));
  }

  validaConexionApi(apiModel: any): Observable<any> {
    const body = JSON.stringify(apiModel);
    return this.http.post<any>(`${this.apiUrl}/validaConexionApi`, body, this.utils.getHeaders(true));
  }

  validaConexionBd(datosImplementacion: any): Observable<any> {
    const body = JSON.stringify(datosImplementacion);
    return this.http.post<any>(`${this.apiUrl}/validaConexionBaseDatos`, body, this.utils.getHeaders(true));
  }

  obtieneCuentasContables(apiModel: any): Observable<any[]> {
    const body = JSON.stringify(apiModel);
    return this.http.post<any[]>(`${this.apiUrl}/ObtenerCuentasContables`, body, this.utils.getHeaders(true));
  }

  obtieneTiposDocumentos(apiModel: any): Observable<any[]> {
    const body = JSON.stringify(apiModel);
    return this.http.post<any[]>(`${this.apiUrl}/ObtenerTiposDocumento`, body, this.utils.getHeaders(true));
  }

  obtieneCuentasContablesPasarela(apiModel: any): Observable<any[]> {
    const body = JSON.stringify(apiModel);
    return this.http.post<any[]>(`${this.apiUrl}/ObtenerCuentasContablesPasarelas`, body, this.utils.getHeaders(true));
  }

  obtieneDatosTenant(tenant: any): Observable<any[]> {
    const body = JSON.stringify(tenant);
    return this.http.post<any[]>(`${this.apiUrl}/ObtieneDatosTenant`, body, this.utils.getHeaders(true));
  }

  saveTenant(tenant: any): Observable<any> {
    const body = JSON.stringify(tenant);
    const formData = new FormData();

    for (const prop in tenant) {
      if (tenant.hasOwnProperty(prop)) {
        // Verificar si la propiedad es un objeto IFormFile (archivos)
        if (tenant[prop] instanceof File && tenant[prop] !== null) {
          formData.append(this.capitalizeFirstLetter(prop), tenant[prop]);
        } else if (typeof tenant[prop] === 'object' && tenant[prop] !== null) {
          // Si la propiedad es un objeto, recursivamente mapea sus propiedades
          const subObject = tenant[prop];
          for (const subProp in subObject) {
            if (subObject.hasOwnProperty(subProp)) {
              if (subObject[subProp] !== null) {
                const formattedSubProp = this.capitalizeFirstLetter(subProp);

                // Verificar si la propiedad es de tipo Date
                if (subObject[subProp] instanceof Date) {
                  // Aquí puedes manejar la propiedad Date como desees
                  // Por ejemplo, puedes convertirla a una cadena en el formato deseado
                  const formattedDate = subObject[subProp].toISOString(); // O el formato que prefieras
                  formData.append(`${this.capitalizeFirstLetter(prop)}.${this.capitalizeFirstLetter(formattedSubProp)}`, formattedDate);
                } else {
                  // Si no es Date, simplemente agrega la propiedad al FormData
                  formData.append(`${this.capitalizeFirstLetter(prop)}.${this.capitalizeFirstLetter(formattedSubProp)}`, subObject[subProp]);
                }
              }
            }
          }
        } else {
          // Verificar si la propiedad es de tipo Date
          if (tenant[prop] instanceof Date) {
            // Aquí puedes manejar la propiedad Date como desees
            // Por ejemplo, puedes convertirla a una cadena en el formato deseado
            const formattedDate = tenant[prop].toISOString(); // O el formato que prefieras
            formData.append(this.capitalizeFirstLetter(prop), formattedDate);
          } else {
            // Si no es Date, simplemente agrega la propiedad al FormData
            formData.append(this.capitalizeFirstLetter(prop), tenant[prop]);
          }
        }
      }
    }







    // formData.append('IdTenant', tenant.idTenant)
    // formData.append('IdEmpresa', tenant.idEmpresa)
    // formData.append('Identifier', tenant.identifier)
    // formData.append('Dominio', tenant.dominio)
    // formData.append('ConnectionString', tenant.connectionString)
    // formData.append('OtImplementacion', tenant.otImplementacion)
    // formData.append('NombreImplementador', tenant.nombreImplementador)
    // formData.append('TelefonoImplementador', tenant.telefonoImplementador)
    // formData.append('CorreoImplementador', tenant.correoImplementador)
    // formData.append('FechaInicioImplementacion', tenant.fechaInicioImplementacion.toISOString())
    // formData.append('FechaTerminoImplementacion', tenant.fechaTerminoImplementacion.toISOString())
    // formData.append('FechaInicioContrato', tenant.fechaInicioContrato.toISOString())
    // formData.append('FechaTerminoContrato', tenant.fechaTerminoContrato.toISOString())
    // formData.append('IdPlan', tenant.idPlan)
    // formData.append('IdImplementador', tenant.idImplementador)
    // formData.append('IdLineaProducto', tenant.idLineaProducto)


    // idLineaProducto?: number
    // idAreaComercial?: number
    // datosImplementacion?: DatosImplementacion
    // rutEmpresa?: string
    // nombreEmpresa?: string
    // logoCorreo?: File
    // imagenUltimasCompras?: File
    // bannerPortal?: File
    // imagenUsuario?: File
    // iconoContactos?: File
    // bannerMisCompras?: File
    // iconoMisCompras?: File
    // iconoClavePerfil?: File
    // iconoEditarPerfil?: File
    // iconoEstadoPerfil?: File
    // imagenPortada?: File
    // logoPortada?: File
    // bannerPagoRapido?: File
    // logoMinimalistaSidebar?: File
    // logoSidebar?:  File
    return this.http.post<any>(`${this.apiUrl}/CreaActualizaTenant`, body, this.utils.getHeaders(true));
  }


  subirTenantYArchivos(tenant: any) {
    if (!tenant) {
      return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
      let formData = new FormData();
      let xhr = new XMLHttpRequest();

      // Itera sobre las propiedades de la clase TenantVm
      for (const prop in tenant) {
        if (tenant.hasOwnProperty(prop)) {
          // Verificar si la propiedad es un objeto IFormFile (archivos)
          if (tenant[prop] instanceof File && tenant[prop] !== null) {
            formData.append(this.capitalizeFirstLetter(prop), tenant[prop]);
          } else if (typeof tenant[prop] === 'object' && tenant[prop] !== null) {
            this.appendNestedProperties(formData, tenant[prop], this.capitalizeFirstLetter(prop));
          } else {
            // Verificar si la propiedad es de tipo Date
            if (tenant[prop] instanceof Date) {
              // Aquí puedes manejar la propiedad Date como desees
              // Por ejemplo, puedes convertirla a una cadena en el formato deseado
              const formattedDate = tenant[prop].toISOString(); // O el formato que prefieras
              formData.append(this.capitalizeFirstLetter(prop), formattedDate);
            } else {
              // Si no es Date, simplemente agrega la propiedad al FormData
              formData.append(this.capitalizeFirstLetter(prop), tenant[prop]);
            }
          }
        }
      }

      xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
          if (xhr.status === 200) {
            resolve(JSON.parse(xhr.response));
          } else {
            reject(xhr.response);
          }
        }
      };

      xhr.open('POST', `${this.apiUrl}/CreaActualizaTenant`, true);
      xhr.setRequestHeader('Authorization', this.utils.getHeaders(true).headers.get('Authorization'));
      xhr.send(formData);
    });
  }

  capitalizeFirstLetter(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  appendNestedProperties(formData, obj, parentKey = '') {
    for (const prop in obj) {
      if (obj.hasOwnProperty(prop)) {
        const value = obj[prop];
        const key = parentKey ? `${parentKey}.${prop}` : prop;

        if (value instanceof File) {
          formData.append(key, value);
        } else if (value instanceof Date) {
          formData.append(key, value.toISOString());
        } else if (typeof value === 'object' && value !== null) {
          // Si la propiedad es un objeto, llamar recursivamente
          this.appendNestedProperties(formData, value, key);
        } else if (value !== null) {
          formData.append(key, value);
        }
      }
    }
  }

  getTemplate(tipo: number, nombreEmpresa: string, config: any): Observable<any> {
    const body = JSON.stringify(config);
    return this.http.post<any>(`${this.apiUrl}/getTemplate/${tipo}/${nombreEmpresa}`, body, this.utils.getHeaders(true));
  }

}
