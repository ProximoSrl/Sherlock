import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { ISherlockLog } from '../../actors.service';
import { OnChanges } from '@angular/core/src/metadata/lifecycle_hooks';

@Component({
  selector: 'app-logviewer',
  templateUrl: './logviewer.component.html',
  styleUrls: ['./logviewer.component.css']
})
export class LogviewerComponent implements OnInit, OnChanges {
  @Input()
  logs: ISherlockLog[];
  levels = ['Verbose', 'Debug', 'Information', 'Warning', 'Error'];
  constructor() { }

  ngOnInit() {
    this.logs = [];
  }

  ngOnChanges(changes: SimpleChanges): void {
  }
}
