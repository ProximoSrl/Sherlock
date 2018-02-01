import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MaterialModule } from '../material/material.module';
import { PageIndexComponent } from './page-index/page-index.component';
import { LayoutComponent } from './layout/layout.component';
import { SherlockRoutingModule } from './sherlock.routes';
import { PageActorsComponent } from './page-actors/page-actors.component';
import { ActorstreeComponent } from './page-actors/actorstree/actorstree.component';
import { VisModule } from 'ngx-vis';
import { ActorDataService } from './actors.service';
import { PropertiesComponent } from './page-actors/properties/properties.component';
import { MessagesComponent } from './page-actors/properties/messages/messages.component';
import { StateComponent } from './page-actors/properties/state/state.component';
import { AlertsComponent } from './page-actors/alerts/alerts.component';
import { LogviewerComponent } from './page-actors/properties/logviewer/logviewer.component';
import { PaddedNumberPipe } from './padded-number.pipe';
import { HeaderComponent } from './page-actors/header/header.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    SherlockRoutingModule,
    VisModule,
    MaterialModule,
    HttpClientModule,
    SharedModule
  ],
  declarations: [
    PageIndexComponent,
    LayoutComponent,
    PageActorsComponent,
    ActorstreeComponent,
    PropertiesComponent,
    MessagesComponent,
    StateComponent,
    AlertsComponent,
    LogviewerComponent,
    PaddedNumberPipe,
    HeaderComponent,
  ],
  providers: [
    ActorDataService,
    DatePipe
  ]
})
export class SherlockModule { }
