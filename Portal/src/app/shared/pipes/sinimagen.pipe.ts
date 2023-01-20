import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
    name: 'sinimagen'
})
export class SinImagenPipe implements PipeTransform {

    constructor(private domSanitizer: DomSanitizer) { }

    transform(value: any, args?: any): any {

        if (!value) {
           return "assets/images/noimage.png";
        }

        //return (value.length > 0) ? value[1].url : "images/noimage.png";

        if (value.length == 0) {
            return "images/noimage.png";
        }
        else {
            return this.cleanURL(value);
            //return this.domSanitizer.bypassSecurityTrustHtml(value);
        }
    }

    cleanURL(oldURL: string): SafeResourceUrl {
        return this.domSanitizer.bypassSecurityTrustUrl(oldURL);
    }

}