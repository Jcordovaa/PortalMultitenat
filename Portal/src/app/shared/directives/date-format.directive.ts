import { Directive, HostListener, ElementRef } from '@angular/core';
import { NgControl } from '@angular/forms';
import { DatePipe } from '@angular/common';

@Directive({
  selector: '[appDateFormat]'
})
export class DateFormatDirective {
  constructor(private el: ElementRef, private control: NgControl) {}

  @HostListener('ngModelChange', ['$event'])
  onInputChange(value: string) {
    debugger
    const formattedValue = this.formatDate(value);
    if (this.isValidDate(formattedValue)) {
      this.control.control.setValue(formattedValue, { emitEvent: false });
    } else {
      this.control.control.setValue('', { emitEvent: false });
    }
  }

  private formatDate(input: string): string {
    const parts = input.split('/');

    if (parts.length === 3) {
      const day = parts[0].padStart(2, '0');
      const month = parts[1].padStart(2, '0');
      const year = parts[2];
      
      return `${day}/${month}/${year}`;
    }
  }

  private isValidDate(input: string): boolean {
    console.log('Input:', input);

    const datePipe = new DatePipe('en-US');
    const formattedDate = datePipe.transform(input, 'dd/MM/yyyy');
    console.log('Formatted Date:', formattedDate);
  
    const isValid = formattedDate === input && !isNaN(Date.parse(formattedDate));
    console.log('Is Valid:', isValid);
  
    return isValid;
  }
}