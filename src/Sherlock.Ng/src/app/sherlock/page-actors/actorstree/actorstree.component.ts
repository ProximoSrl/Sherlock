import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import {
  VisNode,
  VisNodes,
  VisEdges,
  VisEdge,
  VisNetworkService,
  VisNetworkData,
  VisNetworkOptions
} from 'ngx-vis';
import { OnDestroy, OnChanges } from '@angular/core/src/metadata/lifecycle_hooks';
import { ISherlockNode } from '../../actors.service';
import { Color } from 'vis';
import { id } from '@swimlane/ngx-charts/release/utils';

interface SherlockEdge extends VisEdge {
  color: any;
  dashes: boolean;
}

interface ActorNode extends VisNode {
  heightConstraint: {
    minimum: number
  };
  widthConstraint: {
    minimum: number
  };
}

interface ICustomColors {
  background: string;
  text: string;
}

class ExampleNetworkData implements VisNetworkData {
  public nodes: VisNodes;
  public edges: VisEdges;
}
@Component({
  selector: 'app-actorstree',
  templateUrl: './actorstree.component.html',
  styleUrls: ['./actorstree.component.css']
})
export class ActorstreeComponent implements OnInit, OnDestroy, OnChanges {
  @Input()
  public nodes: ISherlockNode;

  @Output()
  selected = new EventEmitter<ISherlockNode>();

  public visNetwork = 'networkId1';
  public visNetworkData: ExampleNetworkData;
  public visNetworkOptions: VisNetworkOptions;
  private map = new Map<string, ISherlockNode>();
  private firstNode: string;
  private colors: { [key: string]: ICustomColors } = {
    'default': { background: 'green', text: 'white' },
    'cluster_1': { background: 'orangered', text: 'white' },
    'cluster_2': { background: 'aliceblue', text: 'black' }
  };

  public constructor(private visNetworkService: VisNetworkService) { }

  public addNode(): void {
    const newId = this.visNetworkData.nodes.getLength() + 1;
    this.visNetworkData.nodes.add({ id: newId.toString(), label: 'Node ' + newId });
    this.visNetworkService.fit(this.visNetwork);
  }

  public networkInitialized(): void {
    // now we can use the service to register on events
    this.visNetworkService.on(this.visNetwork, 'click');

    // open your console/dev tools to see the click params
    this.visNetworkService.click
      .subscribe((eventData: any[]) => {
        if (eventData[0] === this.visNetwork) {
          const nodeId = eventData[1].nodes[0];
          this.selected.emit(this.map.get(nodeId));
        }
      });
  }

  public ngOnInit(): void {
    const nodes = new VisNodes();
    const edges = new VisEdges();

    this.visNetworkData = {
      nodes,
      edges,
    };

    this.visNetworkOptions = {
      autoResize: true,
      height: '100%',
      width: '100%',
      layout: {
        hierarchical: {
          direction: 'UD',
          levelSeparation: 100,
          nodeSpacing: 150,
          sortMethod: 'directed'
        }
      },
      physics: {
        enabled: false
      },
      nodes: {
        shape: 'box',
        size: 60,
        font: {
          size: 16,
          color: '#ffffff'
        },
        borderWidth: 1
      },
      edges: {
        width: 1
      }
    };
  }

  public ngOnDestroy(): void {
    this.visNetworkService.off(this.visNetwork, 'click');
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.nodes) {
      return;
    }

    this.map.clear();
    this.recursiveBuildNetwork(this.nodes, 'default');
    this.visNetworkService.fit(this.visNetwork);

    if (this.firstNode) {
      this.visNetworkService.selectNodes(this.visNetwork, [this.firstNode], true);
      this.selected.emit(this.map.get(this.firstNode));
    }
  }

  private recursiveBuildNetwork(
    nodeinfo: ISherlockNode,
    group: string
  ) {
    if (!nodeinfo) {
      return;
    }

    if (nodeinfo.id !== 'actors') {
      const newNode: ActorNode = {
        id: nodeinfo.id,
        label: this.getLabel(nodeinfo),
        heightConstraint: { minimum: 40 },
        widthConstraint: { minimum: 40 },
      };

      group = nodeinfo.status['kernel::group'] || group;

      this.setColor(newNode, nodeinfo, group);
      this.visNetworkData.nodes.add(newNode);
      this.map.set(nodeinfo.id, nodeinfo);

      if (!this.firstNode) {
        this.firstNode = nodeinfo.id;
      }
    }

    nodeinfo.childsNodes.forEach(element => {
      this.recursiveBuildNetwork(element, group);
      const edge: SherlockEdge = {
        from: nodeinfo.id,
        to: element.id,
        color: { color: 'darkgray' },
        dashes: true
      };
      this.visNetworkData.edges.add(edge);
    });
  }

  private getLabel(node: ISherlockNode): string {
    const name = node.status['kernel::name'];

    if (name) {
      return name;
    }

    const last = node.id.lastIndexOf('/');
    if (last > 0) {
      return node.id.substr(last + 1);
    }
    return node.id;
  }

  setColor(info: ActorNode, node: ISherlockNode, group: string): void {
    const groupSettings = this.colors[group] || { background: 'green', text: 'white' };
    let back = groupSettings.background;

    if (node.status['kernel::status'] === 'Stopped') {
      back = 'darkred';
    }

    if (node.warnings.length > 0) {
      back = 'orangered';
    }

    const color: Color = {
      border: 'darkgray',
      background: back,
      highlight: {
        border: '#7BE141',
        background: back
      }
    };

    (<any>info).font = { color: groupSettings.text };

    info.color = color;
  }
}
