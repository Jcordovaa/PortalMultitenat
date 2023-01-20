import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
    name: 'sintexto'
})
export class SinTextoPipe implements PipeTransform {

    transform(value: string): string {
        
        if (value == "" || value == null)
        {
            return "Sin Información";
        }else
        {
            return value;
        }
        
    }

}