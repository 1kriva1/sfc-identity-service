import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import {
    ButtonType,
    ILoaderEvent, LoaderService,
    NgxSfcCommonModule, Theme, WINDOW
} from 'ngx-sfc-common';
import { NgxSfcComponentsModule } from 'ngx-sfc-components';
import { NgxSfcInputsModule } from 'ngx-sfc-inputs';
import { of } from 'rxjs';
import { IdentityService } from '../../services/identity/identity.service';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { LogoutPageComponent } from './logout.page.component';
import { LogoutPageConstants } from './logout.page.constants';
import { ILogoutResponse } from '../../services/identity/models';
import { By, Title } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';
import { environment } from '@environments/environment';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LogoutState } from './logout-state.enum';
import { ThemeService } from '@core/services';
import { buildTitle } from '@core/utils';

describe('Features.Identity.Page:Logout', () => {
    let component: LogoutPageComponent;
    let fixture: ComponentFixture<LogoutPageComponent>;
    let identityServiceStub: Partial<IdentityService> = {
        logout: (_: string) => { return of(); },
        postLogout: (_: string) => { return of(); }
    }, loaderServiceStub: Partial<LoaderService> = {
        show: (_?: string, __?: boolean) => { return null; },
        hide: () => { },
        register: (_: ILoaderEvent) => { return of(); }
    }, themeServiceStub: Partial<ThemeService> = {
        set: (_?: string, __?: boolean) => { return null; }
    };
    let windowMock: any = <any>{
        location: {
            set href(_: string) { }
        }
    };
    let logoutId = 'logoutId', clientName = 'sfc';

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                ReactiveFormsModule, FontAwesomeModule, HttpClientModule,
                NgxSfcCommonModule, NgxSfcInputsModule, NgxSfcComponentsModule],
            declarations: [LogoutPageComponent],
            providers: [
                { provide: IdentityService, useValue: identityServiceStub },
                { provide: ThemeService, useValue: themeServiceStub },
                { provide: LoaderService, useValue: loaderServiceStub },
                {
                    provide: ActivatedRoute,
                    useValue: {
                        snapshot: {
                            queryParamMap: convertToParamMap({
                                [LogoutPageConstants.LOGOUT_ID_PART_KEY]: logoutId
                            })
                        }
                    }
                },
                { provide: WINDOW, useFactory: (() => { return windowMock; }) }
            ]
        }).compileComponents();

        spyOn(loaderServiceStub, 'show' as any);

        fixture = TestBed.createComponent(LogoutPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    describe('General', () => {
        fit('Should create page', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have default class', () => {
            expect(fixture.nativeElement.className)
                .toContain(LogoutState.Initialization);
        });

        fit('Should show loader on init', () => {
            expect(loaderServiceStub.show).toHaveBeenCalledTimes(1);
        });

        fit('Should have page title', () => {
            const titleService = TestBed.inject(Title);

            expect(titleService.getTitle()).toBe(buildTitle('Logout'));
        });
    });

    describe('Prompt', () => {
        fit('Should exist container', () => {
            spyPromptLogout();

            expect(fixture.nativeElement.querySelector('.prompt-container')).toBeTruthy();
        });

        fit('Should have main elements', () => {
            spyPromptLogout();

            expect(fixture.nativeElement.querySelector('.prompt-container > fa-icon')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.prompt-container > h1')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.prompt-container > .actions')).toBeTruthy();
            expect(fixture.nativeElement.querySelectorAll('.prompt-container > .actions > sfc-button').length).toEqual(2);
        });

        fit('Should have defined icon', () => {
            spyPromptLogout();

            const iconEl = fixture.debugElement.query(By.css('.prompt-container > fa-icon'));

            expect(iconEl.nativeElement.querySelector('svg').classList).toContain('fa-circle-question');
            expect(iconEl.attributes['ng-reflect-custom-size']).toEqual('4');
        });

        fit('Should have defined action buttons', () => {
            spyPromptLogout();

            const actionBtns: DebugElement[] = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'));

            expect(actionBtns[0].componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(actionBtns[0].componentInstance.text).toEqual('Yes');
            expect(actionBtns[1].componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(actionBtns[1].componentInstance.text).toEqual('No');
        });

        fit('Should confirm logout', () => {
            spyPromptLogout();

            const postLogoutSpy = spyLogout('postLogout', true, true, false);

            const actionConfirmBtn: DebugElement = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'))[0];
            actionConfirmBtn.nativeElement.click();
            fixture.changeDetectorRef.detectChanges();

            expect(postLogoutSpy).toHaveBeenCalledOnceWith(logoutId);
        });

        fit('Should hide loader on confirm logout', () => {
            spyPromptLogout();

            spyLogout('postLogout', true, true, false);

            spyOn(loaderServiceStub, 'hide' as any);

            const actionConfirmBtn: DebugElement = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'))[0];
            actionConfirmBtn.nativeElement.click();
            fixture.changeDetectorRef.detectChanges();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        });

        fit('Should redirect container exist on confirm logout', () => {
            spyPromptLogout();

            spyLogout('postLogout', true, false, false, 'https:\\localhost:4200');

            expect(fixture.nativeElement.querySelector('.redirect-container')).toBeNull();

            const actionConfirmBtn: DebugElement = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'))[0];
            actionConfirmBtn.nativeElement.click();
            fixture.changeDetectorRef.detectChanges();

            expect(fixture.nativeElement.querySelector('.redirect-container')).toBeTruthy();
        });

        fit('Should iframe container exist on confirm logout', () => {
            spyPromptLogout();

            spyLogout('postLogout', true, false, false, null, 'https:\\localhost:4200');

            expect(fixture.nativeElement.querySelector('.redirect-container')).toBeNull();

            const actionConfirmBtn: DebugElement = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'))[0];
            actionConfirmBtn.nativeElement.click();
            fixture.changeDetectorRef.detectChanges();

            expect(fixture.nativeElement.querySelector('.iframe-container')).toBeTruthy();
        });

        fit('Should cancel logout', () => {
            spyPromptLogout();

            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set'),
                postLogoutSpy = spyLogout('postLogout', true, true, false);

            const actionCancelBtn: DebugElement = fixture.debugElement.queryAll(By.css('.prompt-container > .actions > sfc-button'))[1];
            actionCancelBtn.nativeElement.click();
            fixture.detectChanges();

            expect(postLogoutSpy).not.toHaveBeenCalled();
            expect(windowsLocationHref).toHaveBeenCalledOnceWith(environment.client_url);
        });

        fit('Should hide loader', () => {
            spyOn(loaderServiceStub, 'hide' as any);

            spyPromptLogout();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        });

        fit('Should set dark theme', () => {
            spyOn(themeServiceStub, 'set' as any);

            spyPromptLogout();

            expect(themeServiceStub.set).toHaveBeenCalledOnceWith(Theme.Dark);
        });
    });

    describe('Redirect', () => {
        fit('Should exist container', () => {
            spyRedirectLogout();

            expect(fixture.nativeElement.querySelector('.redirect-container')).toBeTruthy();
        });

        fit('Should have main elements', () => {
            spyRedirectLogout();

            expect(fixture.nativeElement.querySelector('.redirect-container > fa-icon')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.redirect-container > h1')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.redirect-container > p')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.redirect-container > p > span')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.redirect-container > a')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.redirect-container > a > sfc-button')).toBeTruthy();
        });

        fit('Should have defined icon', () => {
            spyRedirectLogout();

            const iconEl = fixture.debugElement.query(By.css('.redirect-container > fa-icon'));

            expect(iconEl.nativeElement.querySelector('svg').classList).toContain('fa-right-from-bracket');
            expect(iconEl.attributes['ng-reflect-custom-size']).toEqual('4');
        });

        fit('Should have defined title', () => {
            spyRedirectLogout();

            expect(fixture.nativeElement.querySelector('.redirect-container > h1').innerText)
                .toEqual('You are now Logged Out');
        });

        fit('Should have defined description', () => {
            spyRedirectLogout();

            expect(fixture.nativeElement.querySelector('.redirect-container > p').innerText)
                .toEqual('Thank you for using SFC application');
            expect(fixture.nativeElement.querySelector('.redirect-container > p > span.client').innerText)
                .toEqual(clientName.toLocaleUpperCase());
        });

        fit('Should have appropriate reference to redirect', () => {
            const postLogoutRedirectUrl = 'https:\\localhost:4200';
            spyRedirectLogout(postLogoutRedirectUrl);

            expect(fixture.debugElement.query(By.css('.redirect-container > a')).attributes['href'])
                .toEqual(postLogoutRedirectUrl);
        });

        fit('Should have defined redirect button', () => {
            spyRedirectLogout();

            const redirectBtn: DebugElement = fixture.debugElement.query(By.css('.redirect-container > a > sfc-button'));

            expect(redirectBtn.componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(redirectBtn.componentInstance.text).toEqual(`Back to ${clientName.toUpperCase()}`);
        });

        fit('Should hide loader', () => {
            spyOn(loaderServiceStub, 'hide' as any);

            spyRedirectLogout();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        });

        fit('Should set dark theme', () => {
            spyOn(themeServiceStub, 'set' as any);

            spyRedirectLogout();

            expect(themeServiceStub.set).toHaveBeenCalledOnceWith(Theme.Dark);
        });
    });

    describe('IFrame', () => {
        fit('Should exist container', () => {
            spyIFrameLogout();

            expect(fixture.nativeElement.querySelector('.iframe-container')).toBeTruthy();
        });

        fit('Should iframe element exist', () => {
            spyIFrameLogout();

            expect(fixture.nativeElement.querySelector('.iframe-container > iframe')).toBeTruthy();
        });

        fit('Should iframe has defined src', () => {
            spyIFrameLogout();

            expect(fixture.nativeElement.querySelector('.iframe-container > iframe').src).toEqual('https://localhost:4200/');
        });

        fit('Should hide loader', () => {
            spyOn(loaderServiceStub, 'hide' as any);

            spyIFrameLogout();

            expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
        });

        fit('Should set dark theme', () => {
            spyOn(themeServiceStub, 'set' as any);

            spyIFrameLogout();

            expect(themeServiceStub.set).toHaveBeenCalledOnceWith(Theme.Dark);
        });
    });

    describe('Automatic', () => {
        fit('Should exist container', () => {
            spyAutomaticLogout();

            expect(fixture.nativeElement.querySelector('.automatic-container')).toBeTruthy();
        });

        fit('Should have main elements', () => {
            spyAutomaticLogout();

            expect(fixture.nativeElement.querySelector('.automatic-container > fa-icon')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.automatic-container > h1')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.automatic-container > p')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.automatic-container > p > span')).toBeTruthy();
        });

        fit('Should have defined icon', () => {
            spyAutomaticLogout();

            const iconEl = fixture.debugElement.query(By.css('.automatic-container > fa-icon'));

            expect(iconEl.nativeElement.querySelector('svg').classList).toContain('fa-right-from-bracket');
            expect(iconEl.attributes['ng-reflect-custom-size']).toEqual('4');
        });

        fit('Should have defined title', () => {
            spyAutomaticLogout();

            expect(fixture.nativeElement.querySelector('.automatic-container > h1').innerText)
                .toEqual('You are now Logging Out and redirected back to application');
        });

        fit('Should have defined description', () => {
            spyAutomaticLogout();

            expect(fixture.nativeElement.querySelector('.automatic-container > p').innerText)
                .toEqual('Thank you for using SFC application');
            expect(fixture.nativeElement.querySelector('.automatic-container > p > span.client').innerText)
                .toEqual(clientName.toLocaleUpperCase());
        });

        fit('Should not hide loader', () => {
            spyOn(loaderServiceStub, 'hide' as any);

            spyAutomaticLogout();

            expect(loaderServiceStub.hide).not.toHaveBeenCalled();
        });

        fit('Should set default theme', () => {
            spyOn(themeServiceStub, 'set' as any);

            spyAutomaticLogout();

            expect(themeServiceStub.set).toHaveBeenCalledOnceWith(Theme.Default);
        });

        fit('Should redirect to post logout url', () => {
            const windowsLocationHref = spyOnProperty(windowMock.location, 'href', 'set'),
                postLogoutRedirectUrl: string = 'https:\\localhost:4200';

            spyAutomaticLogout(postLogoutRedirectUrl);

            expect(windowsLocationHref).toHaveBeenCalledOnceWith(postLogoutRedirectUrl);
        });
    });

    function spyPromptLogout() {
        return spyLogout('logout', true, true, false);
    }

    function spyRedirectLogout(postLogoutRedirectUrl: string = 'https:\\localhost:4200') {
        return spyLogout('logout', true, false, false, postLogoutRedirectUrl);
    }

    function spyIFrameLogout(signOutIFrameUrl: string = 'https:\\localhost:4200') {
        return spyLogout('logout', true, false, false, null, signOutIFrameUrl);
    }

    function spyAutomaticLogout(postLogoutRedirectUrl: string = 'https:\\localhost:4200') {
        return spyLogout('logout', true, false, true, postLogoutRedirectUrl);
    }

    function spyLogout(method: string, success: boolean = true, showPrompt: boolean,
        automaticRedirectAfterSignOut: boolean, postLogoutRedirectUrl: string | null = null,
        signOutIFrameUrl: string | null = null): jasmine.Spy<any> {
        const spy = spyOn(identityServiceStub, method as any).and.returnValue(of({
            Errors: null,
            Success: success,
            Message: 'msg',
            AutomaticRedirectAfterSignOut: automaticRedirectAfterSignOut,
            ClientName: clientName,
            PostLogoutRedirectUrl: postLogoutRedirectUrl,
            ShowLogoutPrompt: showPrompt,
            SignOutIFrameUrl: signOutIFrameUrl
        } as ILogoutResponse));

        component.ngOnInit();
        fixture.changeDetectorRef.detectChanges();

        return spy;
    }
});