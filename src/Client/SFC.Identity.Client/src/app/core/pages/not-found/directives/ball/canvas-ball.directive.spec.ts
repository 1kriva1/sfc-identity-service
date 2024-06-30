import { Component, ViewChild } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { CanvasBallDirective } from './canvas-ball.directive';

@Component({
  template: `<canvas sfcCanvasBall [size]="size"></canvas>`
})
class TestCanvasBallDirectiveComponent {

  @ViewChild(CanvasBallDirective, { static: false })
  directive: CanvasBallDirective = <CanvasBallDirective><unknown>null;

  size: number = 1;
}

describe('Core.Page:NotFound.Directive:CanvasBall', () => {
  let component: TestCanvasBallDirectiveComponent;
  let fixture: ComponentFixture<TestCanvasBallDirectiveComponent>;
  let initSpy: jasmine.Spy<any>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CanvasBallDirective, TestCanvasBallDirectiveComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestCanvasBallDirectiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    initSpy = spyOn((component.directive as any), 'init').and.callThrough();
  });

  fit('Should create an instance', () => {
    expect(component.directive).toBeTruthy();
  });

  fit('Should have default size value', () => {
    expect(component.directive.size).toEqual(1);
  });

  fit('Should call init on change size value', () => {
    component.size = 2;
    fixture.detectChanges();

    expect(initSpy).toHaveBeenCalledTimes(1);
  });

  fit('Should call init on click', () => {
    fixture.debugElement.query(By.css('canvas')).nativeElement.dispatchEvent(new MouseEvent('click'));

    expect(initSpy).toHaveBeenCalledTimes(1);
  });
});