import { Component, HostBinding, OnDestroy, OnInit } from '@angular/core';
import { ActivationStart, Router } from '@angular/router';
import { nameof, Theme } from 'ngx-sfc-common';
import { INotificationAutoCloseModel } from 'ngx-sfc-components';
import { Subscription } from 'rxjs';
import { IRouteDataModel } from '@core/models/route-data.model';
import { ILayoutModel } from '@core/models/layout.model';
import { NotificationService } from '@core/services/notification/notification.service';
import { AppComponentConstants } from './app.component.constants';
import { LayoutConstants } from '@core/constants';
import { ThemeService } from '@core/services';
import { IThemeModel } from '@core/models';

@Component({
  selector: 'sfc-identity-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {

  public layout: ILayoutModel = LayoutConstants.ONLY_CONTENT_LAYOUT_MODEL;

  public notificationAutoCloseModel: INotificationAutoCloseModel = {
    enabled: true,
    interval: AppComponentConstants.NOTIFICATION_AUTO_CLOSE_INTERVAL_MS
  };

  @HostBinding('class')
  public get themeValue(): Theme | null { return this.theme.enabled ? this.theme.value || this.themeService.theme : null; }

  private theme: IThemeModel = { enabled: true };

  private _subscription!: Subscription;

  constructor(
    public notificationService: NotificationService,
    private router: Router,
    private themeService: ThemeService) { }

  ngOnInit(): void {
    this._subscription = this.router.events.subscribe(data => {
      if (data instanceof ActivationStart) {
        const dataValue: IRouteDataModel = data.snapshot.data as IRouteDataModel,
          theme: IThemeModel = dataValue[nameof<IRouteDataModel>('theme')] as IThemeModel;

        this.layout = (dataValue[nameof<IRouteDataModel>('layout')] as ILayoutModel)
          ?? LayoutConstants.ONLY_CONTENT_LAYOUT_MODEL;

        this.theme = theme;
      }
    });
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }
}