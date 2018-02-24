import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class ActorDataService {

  constructor(private http: HttpClient) { }

  public getClients(): Promise<string[]> {
    return this.http.get<string[]>('/api/actors/clients').toPromise();
  }

  public getRoot(clientId: string): Promise<ISherlockNode> {
    const uri = '/api/actors/tree/'
      + encodeURIComponent(clientId);

    return this.http.get<ISherlockNode>(uri).toPromise();
  }

  public getDetail(clientId: string, nodeId: string): Promise<ISherlockNode> {
    const uri = '/api/actors/full/'
      + encodeURIComponent(clientId)
      + '/'
      + encodeURIComponent(nodeId);

    return this.http.get<ISherlockNode>(uri).toPromise();
  }
}

export interface ISherlockLink {
  name: string;
  url: string;
}

export interface ISherlockMessage {
  sequence: number;
  type: string;
  executionTimeMs?: string;
  exception?: string;
  exceptionMessage?: string;
  message?: string;
  expanded: boolean;
  when: string;
}

export interface ISherlockNode {
  id: string;
  parent: ISherlockLink;
  childsLinks: ISherlockLink[];
  internalState: { [key: string]: string };
  childsNodes: ISherlockNode[];
  trackedMessages: ISherlockMessage[];
  timestamp: string;
  warnings: string[];
  logs: ISherlockLog[];
}

export interface ISherlockLog {
  sequence: number;
  timestamp: string;
  logger: string;
  level: string;
  message: string;
}
