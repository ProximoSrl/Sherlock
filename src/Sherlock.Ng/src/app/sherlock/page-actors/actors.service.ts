import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class ActorDataService {

  constructor(private http: HttpClient) { }

  public getRoot(): Promise<ISherlockNode> {
    return this.http.get<ISherlockNode>('/api/sherlock/tree').toPromise();
    //    return Promise.resolve(json);
  }

  public getDetail(nodeId: string): Promise<ISherlockNode> {
    return this.http.get<ISherlockNode>('/api/sherlock/full/' + encodeURIComponent(nodeId)).toPromise();
  }

  public getLogs(nodeId: string): Promise<ISherlockLog[]> {
    return this.http.get<ISherlockLog[]>('/api/sherlock/messages/' + encodeURIComponent(nodeId)).toPromise();
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
  status: { [key: string]: string };
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
