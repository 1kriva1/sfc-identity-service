import { AfterViewInit, Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ButtonType, CheckmarkType, Direction, isEmail, LoaderService, WINDOW } from "ngx-sfc-common";
import { SliderType } from "ngx-sfc-components";
import { LoginPageConstants } from "./login.page.constants";
import { LoginPageLocalization } from "./login.page.localization";
import { BaseErrorResponse } from '@core/models/http/base-error.response';
import { catchError, filter, fromEvent, map, of, startWith, Subscription, switchMap, tap } from "rxjs";
import { IdentityService } from "../../services/identity/identity.service";
import { Title } from "@angular/platform-browser";
import { IForm } from '@core/types';
import { ILoginPageModel } from "./login.page.model";
import { buildPath, buildTitle, markFormTouchedAndDirty } from '@core/utils';
import { ILoginRequest, ILoginResponse } from '../../services/identity/models';
import { RoutKey } from '@core/enums';
import { IdentityPageConstants } from '../base/identity.page.constants';
import { IIdentityNavigationModel } from '../base/identity-navigation.model';

@Component({
  templateUrl: './login.page.component.html',
  styleUrls: ['../base/base-identity.page.component.scss', './login.page.component.scss']
})
export class LoginPageComponent implements OnInit, AfterViewInit, OnDestroy {

  ButtonType = ButtonType;
  Direction = Direction;
  SliderType = SliderType;
  CheckmarkType = CheckmarkType;

  Constants = LoginPageConstants;
  Localization = LoginPageLocalization;

  public submitted: boolean = false;

  public error: BaseErrorResponse | null = null;

  public get registrationLinkModel(): IIdentityNavigationModel {
    return {
      link: `${buildPath(RoutKey.Identity)}${buildPath(RoutKey.Registration)}`,
      params: { [IdentityPageConstants.RETURN_URL_PART_KEY]: this.returnUrl }
    };
  }

  public form!: FormGroup;

  @ViewChild('submitBtn', { static: false, read: ElementRef })
  private submitBtn!: ElementRef;

  private returnUrl: string | null;

  private _subscription!: Subscription;

  constructor(
    @Inject(WINDOW) private window: Window,
    private fb: FormBuilder,
    private identityService: IdentityService,
    private titleService: Title,
    private route: ActivatedRoute,
    private loaderService: LoaderService) {
    this.returnUrl = this.route.snapshot.queryParamMap.get(IdentityPageConstants.RETURN_URL_PART_KEY);
  }

  ngOnInit(): void {
    const controls: IForm<ILoginPageModel> = {
      userNameEmail: [null, [Validators.required]],
      password: [null, [Validators.required]],
      remember: [false]
    };

    this.form = this.fb.group(controls);
    this.titleService.setTitle(buildTitle(this.Localization.PAGE_TITLE));
  }

  ngAfterViewInit(): void {
    this._subscription = this.form.valueChanges.pipe(
      startWith({}),
      tap(() => this.error = null),
      switchMap((value: ILoginPageModel) => {
        return fromEvent<InputEvent>(this.submitBtn.nativeElement, 'click')
          .pipe(
            tap(() => this.tapSubmit()),
            filter(() => this.form.valid),
            map(() => this.mapRequest(value)),
            tap(() => this.loaderService.show()),
            switchMap((request: ILoginRequest) =>
              this.identityService.login(request).pipe(
                catchError(error => of(error))
              ))
          );
      })
    ).subscribe((response: ILoginResponse) => {
      this.error = response.Success ? null : response as BaseErrorResponse;

      if (response.Success) {
        this.window.location.href = response.ReturnUrl;
      } else {
        this.loaderService.hide();
      }
    });
  }

  ngOnDestroy(): void {
    this._subscription.unsubscribe();
  }

  private tapSubmit(): void {
    if (!this.submitted) {
      this.submitted = true;
      markFormTouchedAndDirty(this.form);
    }
  }

  private mapRequest(value: ILoginPageModel): ILoginRequest {
    const request: ILoginRequest = {
      Password: value.password,
      RememberMe: value.remember,
      ReturnUrl: this.returnUrl
    };

    if (isEmail(value.userNameEmail))
      request.Email = value.userNameEmail;
    else
      request.UserName = value.userNameEmail;

    return request;
  }
}
