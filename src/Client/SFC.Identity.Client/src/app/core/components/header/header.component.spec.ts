import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { LogoComponent } from '@share/components/logo/logo.component';
import { NgxSfcCommonModule } from 'ngx-sfc-common';
import { HeaderComponent } from './header.component';

describe('Core.Component:Header', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports:[NgxSfcCommonModule],
      declarations: [
        LogoComponent,
        HeaderComponent
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('Should create component', () => {
    expect(component).toBeTruthy();
  });

  fit('Should have main elements', () => {
    expect(fixture.nativeElement.querySelector('.container')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('.container > sfc-logo')).toBeTruthy();
  });

  fit('Should logo have appropriate size', () => {
    const logoEl: DebugElement = fixture.debugElement.query(By.css('.container > sfc-logo'));

    expect(logoEl.attributes['ng-reflect-custom-size']).toEqual('1.3');
    expect(logoEl.componentInstance.showTitle).toBeTrue();
  });
});
