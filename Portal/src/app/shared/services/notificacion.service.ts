import { Injectable } from '@angular/core';
import Swal from 'sweetalert2'

@Injectable({
    providedIn: 'root'
  })
export class NotificationService {

    public success(title: string, text: string, isToast: boolean = false): void {
        if (!isToast) {
            Swal.fire({
                title: title,
                text: text,
                icon: 'success',
                timerProgressBar: true,
                confirmButtonText: 'Aceptar'
            })
        } else {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                showCloseButton: true
            })

            Toast.fire({
                icon: 'success',
                title: title
            })
        }
    }

    public error(title: string, text: string, isToast: boolean = false): void {
        if (!isToast) {
            Swal.fire({
                title: title,
                text: text,
                icon: 'error',
                timerProgressBar: true,
                confirmButtonText: 'Aceptar'
            })
        } else {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                showCloseButton: true
            })

            Toast.fire({
                icon: 'error',
                title: title
            })
        }
    }

    public warning(title: string, text: string, isToast: boolean = false): void {
        if (!isToast) {
            Swal.fire({
                title: title,
                text: text,
                icon: 'warning',
                timerProgressBar: true,
                confirmButtonText: 'Aceptar'
            })
        } else {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                showCloseButton: true
            })

            Toast.fire({
                icon: 'warning',
                title: title
            })
        }
    }

    public info(title: string, text: string, isToast: boolean = false): void {
        if (!isToast) {
            Swal.fire({
                title: title,
                text: text,
                icon: 'info',
                timerProgressBar: true,
                confirmButtonText: 'Aceptar'
            })
        } else {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                showCloseButton: true
            })

            Toast.fire({
                icon: 'info',
                title: title
            })
        }
    }

    public async confirmation(title: string, text: string, confirmButtonText: string = 'Aceptar', cancelButtonText: string = 'Cancelar') {
        const res = await Swal.fire({
            title: title,
            text: text,
            icon: 'question',
            confirmButtonText: confirmButtonText,
            cancelButtonText: cancelButtonText,
            showCancelButton: true,
            showCloseButton: true
        })
        return res
    }

    public async warningAync(title: string, text: string) {
        const res = await Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            confirmButtonText: 'Aceptar',
            showCancelButton: false,
            showCloseButton: true
        })
        return res
    }

    public async sesionExpiredMsg(title: string, text: string) {
        const res = await Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: false,
            backdrop: true,
            confirmButtonText: 'Aceptar',
            showConfirmButton: false,
            timerProgressBar: true,
            timer: 5000,
            allowEnterKey: false,
            focusConfirm: false
        })   
        return res
    }

}