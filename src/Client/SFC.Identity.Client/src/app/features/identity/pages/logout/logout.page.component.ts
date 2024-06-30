import { Component, HostBinding, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { map, Observable, tap } from "rxjs";
import { IdentityService } from "../../services/identity/identity.service";
import { DomSanitizer, Title } from "@angular/platform-browser";
import { LogoutPageConstants } from "./logout.page.constants";
import { buildTitle } from "@core/utils";
import { LogoutPageLocalization } from "./logout.page.localization";
import {
  ButtonType, ComponentSize, isNullOrEmptyString, LoaderService, Theme, WINDOW
} from 'ngx-sfc-common';
import { faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
import { LogoutState } from './logout-state.enum';
import { ThemeService } from '@core/services';
import { environment } from '@environments/environment';
import { ILogoutResponse } from '../../services/identity/models';
import { faQuestionCircle } from '@fortawesome/free-regular-svg-icons';
import { ILogoutPageModel } from './logout.page.model';

@Component({
  templateUrl: './logout.page.component.html',
  styleUrls: ['./logout.page.component.scss']
})
export class LogoutPageComponent implements OnInit {

  faRightFromBracket = faRightFromBracket;
  faQuestionCircle = faQuestionCircle;

  ButtonType = ButtonType;
  ComponentSize = ComponentSize;

  LogoutState = LogoutState;
  Localization = LogoutPageLocalization;

  public model$!: Observable<ILogoutPageModel>;

  private logoutId: string | null;

  @HostBinding('class')
  public state: LogoutState = LogoutState.Initialization;

  constructor(
    public sanitizer: DomSanitizer,
    @Inject(WINDOW) private window: Window,
    private identityService: IdentityService,
    private titleService: Title,
    private route: ActivatedRoute,
    private loaderService: LoaderService,
    private themeService: ThemeService) {
    this.logoutId = this.route.snapshot.queryParamMap.get(LogoutPageConstants.LOGOUT_ID_PART_KEY);
    this.loaderService.show();
  }

  ngOnInit(): void {
    this.model$ = this.identityService.logout(this.logoutId).pipe(
      tap((response: ILogoutResponse) => {
        if (response.AutomaticRedirectAfterSignOut && !isNullOrEmptyString(response.PostLogoutRedirectUrl)) {
          this.state = LogoutState.Automatic;
          this.themeService.set(Theme.Default);
          this.window.location.href = response.PostLogoutRedirectUrl!;
        } else if (response.ShowLogoutPrompt) {
          this.state = LogoutState.Prompt;
        } else if (!isNullOrEmptyString(response.PostLogoutRedirectUrl)) {
          this.state = LogoutState.Redirect;
        } else if (!isNullOrEmptyString(response.SignOutIFrameUrl)) {
          this.state = LogoutState.IFrame;
        }

        if (this.state != LogoutState.Automatic) {
          this.loaderService.hide();
          this.themeService.set(Theme.Dark);
        }
      }),
      map((response: ILogoutResponse) => this.mapResponse(response))
    );

    this.titleService.setTitle(buildTitle(this.Localization.PAGE_TITLE));
  }

  public confirm(): void {
    this.model$ = this.identityService.postLogout(this.logoutId).pipe(
      tap((response: ILogoutResponse) => {
        if (!isNullOrEmptyString(response.PostLogoutRedirectUrl)) {
          this.state = LogoutState.Redirect;
        } else if (!isNullOrEmptyString(response.SignOutIFrameUrl)) {
          this.state = LogoutState.IFrame;
        }

        this.loaderService.hide();
      }),
      map((response: ILogoutResponse) => this.mapResponse(response))
    );
  }

  public cancel(): void {
    this.window.location.href = environment.client_url;
  }

  private mapResponse(response: ILogoutResponse): ILogoutPageModel {
    return {
      automaticRedirectAfterSignOut: response.AutomaticRedirectAfterSignOut,
      buttonText: `${LogoutPageLocalization.BACK} ${LogoutPageLocalization.TO} ${response.ClientName?.toUpperCase()}`,
      clientName: response.ClientName,
      postLogoutRedirectUrl: response.PostLogoutRedirectUrl,
      showLogoutPrompt: response.ShowLogoutPrompt,
      signOutIFrameUrl: response.SignOutIFrameUrl
    };
  }
}
