import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ElapsedTimePipe } from './elapsed-time.pipe';
import { FrameComponent } from './frame/frame.component';
import { TimestampPipe } from './timestamp.pipe';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    ElapsedTimePipe,
    FrameComponent,
    TimestampPipe
  ],
  exports: [
    ElapsedTimePipe,
    FrameComponent,
    TimestampPipe
  ]
})
export class SharedModule { }
