import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'paddedNumber'
})
export class PaddedNumberPipe implements PipeTransform {

  transform(value: any, digits?: any): any {
    return ('000000000000000' + (value || 0)).slice(-(digits || 5));
  }
}

function isEmpty(value: any): boolean {
  return value == null || value === '' || value !== value;
}
