import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from "@angular/forms";
import { IdentityRoutingModule } from './identity-routing.module';
import { NgxSfcCommonModule } from 'ngx-sfc-common';
import { NgxSfcComponentsModule } from 'ngx-sfc-components';
import { NgxSfcInputsModule } from 'ngx-sfc-inputs';
import {
  LoginPageComponent,
  LogoutPageComponent,
  RegistrationPageComponent
} from './pages';
import { ShareModule } from '@share/share.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [
    LoginPageComponent,
    LogoutPageComponent,
    RegistrationPageComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    NgxSfcCommonModule,
    NgxSfcComponentsModule,
    NgxSfcInputsModule,
    ShareModule,
    IdentityRoutingModule
  ]
})
export class IdentityModule { }
