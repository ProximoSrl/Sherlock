import { Component, OnInit } from '@angular/core';
import { ActorDataService } from '../actors.service';

@Component({
  selector: 'app-page-index',
  templateUrl: './page-index.component.html',
  styleUrls: ['./page-index.component.css']
})
export class PageIndexComponent implements OnInit {
  clients: string[];

  constructor(private _dataservice: ActorDataService) { }

  async ngOnInit() {
    this.clients = await this._dataservice.getClients();
  }
}
