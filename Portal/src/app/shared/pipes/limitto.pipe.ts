import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
    name: 'limitTo'
})
export class LimitToPipe implements PipeTransform {

    transform(value: any, limit?: number): any {
        if (value != null) {
            if (value.length > limit) {
                return `${value.toString().substring(0, limit)}...`;
            } else {
                return value;
            }            
        }
        return ''
    }

}