import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutConstants, RouteConstants } from '@core/constants';
import { RoutKey } from '@core/enums';
import { LoginPageComponent, LogoutPageComponent, RegistrationPageComponent } from './pages';

const routes: Routes = [
  {
    path: RoutKey.Login,
    component: LoginPageComponent,
    data: { layout: LayoutConstants.ONLY_CONTENT_LAYOUT_MODEL, theme: { enabled: false } }
  },
  {
    path: RoutKey.Registration,
    component: RegistrationPageComponent,
    data: { layout: LayoutConstants.ONLY_CONTENT_LAYOUT_MODEL, theme: { enabled: false } }
  },
  {
    path: RoutKey.Logout,
    component: LogoutPageComponent,
    data: { layout: LayoutConstants.FULL_LAYOUT_MODEL, theme: { enabled: true } }
  },
  {
    path: RouteConstants.DEFAULT_ROUTE_PATH,
    redirectTo: RoutKey.Registration,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IdentityRoutingModule { }
