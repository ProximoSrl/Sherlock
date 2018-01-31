import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent implements OnInit, OnChanges {
  @Input()
  alerts: string[];
  constructor() { }

  ngOnInit() {
    this.alerts = [];
  }

  public ngOnChanges(changes: SimpleChanges): void {
  }
}
