import { Injectable } from '@angular/core';

declare var $: any;

@Injectable()
export class LoadingService {

    showFull() {
        $.LoadingOverlay("show", {
            size: 6,
            image: "",
            fontawesome: "fa fa-refresh fa-spin"
        });
    }

    hideFull() {
        $.LoadingOverlay("hide");
    }

    showElement(element: string, isClass: boolean = false) {
        const elm = !isClass ? `#${element}` : `.${element}`
        $(elm).LoadingOverlay("show", {
            size: 6,
            image: "",
            fontawesome: "fa fa-refresh fa-spin"
        });
    }

    hideElement(element: string, isClass: boolean = false) {
        const elm = !isClass ? `#${element}` : `.${element}`
        $(elm).LoadingOverlay("hide", true);
    }

}