import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-state',
  templateUrl: './state.component.html',
  styleUrls: ['./state.component.css']
})
export class StateComponent implements OnInit, OnChanges {
  @Input()
  state: { [key: string]: string };

  properties: { key: string, value: string }[];

  constructor() { }

  ngOnInit() {
    this.properties = [];
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.properties = [];

    if (!this.state) {
      return;
    }

    for (const i in this.state) {
      if (this.state.hasOwnProperty(i)) {
        this.properties.push({ key: i, value: this.state[i] });
      }
    }

    if (this.properties.length % 2 === 1) {
      this.properties.push({ key: '--', value: ' ' });
    }
  }
}
