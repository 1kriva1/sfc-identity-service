import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxSfcCommonModule } from 'ngx-sfc-common';
import { NgxSfcComponentsModule } from 'ngx-sfc-components';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from '@core/core.module';
import { I18nModule} from '@core/initializers';
import { HttpInterceptorProviders } from '@core/interceptors';
import { HttpClientModule } from '@angular/common/http';
import { ShareModule } from '@share/share.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserAnimationsModule,
    HttpClientModule,
    NgxSfcCommonModule,
    NgxSfcComponentsModule,
    CoreModule,
    ShareModule,
    AppRoutingModule
  ],
  providers: [
    I18nModule.setLocale(),
    I18nModule.setLocaleId(),
    HttpInterceptorProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
