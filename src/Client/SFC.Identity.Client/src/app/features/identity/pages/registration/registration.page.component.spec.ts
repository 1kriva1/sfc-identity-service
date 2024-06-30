import { HttpClientModule } from '@angular/common/http';
import { DebugElement } from '@angular/core';
import { ComponentFixture, discardPeriodicTasks, fakeAsync, flush, TestBed, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By, Title } from '@angular/platform-browser';
import {
    ButtonType, CommonConstants, ComponentSize, Direction, ILoaderEvent, LoaderService,
    nameof,
    NgxSfcCommonModule, UIConstants, WINDOW
} from 'ngx-sfc-common';
import { NgxSfcComponentsModule, SliderType } from 'ngx-sfc-components';
import { NgxSfcInputsModule } from 'ngx-sfc-inputs';
import { of, throwError } from 'rxjs';
import { RoutKey } from '@core/enums';
import { LogoComponent } from '@share/components/logo/logo.component';
import { ExistenceService } from '../../services/existence/existence.service';
import { RegistrationPageComponent } from './registration.page.component';
import { RegistrationPageConstants } from './registration.page.constants';
import { IRegistrationPageModel } from './registration.page.model';
import { buildTitle } from '@core/utils';
import { IdentityService } from '../../services/identity/identity.service';
import { IRegistrationRequest, IRegistrationResponse } from '../../services/identity/models';
import { IExistenceResponse } from '../../services/existence/models';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { IdentityPageConstants } from '../base/identity.page.constants';
import { RouterTestingModule } from '@angular/router/testing';

describe('Features.Identity.Page:Registration', () => {
    let component: RegistrationPageComponent;
    let fixture: ComponentFixture<RegistrationPageComponent>;
    let existenceServiceStub: Partial<ExistenceService> = {},
        identityServiceStub: Partial<IdentityService> = { register: (_: IRegistrationRequest) => { return of(); } },
        loaderServiceStub: Partial<LoaderService> = {
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
                ReactiveFormsModule, HttpClientModule,
                NgxSfcCommonModule, NgxSfcInputsModule,
                NgxSfcComponentsModule, RouterTestingModule.withRoutes([])],
            declarations: [LogoComponent, RegistrationPageComponent],
            providers: [
                { provide: ExistenceService, useValue: existenceServiceStub },
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

        fixture = TestBed.createComponent(RegistrationPageComponent);
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
            expect(fixture.nativeElement.querySelector('sfc-text-input#password')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-text-input#confirm-password')).toBeTruthy();
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

            expect(titleService.getTitle()).toBe(buildTitle('Registration'));
        });

        fit('Should login link has valid value', () => {
            expect(component.loginLinkModel).toEqual({
                link: `/${RoutKey.Identity}/${RoutKey.Login}`,
                params: { [IdentityPageConstants.RETURN_URL_PART_KEY]: returnUrl }
            });
        });
    });

    describe('Left side', () => {
        fit('Should have appropriate attributes for slider', () => {
            const sliderEl: DebugElement = fixture.debugElement.query(By.css('sfc-slider'));

            expect(sliderEl.componentInstance.items).toEqual(RegistrationPageConstants.SLIDER_ITEMS);
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
            const passwordInput: DebugElement = fixture.debugElement.query(By.css('sfc-text-input#password'));

            expect(passwordInput.componentInstance.label).toEqual('Password');
            expect(passwordInput.componentInstance.placeholder).toEqual('Password');
        });

        fit('Should confirm password input have appropriate attributes', () => {
            const passwordInput: DebugElement = fixture.debugElement.query(By.css('sfc-text-input#confirm-password'));

            expect(passwordInput.componentInstance.label).toEqual('Password');
            expect(passwordInput.componentInstance.placeholder).toEqual('Password');
        });

        fit('Should have appropriate attributes for sign up button', () => {
            const registrationBtn: DebugElement = fixture.debugElement.query(By.css('.action > sfc-button'));

            expect(registrationBtn.componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(registrationBtn.componentInstance.text).toEqual('Sign up!');
        });

        fit('Should have appropriate attributes for delimeter', () => {
            const delimeterEl: DebugElement = fixture.debugElement.query(By.css('.action > sfc-delimeter'));

            expect(delimeterEl.componentInstance.label).toEqual('or');
            expect(delimeterEl.componentInstance.direction).toEqual(Direction.Horizontal);
        });

        fit('Should have appropriate href for google sso', () => {
            expect(fixture.nativeElement.querySelector('.sso-link').href).toEqual('https://google.com/');
        });

        fit('Should have appropriate route link for sign in link', () => {
            expect(fixture.debugElement.query(By.css('.redirect a')).attributes['href'])
                .toEqual(`/${RoutKey.Identity}/${RoutKey.Login}?${IdentityPageConstants.RETURN_URL_PART_KEY}=${returnUrl}`);
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
                    confirmPassword: null
                });
            });

            fit('Should have valid autocomplete attribute', () => {
                expect(fixture.nativeElement.querySelector('form').attributes['autocomplete'].nodeValue).toEqual('off');
            });

            fit('Should be invalid', () => {
                expect(component.form.valid).toBeFalse();
            });

            fit('Should be valid', fakeAsync(() => {
                makeFormValid();

                expect(component.form.valid).toBeTrue();

                discardPeriodicTasks();
            }));

            fit('Should submit button be enabled by default', () => {
                expect(fixture.debugElement.query(By.css('sfc-button')).componentInstance.disabled).toBeFalse();
            });

            fit('Should submit button be disabled after click with invalid state', () => {
                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                expect(submitBtnEl.componentInstance.disabled).toBeTrue();
            });

            fit('Should submit button become enabled after make form valid', fakeAsync(() => {
                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                expect(submitBtnEl.componentInstance.disabled).toBeTrue();

                makeFormValid();

                expect(submitBtnEl.componentInstance.disabled).toBeFalse();
            }));

            fit('Should make form controls dirty on submit button click', () => {
                Object.keys(component.form.controls).forEach(key =>
                    expect(component.form.get(key)?.dirty).toBeFalse());

                const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
                submitBtnEl.nativeElement.click();
                fixture.detectChanges();

                Object.keys(component.form.controls).forEach(key =>
                    expect(component.form.get(key)?.dirty).toBeTrue());
            });

            describe('UserNameEmail input', () => {
                fit('Should have bounce loader', () => {
                    expect(fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email ~ sfc-bounce-loader'))
                        .toBeTruthy();
                });

                fit('Should bounce loader has defined attributes', () => {
                    const loader = fixture.debugElement.query(By.css('sfc-text-input .sfc-input#sfc-username-email ~ sfc-bounce-loader'));

                    expect(loader.componentInstance.id).toEqual(RegistrationPageConstants.USERNAME_EMAIL_ID);
                    expect(loader.componentInstance.background).toBeFalse();
                    expect(loader.attributes['ng-reflect-size']).toEqual(ComponentSize.Small);
                });

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

                fit('Should be invalid username', () => {
                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'username#';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect((userNameEmailControl?.errors as any)['sfcUserName']).toBeTrue();
                });

                fit('Should be invalid when username already exist', fakeAsync(() => {
                    (existenceServiceStub as any).existByUserName = () => {
                        return of({
                            Exist: true,
                            Success: true,
                            Message: 'msg'
                        } as IExistenceResponse)
                    };

                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'username';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    tick(300);

                    expect((userNameEmailControl?.errors as any)['sfcUserNameAlreadyExist']).toBeTrue();

                    discardPeriodicTasks();
                }));

                fit('Should be invalid when email already exist', fakeAsync(() => {
                    (existenceServiceStub as any).existByEmail = () => {
                        return of({
                            Exist: true,
                            Success: true,
                            Message: 'msg'
                        } as IExistenceResponse)
                    };

                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'test@mail.com';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    tick(300);

                    expect((userNameEmailControl?.errors as any)['sfcEmailAlreadyExist']).toBeTrue();

                    discardPeriodicTasks();
                }));

                fit('Should be invalid when check existence failed', fakeAsync(() => {
                    (existenceServiceStub as any).existByEmail = () => throwError(() => new Error('Errror'));

                    const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
                        userNameEmailControl = component.form.get('userNameEmail');

                    userNameEmailInputEl.value = 'test@mail.com';
                    userNameEmailInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    tick(300);

                    expect((userNameEmailControl?.errors as any)['sfcCheckError']).toBeTrue();

                    discardPeriodicTasks();
                }));

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

                fit('Should be invalid', () => {
                    const passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password'),
                        passwordControlControl = component.form.get('password');

                    passwordControlInputEl.value = 'pass';
                    passwordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect((passwordControlControl?.errors as any)['pattern']).toBeDefined();
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

            describe('Confirm password input', () => {
                fit('Should have initial validation state', () => {
                    const confirmPasswordControl = component.form.get('confirmPassword');

                    expect(confirmPasswordControl?.errors).not.toBeNull();
                    expect((confirmPasswordControl?.errors as any)['required']).toBeTrue();
                });

                fit('Should change value after input', () => {
                    const confirmPasswordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-confirm-password'),
                        confirmPasswordControlControl = component.form.get('confirmPassword');

                    confirmPasswordControlInputEl.value = 'pass';
                    confirmPasswordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(confirmPasswordControlControl?.value).toEqual(confirmPasswordControlInputEl.value);
                });

                fit('Should be invalid', () => {
                    const confirmPasswordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-confirm-password'),
                        passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password'),
                        confirmPasswordControlControl = component.form.get(nameof<IRegistrationPageModel>('confirmPassword'));

                    passwordControlInputEl.value = 'Test1234!';
                    passwordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    confirmPasswordControlInputEl.value = 'Test12345!';
                    confirmPasswordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect((confirmPasswordControlControl?.errors!)['sfc-match']).toBeTrue();
                });

                fit('Should be valid', () => {
                    const confirmPasswordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-confirm-password'),
                        passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password'),
                        confirmPasswordControlControl = component.form.get('confirmPassword');

                    passwordControlInputEl.value = 'Test1234!';
                    passwordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    confirmPasswordControlInputEl.value = 'Test1234!';
                    confirmPasswordControlInputEl.dispatchEvent(new Event('input'));
                    fixture.detectChanges();

                    expect(confirmPasswordControlControl?.errors).toBeNull();
                });
            });
        });
    });

    describe('Registration process', () => {
        fit('Should not call register if form invalid', () => {
            spyOn(identityServiceStub, 'register' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.register).not.toHaveBeenCalled();
        });

        fit('Should call register if form valid when using email', fakeAsync(() => {
            spyRegister();

            makeFormValid();

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.register).toHaveBeenCalledOnceWith({
                Password: 'Test1234!',
                ConfirmPassword: 'Test1234!',
                Email: 'email@mail.com',
                ReturnUrl: returnUrl
            });

            flush();
        }));

        fit('Should call register if form valid when using username', fakeAsync(() => {
            spyRegister();

            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(identityServiceStub.register).toHaveBeenCalledOnceWith({
                Password: 'Test1234!',
                ConfirmPassword: 'Test1234!',
                UserName: 'username',
                ReturnUrl: returnUrl
            });

            flush();
        }));

        fit('Should show error if register failed', fakeAsync(() => {
            spyRegister(false);

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

            flush();
        }));

        fit('Should clear previous error when flow success', fakeAsync(() => {
            const registerSpy = spyRegister(false);

            makeFormValid();

            const errorsEl = fixture.debugElement.query(By.css('.errors'));

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_VISIBLE);
            expect(errorsEl.styles['opacity']).toEqual('1');

            registerSpy.and.returnValue(of({
                ReturnUrl: 'https:\\localhost:4200',
                Errors: null,
                Success: true,
                Message: 'Success'
            } as IRegistrationResponse));

            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(errorsEl.styles['visibility']).toEqual(UIConstants.CSS_VISIBILITY_HIDDEN);
            expect(errorsEl.styles['opacity']).toEqual('0');

            flush();
        }));

        fit('Should show loader during flow', fakeAsync(() => {
            makeFormValid(false);

            spyOn(loaderServiceStub, 'show' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.show).toHaveBeenCalledTimes(1);
        }));

        fit('Should hide loader when flow not success', fakeAsync(() => {
            spyRegister(false);

            makeFormValid(false);

            spyOn(loaderServiceStub, 'hide' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        }));

        fit('Should not hide loader when flow success', fakeAsync(() => {
            spyRegister(true);

            makeFormValid(false);

            spyOn(loaderServiceStub, 'hide' as any);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(loaderServiceStub.hide).not.toHaveBeenCalled();
        }));

        fit('Should navigate to return url when flow success', fakeAsync(() => {
            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set');

            spyRegister(true);
            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(windowsLocationHref).toHaveBeenCalledOnceWith(returnUrl);
        }));

        fit('Should not navigate to return url when flow not success', fakeAsync(() => {
            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set');

            spyRegister(false);
            makeFormValid(false);

            const submitBtnEl = fixture.debugElement.query(By.css('sfc-button'));
            submitBtnEl.nativeElement.click();
            fixture.detectChanges();

            expect(windowsLocationHref).not.toHaveBeenCalled();
        }));
    });

    function makeFormValid(useEmail = true): void {
        if (useEmail) {
            (existenceServiceStub as any).existByEmail = () => {
                return of({
                    Exist: false,
                    Success: true,
                    Message: 'msg'
                } as IExistenceResponse)
            };
        } else {
            (existenceServiceStub as any).existByUserName = () => {
                return of({
                    Exist: false,
                    Success: true,
                    Message: 'msg'
                } as IExistenceResponse)
            };
        }

        const userNameEmailInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-username-email'),
            confirmPasswordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-confirm-password'),
            passwordControlInputEl = fixture.nativeElement.querySelector('sfc-text-input .sfc-input#sfc-password');

        userNameEmailInputEl.value = useEmail ? 'email@mail.com' : 'username';
        userNameEmailInputEl.dispatchEvent(new Event('input'));
        fixture.detectChanges();

        tick(300);

        passwordControlInputEl.value = 'Test1234!';
        passwordControlInputEl.dispatchEvent(new Event('input'));
        fixture.detectChanges();

        confirmPasswordControlInputEl.value = 'Test1234!';
        confirmPasswordControlInputEl.dispatchEvent(new Event('input'));
        fixture.detectChanges();
    }

    function spyRegister(success: boolean = true): jasmine.Spy<any> {
        return spyOn(identityServiceStub, 'register' as any).and.returnValue(of({
            ReturnUrl: returnUrl,
            Errors: null,
            Success: success,
            Message: 'msg'
        } as IRegistrationResponse));
    }
});