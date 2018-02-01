import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ISherlockNode } from '../../actors.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnChanges {
  @Input()
  node: ISherlockNode;

  kernel: IKernelData;

  constructor() { }

  ngOnInit() {
    this.kernel = <IKernelData>{};
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.node) {
      return;
    }

    this.kernel = <IKernelData>{};
    for (const i in this.node.status) {
      if (this.node.status.hasOwnProperty(i)) {
        if (i.startsWith('kernel::')) {
          const propname = i.substr(8);
          this.kernel[propname] = this.convertProperty(i, this.node.status[i]);
        }
      }
    }

    if (this.kernel.actorType) {
      this.kernel.name = this.kernel.actorType.substr(this.kernel.actorType.lastIndexOf('.') + 1);
      this.kernel.namespace = this.kernel.actorType.substr(0, this.kernel.actorType.lastIndexOf('.'));
    } else {
      this.kernel.name = '';
      this.kernel.namespace = '';
    }
  }

  private convertProperty(propname: string, value: string): any {
    switch (propname) {
      case 'kernel::receivedMessages':
        return parseInt(value, 10);
    }
    return value;
  }

}

interface IKernelData {
  name: string;
  namespace: string;
  status: string;
  uptime: string;
  actorType: string;
  receivedMessages: number;
}
