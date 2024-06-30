import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutConstants, RouteConstants } from '@core/constants';
import { RoutKey } from '@core/enums';
import { ErrorPageComponent, NotFoundPageComponent } from '@core/pages';
import { Theme } from 'ngx-sfc-common';

const routes: Routes = [
  {
    path: RoutKey.Identity,
    loadChildren: () => import('./features/identity/identity.module').then(m => m.IdentityModule)
  },
  {
    path: RoutKey.Error,
    component: ErrorPageComponent,
    data: { layout: LayoutConstants.FULL_LAYOUT_MODEL, theme: { enabled: true, value: Theme.Dark } }
  },
  {
    path: RouteConstants.DEFAULT_ROUTE_PATH,
    redirectTo: RoutKey.Identity,
    pathMatch: 'full'
  },
  {
    path: RouteConstants.NOT_FOUND_ROUTE_PATH,
    component: NotFoundPageComponent,
    data: { layout: LayoutConstants.ONLY_CONTENT_LAYOUT_MODEL, theme: { enabled: false } }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
