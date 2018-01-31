import { Component, ElementRef, Input, OnInit, SimpleChanges, ViewChild, OnChanges } from '@angular/core';
import { MatPaginator } from '@angular/material';
import { DataSource } from '@angular/cdk/collections';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/startWith';
import 'rxjs/add/observable/merge';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/observable/fromEvent';
import { ISherlockNode, ISherlockMessage } from '../../actors.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit, OnChanges {
  @ViewChild('filter') filter: ElementRef;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  @Input()
  messages: ISherlockMessage[];

  displayedColumns = ['messagePosition', 'messageDirection', 'messageType', 'messageTarget', 'timestamp', 'executionTimeMs'];
  exampleDatabase = new ExampleDatabase();
  dataSource: MessagesDataSource | null;

  constructor() { }

  ngOnInit() {
    this.messages = [];
    this.dataSource = new MessagesDataSource(
      this.exampleDatabase,
      this.paginator
    );

    Observable.fromEvent(this.filter.nativeElement, 'keyup')
      .debounceTime(150)
      .distinctUntilChanged()
      .subscribe(() => {
        if (!this.dataSource) {
          return;
        }
        this.dataSource.filter = this.filter.nativeElement.value;
      });
  }

  public displayDetail(index: number, row: ISherlockMessage): boolean {
    return row.expanded;
  }

  public toggle(row: ISherlockMessage) {
    row.expanded = !row.expanded;
    this.refreshTable();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.exampleDatabase.resetTo(this.messages || []);
  }

  private refreshTable() {
    this.exampleDatabase.resetTo([]);
    this.exampleDatabase.resetTo(this.messages);
  }
}

export class ExampleDatabase {
  /** Stream that emits whenever the data has been modified. */
  dataChange: BehaviorSubject<ISherlockMessage[]> = new BehaviorSubject<ISherlockMessage[]>([]);
  get data(): ISherlockMessage[] { return this.dataChange.value; }

  constructor() {
  }

  resetTo(messages: ISherlockMessage[]): void {
    this.dataChange.next(messages);
  }
}

/**
 * Data source to provide what data should be rendered in the table. Note that the data source
 * can retrieve its data in any way. In this case, the data source is provided a reference
 * to a common data base, ExampleDatabase. It is not the data source's responsibility to manage
 * the underlying data. Instead, it only needs to take the data and send the table exactly what
 * should be rendered.
 */
export class MessagesDataSource extends DataSource<any> {
  _filterChange = new BehaviorSubject('');
  get filter(): string { return this._filterChange.value; }
  set filter(filter: string) { this._filterChange.next(filter); }

  constructor(
    private _exampleDatabase: ExampleDatabase,
    private _paginator: MatPaginator) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<ISherlockMessage[]> {
    const displayDataChanges = [
      this._exampleDatabase.dataChange,
      this._filterChange,
      this._paginator.page
    ];

    return Observable.merge(...displayDataChanges).map(() => {
      const data = this._exampleDatabase.data
        .slice()
        .filter((item: ISherlockMessage) => {
          const searchStr = (item.type).toLowerCase();
          if (this.filter.startsWith('-')) {
            return searchStr.indexOf(this.filter.toLowerCase().substr(1)) === -1;
          } else {
            return searchStr.indexOf(this.filter.toLowerCase()) !== -1;
          }
        });

      // Grab the page's slice of data.
      const startIndex = this._paginator.pageIndex * this._paginator.pageSize;
      return data.splice(startIndex, this._paginator.pageSize);
    });
  }

  disconnect() { }
}
