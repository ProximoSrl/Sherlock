import { Component, OnInit } from '@angular/core';
import { ActorDataService, ISherlockNode, ISherlockLog } from '../actors.service';
import 'rxjs/add/operator/switchMap';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-page-actors',
  templateUrl: './page-actors.component.html',
  styleUrls: ['./page-actors.component.css']
})
export class PageActorsComponent implements OnInit {
  _clientId: string;
  public nodes: ISherlockNode;
  public selected: ISherlockNode;

  constructor(
    private _route: ActivatedRoute,
    private _dataservice: ActorDataService
  ) { }

  async ngOnInit() {
    this._clientId = this._route.snapshot.paramMap.get('clientId');
    this.nodes = await this._dataservice.getRoot(this._clientId);
  }

  async nodeSelected(node: ISherlockNode) {
    if (node) {
      this.selected = await this._dataservice.getDetail(this._clientId, node.id);
    }
  }
}
