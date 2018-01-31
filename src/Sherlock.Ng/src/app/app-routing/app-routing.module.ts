import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'sherlock', loadChildren: 'app/sherlock/sherlock.module#SherlockModule' },
  { path: '', redirectTo: 'sherlock', pathMatch: 'full' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { useHash: true })
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class AppRoutingModule { }
