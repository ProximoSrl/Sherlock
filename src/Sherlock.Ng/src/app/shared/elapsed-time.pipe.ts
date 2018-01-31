import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'elapsedTime'
})
export class ElapsedTimePipe implements PipeTransform {

  transform(value: number, args?: any): any {
    if (value <= 0) {
      return '--';
    }

    const h = Math.floor(value / 3600);
    const m = Math.floor((value % 3600) / 60);

    return this.format(h) + ':' + this.format(m);
  }

  format(value: number): string {
    if (value < 10) {
      return '0' + value;
    } else {
      return '' + value;
    }
  }
}
