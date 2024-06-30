import { HttpClientModule } from '@angular/common/http';
import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By, Title } from '@angular/platform-browser';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import {
    ButtonType,
    CheckmarkType,
    CommonConstants,
    Direction,
    ILoaderEvent,
    LoaderService,
    nameof,
    NgxSfcCommonModule,
    UIConstants,
    WINDOW
} from 'ngx-sfc-common';
import { NgxSfcComponentsModule, SliderType } from 'ngx-sfc-components';
import { NgxSfcInputsModule } from 'ngx-sfc-inputs';
import { of } from 'rxjs';
import { RoutKey } from '@core/enums';
import { LoginPageComponent } from './login.page.component';
import { LoginPageConstants } from './login.page.constants';
import { ILoginPageModel } from './login.page.model';
import { ShareModule } from '@share/share.module';
import { buildTitle } from '@core/utils';
import { IdentityService } from '../../services/identity/identity.service';
import { ILoginRequest, ILoginResponse } from '../../services/identity/models';
import { IdentityPageConstants } from '../base/identity.page.constants';
import { RouterTestingModule } from '@angular/router/testing';

describe('Features.Identity.Page:Login', () => {
    let component: LoginPageComponent;
    let fixture: ComponentFixture<LoginPageComponent>;
    let identityServiceStub: Partial<IdentityService> = {
        login: (_: ILoginRequest) => { return of(); }
    }, loaderServiceStub: Partial<LoaderService> = {
        show: (_?: string, __?: boolean) => { return null; },
        hide: () => { },
        register: (_: ILoaderEvent) => { return of(); }
    };
    let windowMock: any = <any>{
        location: {
            set href(_: string) { }
        }
    };
    let returnUrl = 'localhost:4200';

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                ReactiveFormsModule, HttpClientModule, NgxSfcCommonModule,
                NgxSfcInputsModule, NgxSfcComponentsModule, ShareModule, 
                RouterTestingModule.withRoutes([])
            ],
            declarations: [LoginPageComponent],
            providers: [
                { provide: IdentityService, useValue: identityServiceStub },
                { provide: LoaderService, useValue: loaderServiceStub },
                {
                    provide: ActivatedRoute,
                    useValue: {
                        snapshot: {
                            queryParamMap: convertToParamMap({
                                [IdentityPageConstants.RETURN_URL_PART_KEY]: returnUrl
                            })
                        }
                    }
                },
                { provide: WINDOW, useFactory: (() => { return windowMock; }) }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(LoginPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    describe('General', () => {
        fit('Should create page', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have main elements', () => {
            expect(fixture.nativeElement.querySelector('.container')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.left')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.title')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-slider')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.description')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.right')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-logo')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('form')).toBeTruthy();
            expect(fixture.nativeElement.querySelectorAll('form> .part').length).toEqual(5);
            expect(fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-checkbox-input .sfc-input#sfc-remember')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.errors')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.action')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.action sfc-button')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.action sfc-delimeter')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.action .sso')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.action .redirect')).toBeTruthy();
        });

        fit('Should call unsubscribe', () => {
            const unsubscribeSpy = spyOn(
                (component as any)._subscription,
                'unsubscribe'
            ).and.callThrough();

            component.ngOnDestroy();

            expect(unsubscribeSpy).toHaveBeenCalledTimes(1);
        });

        fit('Should have page title', () => {
            const titleService = TestBed.inject(Title);

            expect(titleService.getTitle()).toBe(buildTitle('Login'));
        });

        fit('Should login link has valid value', () => {
            expect(component.registrationLinkModel).toEqual({
                link: `/${RoutKey.Identity}/${RoutKey.Registration}`,
                params: { [IdentityPageConstants.RETURN_URL_PART_KEY]: returnUrl }
            });
        });
    });

    describe('Left side', () => {
        fit('Should have appropriate attributes for slider', () => {
            const sliderEl: DebugElement = fixture.debugElement.query(By.css('sfc-slider'));

            expect(sliderEl.componentInstance.items).toEqual(LoginPageConstants.SLIDER_ITEMS);
            expect(sliderEl.componentInstance.showCount).toBeFalse();
            expect(sliderEl.componentInstance.showAutomaticToggle).toBeFalse();
            expect(sliderEl.componentInstance.type).toEqual(SliderType.Automatic);
        });
    });

    describe('Right side', () => {
        fit('Should logo have appropriate size', () => {
            expect(fixture.debugElement.query(By.css('sfc-logo')).attributes['ng-reflect-custom-size']).toEqual('1.3');
        });

        fit('Should username or email input have appropriate attributes', () => {
            const userNameEmailInput: DebugElement = fixture.debugElement.query(By.css('sfc-text-input .sfc-input#sfc-username-email'));

            expect(userNameEmailInput.componentInstance.label).toEqual('Username or Email');
            expect(userNameEmailInput.componentInstance.placeholder).toEqual('Username or Email');
        });

        fit('Should password input have appropriate attributes', () => {
            const passwordInput: DebugElement = fixture.debugElement.query(By.css('sfc-text-input .sfc-input#sfc-password'));

            expect(passwordInput.componentInstance.label).toEqual('Password');
            expect(passwordInput.componentInstance.placeholder).toEqual('Password');
        });

        fit('Should remember input have appropriate attributes', () => {
            const passwordInput: DebugElement = fixture.debugElement.query(By.css('sfc-checkbox-input .sfc-input#sfc-remember'));

            expect(passwordInput.componentInstance.sideLabel).toEqual('Remember me!');
            expect(passwordInput.componentInstance.checkmarkType).toEqual(CheckmarkType.Rounded);
        });

        fit('Should have appropriate route link for forgot password link', () => {
            expect(fixture.debugElement.query(By.css('.part.remember a.forgot')).attributes['routerLink'])
                .toEqual(`/${RoutKey.Identity}/${RoutKey.Registration}`);
        });

        fit('Should have appropriate attributes for sign in button', () => {
            const registrationBtn: DebugElement = fixture.debugElement.query(By.css('.action > sfc-button'));

            expect(registrationBtn.componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(registrationBtn.componentInstance.text).toEqual('Sign in!');
        });

        fit('Should have appropriate attributes for delimeter', () => {
            const delimeterEl: DebugElement = fixture.debugElement.query(By.css('.action > sfc-delimeter'));

            expect(delimeterEl.componentInstance.label).toEqual('or');
            expect(delimeterEl.componentInstance.direction).toEqual(Direction.Horizontal);
        });

        fit('Should have appropriate href for google sso', () => {
            expect(fixture.nativeElement.querySelector('.sso-link').href).toEqual('https://google.com/');
        });

        fit('Should have appropriate route link for sign up link', () => {
            expect(fixture.debugElement.query(By.css('.redirect a')).attributes['href'])
                .toEqual(`/${RoutKey.Identity}/${RoutKey.Registration}?${IdentityPageConstants.RETURN_URL_PART_KEY}=${returnUrl}`);
        });

        describe('Form', () => {
            fit('Should have valid inputs count', () => {
                const formEl = fixture.nativeElement.querySelector('form'),
                    inputs = formEl.querySelectorAll('input');

                expect(inputs.length).toEqual(3);
            });

            fit('Should have initial value', () => {
                expect(component.form.value).toEqual({
                    userNameEmail: null,
                    password: null,
                    remember: false
                });
            });

            fit('Should have valid autocomplete attribute', () => {
                expect(fixture.nativeElement.querySelector('form').attributes['autocomplete'].nodeValue).toEqual('off');
            });

            fit('Should be invalid', () => {
                expect(component.form.valid).toBeFalse();
            });

            fit('Should be valid', () => {
                makeFormValid();

                expect(component.form.valid).toBeTrue();
            });

            fit('Should submit button be enabled by default', () => {
                expect(fixture.debugElement.query(By.css('sfc-button')).componentInstance.disabled).toBeFalse();
            });

            fit('Should submit button be disabled after click with invalid state', () => {
                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                expect(submitBtnEl.componentInstance.disabled).toBeTrue();
            });

            fit('Should submit button become enabled after make form valid', () => {
                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                expect(submitBtnEl.componentInstance.disabled).toBeTrue();

                makeFormValid();

                expect(submitBtnEl.componentInstance.disabled).toBeFalse();
            });

            fit('Should make form controls dirty on submit button click', () => {
                const passwordInput = component.form.get(nameof<ILoginPageModel>('password')),
                    userNameEmailInput = component.form.get(nameof<ILoginPageModel>('userNameEmail'));

                expect(passwordInput!.dirty).toBeFalse();
                expect(userNameEmailInput!.dirty).toBeFalse();

                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                expect(passwordInput!.dirty).toBeTrue();
                expect(userNameEmailInput!.dirty).toBeTrue();
            });

            describe('UserNameEmail input', () => {
                fit('Should have initial validation state', () => {
                    const userNameEmailControl = component.form.get('userNameEmail');

                    expect(userNameEmailControl?.errors).not.toBeNull();
                    expect((userNameEmailControl?.errors as any)['required']).toBeTrue();
                });

                fit('Should change value after input', () => {
                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'username';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(userNameEmailControl?.value).toEqual(userNameEmailInputEl.value);
                });

                fit('Should be valid', () => {
                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'username';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(userNameEmailControl?.errors).toBeNull();
                });
            });

            describe('Password input', () => {
                fit('Should have initial validation state', () => {
                    const passwordControl = component.form.get('password');

                    expect(passwordControl?.errors).not.toBeNull();
                    expect((passwordControl?.errors as any)['required']).toBeTrue();
                });

                fit('Should change value after input', () => {
                    const passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password'),
                        passwordControlControl = component.form.get('password');

                    passwordControlInputEl.value = 'pass';
                    passwordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(passwordControlControl?.value).toEqual(passwordControlInputEl.value);
                });

                fit('Should be valid', () => {
                    const passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password'),
                        passwordControlControl = component.form.get('password');

                    passwordControlInputEl.value = 'Test1234!';
                    passwordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(passwordControlControl?.errors).toBeNull();
                });
            });

            describe('Remember input', () => {
                fit('Should have initial validation state', () => {
                    const passwordControl = component.form.get('remember');

                    expect(passwordControl?.errors).toBeNull();
                });

                fit('Should change value after input', () => {
                    const passwordControlInputEl = fixture.debugElement.query(By.css('sfc-checkbox-input .sfc-input#sfc-remember')),
                        passwordControlControl = component.form.get('remember');

                    passwordControlInputEl.triggerEventHandler('input', { target: { nativeElement: passwordControlInputEl.nativeElement, checked: true } });
                    fixture.detectChanges();

                    expect(`${passwordControlControl?.value}`).toEqual(passwordControlInputEl.nativeElement.value);
                });
            });
        });
    });

    describe('Login process', () => {
        fit('Should not call login if form invalid', () => {
            spyOn(identityServiceStub, 'login' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.login).not.toHaveBeenCalled();
        });

        fit('Should call login if form valid when using email', () => {
            spyLogin();

            makeFormValid();

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.login).toHaveBeenCalledOnceWith({ 
                Password: 'Test1234!', 
                RememberMe: true, 
                Email: 'email@mail.com',
                ReturnUrl: returnUrl
            });
        });

        fit('Should call login if form valid when using username', () => {
            spyLogin();

            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.login).toHaveBeenCalledOnceWith({ 
                Password: 'Test1234!', 
                RememberMe: true, 
                UserName: 'username',
                ReturnUrl: returnUrl
            });
        });

        fit('Should show error if login failed', () => {
            spyLogin(false);

            makeFormValid();

            const errorsEl = fixture.debugElement.query(By.css('.errors'));

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_HIDDEN);
            expect(errorsEl.styles['opacity']).toEqual('0');
            expect(fixture.nativeElement.querySelector('.error-message').textContent).toEqual(CommonConstants.EMPTY_STRING);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_VISIBLE);
            expect(errorsEl.styles['opacity']).toEqual('1');
            expect(fixture.nativeElement.querySelector('.error-message').textContent).toEqual('msg');
        });

        fit('Should clear previous error if login success', () => {
            const loginSpy = spyLogin(false);

            makeFormValid();

            const errorsEl = fixture.debugElement.query(By.css('.errors'));

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_VISIBLE);
            expect(errorsEl.styles['opacity']).toEqual('1');

            loginSpy.and.returnValue(of({
                ReturnUrl: 'https:\\localhost:4200',
                Errors: null,
                Success: true,
                Message: 'Success'
            } as ILoginResponse));

            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_HIDDEN);
            expect(errorsEl.styles['opacity']).toEqual('0');
        });

        fit('Should show loader during flow', () => {
            makeFormValid(false);

            spyOn(loaderServiceStub, 'show' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.show).toHaveBeenCalledTimes(1);
        });

        fit('Should hide loader when flow not success', () => {
            spyLogin(false);

            makeFormValid(false);

            spyOn(loaderServiceStub, 'hide' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        });

        fit('Should not hide loader when flow success', () => {
            spyLogin(true);

            makeFormValid(false);

            spyOn(loaderServiceStub, 'hide' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.hide).not.toHaveBeenCalled();
        });

        fit('Should navigate to return url when flow success', () => {
            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set');

            spyLogin(true);
            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(windowsLocationHref).toHaveBeenCalledOnceWith(returnUrl);
        });

        fit('Should not navigate to return url when flow not success', () => {
            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set');

            spyLogin(false);
            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(windowsLocationHref).not.toHaveBeenCalled();
        });
    });

    function makeFormValid(useEmail = true): void {
        const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
            rememberControlInputEl = fixture.debugElement.query(By.css('sfc-checkbox-input .sfc-input#sfc-remember')),
            passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password');

        userNameEmailInputEl.value = useEmail ? 'email@mail.com' : 'username';
        userNameEmailInputEl.dispatchEvent(new Event('input'));
        fixture.detectChanges();

        rememberControlInputEl.triggerEventHandler('input', { target: { nativeElement: rememberControlInputEl.nativeElement, checked: true } });
        fixture.detectChanges();

        passwordControlInputEl.value = 'Test1234!';
        passwordControlInputEl.dispatchEvent(new Event('input'));
        fixture.detectChanges();
    }

    function spyLogin(success: boolean = true): jasmine.Spy<any> {
        return spyOn(identityServiceStub, 'login' as any).and.returnValue(of({
            ReturnUrl: returnUrl,
            Errors: null,
            Success: success,
            Message: 'msg'
        } as ILoginResponse));
    }
});