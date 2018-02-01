import { Component, OnInit } from '@angular/core';
import { ActorDataService } from '../actors.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  clients: string[] = [];

  constructor(private _dataservice: ActorDataService) { }

  async ngOnInit() {
    this.clients = await this._dataservice.getClients();
  }
}
