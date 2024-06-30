import { AfterViewInit, Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ButtonType, Direction, isEmail, ComponentSize, LoaderService, nameof, WINDOW } from 'ngx-sfc-common';
import { SliderType } from 'ngx-sfc-components';
import { map, filter, switchMap, tap, fromEvent, Subscription, startWith, catchError, of } from 'rxjs';
import { ExistenceService } from '../../services/existence/existence.service';
import { RegistrationPageConstants } from './registration.page.constants';
import { RegistrationValidators } from './validators/registration.page.validators';
import { BaseErrorResponse } from '@core/models/http/base-error.response';
import { ActivatedRoute } from '@angular/router';
import { RoutKey } from '@core/enums';
import { buildPath, buildTitle, markFormTouchedAndDirty } from '@core/utils';
import { IRegistrationPageModel } from './registration.page.model';
import { IForm } from '@core/types';
import { RegistrationPageLocalization } from './registration.page.localization';
import { match } from 'ngx-sfc-inputs';
import { Title } from '@angular/platform-browser';
import { IdentityService } from '../../services/identity/identity.service';
import { IRegistrationRequest, IRegistrationResponse } from '../../services/identity/models';
import { IIdentityNavigationModel } from '../base/identity-navigation.model';
import { IdentityPageConstants } from '../base/identity.page.constants';

@Component({
    selector: 'sfc-registration.page',
    templateUrl: './registration.page.component.html',
    styleUrls: ['../base/base-identity.page.component.scss', './registration.page.component.scss']
})
export class RegistrationPageComponent implements OnInit, AfterViewInit, OnDestroy {

    ButtonType = ButtonType;
    Direction = Direction;
    SliderType = SliderType;
    ComponentSize = ComponentSize;

    Constants = RegistrationPageConstants;
    Localization = RegistrationPageLocalization;

    public form!: FormGroup;

    public submitted: boolean = false;

    public error: BaseErrorResponse | null = null;

    public get loginLinkModel(): IIdentityNavigationModel {
        return {
            link: `${buildPath(RoutKey.Identity)}${buildPath(RoutKey.Login)}`,
            params: { [IdentityPageConstants.RETURN_URL_PART_KEY]: this.returnUrl }
        };
    }

    @ViewChild('submitBtn', { static: false, read: ElementRef })
    private submitBtn!: ElementRef;

    private returnUrl: string | null;

    private _subscription!: Subscription;

    constructor(
        @Inject(WINDOW) private window: Window,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private existenceService: ExistenceService,
        private loaderService: LoaderService,
        private identityService: IdentityService,
        private titleService: Title) {
        this.returnUrl = this.route.snapshot.queryParamMap.get(IdentityPageConstants.RETURN_URL_PART_KEY);
    }

    ngOnInit(): void {
        const controls: IForm<IRegistrationPageModel> = {
            userNameEmail: [null,
                [Validators.required, RegistrationValidators.userNameEmail],
                [RegistrationValidators.userNameEmailExist(this.existenceService, this.loaderService, RegistrationPageConstants.USERNAME_EMAIL_ID)]
            ],
            password: [null,
                [
                    Validators.required,
                    Validators.pattern(RegistrationPageConstants.PASSWORD_PATTERN),
                    match(nameof<IRegistrationPageModel>('confirmPassword'), true)
                ]
            ],
            confirmPassword: [null,
                [Validators.required, match(nameof<IRegistrationPageModel>('password'))]
            ]
        };

        this.form = this.fb.group(controls);
        this.titleService.setTitle(buildTitle(this.Localization.PAGE_TITLE));
    }

    ngAfterViewInit(): void {
        this._subscription = this.form.valueChanges.pipe(
            startWith({}),
            tap(() => this.error = null),
            switchMap((value: IRegistrationPageModel) => {
                return fromEvent<InputEvent>(this.submitBtn.nativeElement, 'click')
                    .pipe(
                        tap(() => this.tapSubmit()),
                        filter(() => this.form.valid),
                        map(() => this.mapRequest(value)),
                        tap(() => this.loaderService.show()),
                        switchMap((request: IRegistrationRequest) =>
                            this.identityService.register(request).pipe(
                                catchError((error) => of(error))
                            ))
                    );
            })
        ).subscribe((response: IRegistrationResponse) => {
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

    private mapRequest(value: IRegistrationPageModel): IRegistrationRequest {
        const request: IRegistrationRequest = {
            Password: value.password,
            ConfirmPassword: value.confirmPassword,
            ReturnUrl: this.returnUrl
        };

        if (isEmail(value.userNameEmail))
            request.Email = value.userNameEmail;
        else
            request.UserName = value.userNameEmail;

        return request;
    }
}