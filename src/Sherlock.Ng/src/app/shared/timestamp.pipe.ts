import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'timestamp'
})
export class TimestampPipe implements PipeTransform {
  constructor(private datePipe: DatePipe) { }

  transform(value: any, args?: any): any {
    const t = new Date(1970, 0, 1); // Epoch
    t.setSeconds(value.seconds);
    t.setMilliseconds(value.nanos / 1000000);
    return this.datePipe.transform(t, args);
  }
}
