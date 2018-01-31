import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-frame',
  templateUrl: './frame.component.html',
  styleUrls: ['./frame.component.css']
})
export class FrameComponent implements OnInit {
  @Input()
  title= '';
  constructor() { }

  ngOnInit() {
  }

}
