import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgxSfcCommonModule, Theme } from 'ngx-sfc-common';
import { FooterComponent } from './footer.component';

describe('Core.Component:Footer', () => {
    let component: FooterComponent;
    let fixture: ComponentFixture<FooterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [NgxSfcCommonModule],
            declarations: [FooterComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(FooterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    describe('General', () => {
        fit('Should create page', () => {
            expect(component).toBeTruthy();
        });

        fit('Should have main elements', () => {
            expect(fixture.nativeElement.querySelector('.container')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('sfc-delimeter')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content')).toBeTruthy();
            expect(fixture.nativeElement.querySelector('.content a')).toBeTruthy();
        });
    });

    describe('Content', () => {
        fit('Should have current year value', () => {
            expect(fixture.nativeElement.querySelector('.content .year').innerText).toEqual(`© ${new Date().getFullYear()} The SFC. All Rights Reserved.`);
        });
    });
});