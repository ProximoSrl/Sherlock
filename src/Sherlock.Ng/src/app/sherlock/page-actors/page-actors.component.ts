import { Component, OnInit } from '@angular/core';
import { ActorDataService, ISherlockNode, ISherlockLog } from './actors.service';

@Component({
  selector: 'app-page-actors',
  templateUrl: './page-actors.component.html',
  styleUrls: ['./page-actors.component.css']
})
export class PageActorsComponent implements OnInit {
  public logs: ISherlockLog[];
  public nodes: ISherlockNode;
  public selected: ISherlockNode;
  constructor(private _dataservice: ActorDataService) { }

  async ngOnInit() {
    this.nodes = await this._dataservice.getRoot();
  }

  async nodeSelected(node: ISherlockNode) {
    if (node) {
      this.selected = await this._dataservice.getDetail(node.id);
      this.logs = await this._dataservice.getLogs(node.id);
    }
  }
}
