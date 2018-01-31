import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { PageIndexComponent } from './page-index/page-index.component';
import { PageActorsComponent } from './page-actors/page-actors.component';

const routes: Routes = [
  {
    path: '', component: LayoutComponent,
    children: [
      { path: 'index', component: PageIndexComponent },
      { path: 'actors', component: PageActorsComponent },
      { path: '', redirectTo: 'index', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class SherlockRoutingModule { }
