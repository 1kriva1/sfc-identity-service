import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { environment } from '@environments/environment';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ButtonType, NgxSfcCommonModule } from 'ngx-sfc-common';
import { ErrorPageComponent } from './error.page.component';

describe('Core.Page:Error', () => {
    let component: ErrorPageComponent;
    let fixture: ComponentFixture<ErrorPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FontAwesomeModule, NgxSfcCommonModule],
            declarations: [ErrorPageComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ErrorPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    describe('General', () => {
        fit('Should create page', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have main elements', () => {
            expect(fixture.nativeElement.querySelector('.container')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content > fa-icon')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content > h1')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content > p')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content > a')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content > a > sfc-button')).toBeTruthy();
        });
    });

    describe('Content', () => {
        fit('Should icon has defined attributes', () => {
            const iconEl: DebugElement = fixture.debugElement.query(By.css('.content > fa-icon'));

            expect(iconEl.attributes['ng-reflect-custom-size']).toEqual('4');
            expect(iconEl.nativeElement.querySelector('svg').classList).toContain('fa-circle-xmark');
        });

        fit('Should title has defined value', () => {
            expect(fixture.nativeElement.querySelector('.content > h1').innerText).toEqual('Whoops!');
        });

        fit('Should description has defined value', () => {
            expect(fixture.nativeElement.querySelector('.content > p').innerText).toEqual('We are sorry, but it looks like something unexpected happened');
        });

        fit('Should have appropriate link for back button', () => {
            expect(fixture.debugElement.query(By.css('.content > a')).attributes['href'])
                .toEqual(environment.client_url);
        });

        fit('Should back button has appropriate attributes', () => {
            const backBtn: DebugElement = fixture.debugElement.query(By.css('.content > a > sfc-button'));

            expect(backBtn.componentInstance.types).toEqual([ButtonType.Rounded, ButtonType.Filled]);
            expect(backBtn.componentInstance.text).toEqual(component.BUTTON_BACK);
        });
    });
});