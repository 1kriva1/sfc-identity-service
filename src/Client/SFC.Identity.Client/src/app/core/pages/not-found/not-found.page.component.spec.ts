import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { environment } from '@environments/environment';
import { ButtonType, MediaLimits, NgxSfcCommonModule, WINDOW } from 'ngx-sfc-common';
import { CanvasBallDirective } from './directives/ball/canvas-ball.directive';
import { NotFoundPageComponent } from './not-found.page.component';

describe('Core.Page:NotFound', () => {
    let component: NotFoundPageComponent;
    let fixture: ComponentFixture<NotFoundPageComponent>;
    let windowMock: any = <any>{};

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [NgxSfcCommonModule],
            declarations: [CanvasBallDirective, NotFoundPageComponent],
            providers: [
                { provide: WINDOW, useFactory: (() => { return windowMock; }) }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(NotFoundPageComponent);
        component = fixture.componentInstance;
        windowMock.outerWidth = MediaLimits.Laptop;
        fixture.detectChanges();
    });

    describe('General', () => {
        fit('Should create page', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have main elements', () => {
            expect(fixture.nativeElement.querySelector('.container')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.title')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.status-code')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.title h1')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.title .description')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.title sfc-button')).toBeTruthy();
            expect(fixture.nativeElement.querySelectorAll('.status-code .number').length).toEqual(2);
            expect(fixture.nativeElement.querySelector('.status-code canvas')).toBeTruthy();
        });
    });

    describe('Title', () => {
        fit('Should have appropriate attributes for back button', () => {
            const registrationBtn: DebugElement = fixture.debugElement.query(By.css('.title sfc-button'));

            expect(registrationBtn.componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(registrationBtn.componentInstance.text).toEqual('BACK THE GAME');
        });

        fit('Should have appropriate link for back button', () => {
            expect(fixture.debugElement.query(By.css('.title a')).attributes['href'])
                .toEqual(environment.client_url);
        });
    });

    describe('Status code', () => {
        fit('Should have size for more than Tablet devices', () => {
            expect(fixture.debugElement.query(By.css('.status-code')).styles['fontSize'])
                .toEqual('11.9467em');
        });

        fit('Should have size for less or equal Tablet devices', () => {
            windowMock.outerWidth = MediaLimits.Tablet;
            fixture.detectChanges();

            expect(fixture.debugElement.query(By.css('.status-code')).styles['fontSize'])
                .toEqual('11.52em');
        });

        fit('Should have four number', () => {
            fixture.nativeElement.querySelectorAll('.status-code .number')
                .forEach((numberEl: any) => expect(numberEl.innerText).toEqual('4'));
        });

        fit('Should have canvas ball directive', () => {
            expect(fixture.debugElement.query(By.css('.status-code canvas')).attributes.hasOwnProperty('sfcCanvasBall'))
                .toBeTrue();
        });

        fit('Should have canvas ball size for more than Tablet devices', () => {
            expect(fixture.debugElement.query(By.css('.status-code canvas')).attributes['ng-reflect-size'])
                .toEqual('11.946666666666665');
        });

        fit('Should have canvas ball size less or equal Tablet devices', () => {
            windowMock.outerWidth = MediaLimits.Tablet;
            fixture.detectChanges();

            expect(fixture.debugElement.query(By.css('.status-code canvas')).attributes['ng-reflect-size'])
                .toEqual('11.520000000000001');
        });
    });
});