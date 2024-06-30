import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { environment } from '@environments/environment';
import { LogoComponent } from './logo.component';

describe('Share.Component:Logo', () => {
  let component: LogoComponent;
  let fixture: ComponentFixture<LogoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LogoComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(LogoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('Should create component', () => {
    expect(component).toBeTruthy();
  });

  fit('Should have main elements', () => {
    expect(fixture.nativeElement.querySelector('div.container')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('div.container > a')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('div.container > a > img')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('div.container > a > span')).toBeTruthy();
  });

  fit('Should have reference to root', () => {
    expect(fixture.debugElement.query(By.css('div.container > a')).attributes['href'])
                .toEqual(environment.client_url);
  });

  fit('Should have constant image', () => {
    expect(fixture.nativeElement.querySelector('div.container > a > img').src).toContain('app/share/assets/images/logo/bayen-munchen.png');
  });

  fit('Should have constant text', () => {
    expect(fixture.nativeElement.querySelector('div.container > a > span').innerText).toEqual('STREET FOOTBALL CLUB');
  });

  fit('Should hide title', () => {
    component.showTitle = false;
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('div.container > a > span')).toBeNull();
  });
});