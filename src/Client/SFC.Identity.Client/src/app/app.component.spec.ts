import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute, ActivatedRouteSnapshot, ActivationStart, convertToParamMap, Router, RouterEvent } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { CommonConstants, DOCUMENT, NgxSfcCommonModule, Theme, WINDOW } from 'ngx-sfc-common';
import { INotificationContentModel, NgxSfcComponentsModule } from 'ngx-sfc-components';
import { of, Subject } from 'rxjs';
import { AppComponent } from './app.component';
import { FooterComponent, HeaderComponent } from '@core/components';
import { ILayoutModel, IRouteDataModel } from '@core/models';
import { ShareModule } from '@share/share.module';
import { AppComponentConstants } from './app.component.constants';
import { ThemeService, NotificationService } from '@core/services';

describe('Component:Application', () => {
    let routerEventsSubject = new Subject<RouterEvent>();
    let routerStub = { events: routerEventsSubject.asObservable() };
    let component: AppComponent;
    let fixture: ComponentFixture<AppComponent>;
    let notificationServiceStub: Partial<NotificationService> = { remove: (_: INotificationContentModel) => { } };
    let themeServiceStub: Partial<ThemeService> = { theme: Theme.Default };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                RouterTestingModule,
                HttpClientTestingModule,
                NoopAnimationsModule,
                NgxSfcCommonModule,
                NgxSfcComponentsModule,
                ShareModule,
            ],
            declarations: [
                HeaderComponent,
                FooterComponent,
                AppComponent
            ],
            providers: [
                { provide: WINDOW, useValue: {} },
                { provide: DOCUMENT, useValue: window.document },
                { provide: NotificationService, useValue: notificationServiceStub },
                { provide: ThemeService, useValue: themeServiceStub },
                { provide: Router, useValue: routerStub },
                { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({})) } }
            ]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(AppComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => (themeServiceStub as any).theme = Theme.Default);

    describe('General', () => {
        fit('Should create component', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have main elements', () => {
            expect(fixture.nativeElement.querySelector('div.container')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('div.content')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-bounce-loader')).toBeTruthy();
        });

        fit('Should call unsubscribe for layout observable, when component destroyed', () => {
            const layoutUnsubscribeSpy = spyOn(
                (component as any)._subscription,
                'unsubscribe'
            ).and.callThrough();

            component?.ngOnDestroy();

            expect(layoutUnsubscribeSpy).toHaveBeenCalled();
        });
    });

    describe('Theme', () => {
        fit('Should have value by default', () => {
            expect(fixture.nativeElement.className).toContain(Theme.Default);
        });

        fit('Should have default value', () => {
            expect(fixture.nativeElement.className).toContain(Theme.Default);
        });

        fit('Should have defined value', () => {
            (themeServiceStub as any).theme = Theme.Dark;
            fixture.detectChanges();

            expect(fixture.nativeElement.className).toContain(Theme.Dark);
        });

        fit('Should not have value', () => {
            const dataValue: IRouteDataModel = { layout: { footer: true, header: true }, theme: { enabled: false } },
                snapshot: ActivatedRouteSnapshot = ({ data: dataValue } as unknown) as ActivatedRouteSnapshot;

            routerEventsSubject.next(new ActivationStart(snapshot) as any);
            fixture.detectChanges();

            expect(fixture.nativeElement.className).toEqual(CommonConstants.EMPTY_STRING);
        });

        fit('Should have value', () => {
            const dataValue: IRouteDataModel = { layout: { footer: true, header: true }, theme: { enabled: true } },
                snapshot: ActivatedRouteSnapshot = ({ data: dataValue } as unknown) as ActivatedRouteSnapshot;

            routerEventsSubject.next(new ActivationStart(snapshot) as any);
            fixture.detectChanges();

            expect(fixture.nativeElement.className).toContain(Theme.Default);
        });
    });

    describe('Layout', () => {
        fit('Should have default layout model', () => {
            expect(component.layout).toEqual({ header: false, footer: false });
        });

        fit('Should create header', () => {
            expect(fixture.nativeElement.querySelector('sfc-header')).toBeNull();

            component.layout = { header: true, footer: false };
            fixture.detectChanges();

            expect(fixture.nativeElement.querySelector('sfc-header')).toBeTruthy();
        });

        fit('Should create footer', () => {
            expect(fixture.nativeElement.querySelector('sfc-footer')).toBeNull();

            component.layout = { header: false, footer: true };
            fixture.detectChanges();

            expect(fixture.nativeElement.querySelector('sfc-footer')).toBeTruthy();
        });

        fit('Should change layout', () => {
            const newLayout: ILayoutModel = { footer: true, header: true },
                dataValue: IRouteDataModel = { layout: newLayout, theme: { enabled: false } },
                snapshot: ActivatedRouteSnapshot = ({ data: dataValue } as unknown) as ActivatedRouteSnapshot;

            routerEventsSubject.next(new ActivationStart(snapshot) as any);

            expect(component.layout).toEqual(newLayout);
        });
    });

    describe('Notifications', () => {
        fit('Should not exist any notifications', () => {
            expect(fixture.nativeElement.querySelector('div.notifications')).toBeNull();
        });

        fit('Should exist notifications', () => {
            notificationServiceStub.notifications$ = of([
                { id: 'test-id' } as INotificationContentModel
            ]);
            fixture.detectChanges();

            expect(fixture.nativeElement.querySelector('div.notifications')).toBeTruthy();
        });

        fit('Should valid count of notifications', () => {
            notificationServiceStub.notifications$ = of([
                { id: 'test-id-1' } as INotificationContentModel,
                { id: 'test-id-2' } as INotificationContentModel
            ]);
            fixture.detectChanges();

            expect(fixture.nativeElement.querySelectorAll('sfc-notification').length).toEqual(2);
        });

        fit('Should notifications have valid models', () => {
            const notificationsStub = [
                { id: 'test-id-1' } as INotificationContentModel,
                { id: 'test-id-2' } as INotificationContentModel
            ];
            notificationServiceStub.notifications$ = of(notificationsStub);
            fixture.detectChanges();

            fixture.debugElement.queryAll(By.css('sfc-notification')).forEach((el, index) => {
                expect(el.componentInstance.model).toEqual(notificationsStub[index]);
                expect(el.componentInstance.autoCloseModel).toEqual(component.notificationAutoCloseModel);
            });
        });

        fit('Should have default notification model', () => {
            expect(component.notificationAutoCloseModel).toEqual({
                enabled: true,
                interval: AppComponentConstants.NOTIFICATION_AUTO_CLOSE_INTERVAL_MS
            });
        });
    });
});