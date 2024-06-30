import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FooterComponent, HeaderComponent } from './components';
import { NgxSfcCommonModule } from 'ngx-sfc-common';
import { ShareModule } from '@share/share.module';
import { CanvasBallDirective, ErrorPageComponent, NotFoundPageComponent } from './pages';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [
    FooterComponent,
    HeaderComponent,
    ErrorPageComponent,
    NotFoundPageComponent,
    CanvasBallDirective
  ],
  imports: [
    CommonModule,
    FontAwesomeModule,
    NgxSfcCommonModule,
    ShareModule
  ],
  exports: [
    FooterComponent,
    HeaderComponent,
    ErrorPageComponent,
    NotFoundPageComponent
  ]
})
export class CoreModule { }
