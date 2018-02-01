import { Component, Input, OnChanges, OnInit, SimpleChanges, ElementRef, ViewChild } from '@angular/core';
import { ISherlockNode, ISherlockMessage } from '../../actors.service';

@Component({
  selector: 'app-properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.css']
})
export class PropertiesComponent implements OnInit, OnChanges {
  @Input()
  node: ISherlockNode;

  properties: { [key: string]: string };

  constructor() {
  }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.node == null) {
      return;
    }

    this.properties = {};
    for (const i in this.node.status) {
      if (this.node.status.hasOwnProperty(i)) {
        if (!i.startsWith('kernel::')) {
          this.properties[i] = this.node.status[i];
        }
      }
    }
  }
}
