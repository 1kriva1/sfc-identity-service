import { AfterViewInit, Directive, ElementRef, HostListener, Input, OnDestroy, OnInit } from '@angular/core';
import { CanvasBallConstants } from './canvas-ball.constants';
import { ICanvasBallModel } from './canvas-ball.model';

@Directive({
  selector: '[sfcCanvasBall]'
})
export class CanvasBallDirective implements OnInit, AfterViewInit, OnDestroy {

  @Input()
  set size(value: number) {
    this._size = value;
    this.init();
  }
  get size(): number {
    return this._size;
  }
  private _size: number = 1;

  private ballImage = new Image();

  private canvasModel!: ICanvasBallModel;

  private _interval!: NodeJS.Timeout;

  @HostListener('click')
  onClick() {
    this.init();
  }

  constructor(private canvasEl: ElementRef<HTMLCanvasElement>) { }

  ngOnInit(): void {
    this.ballImage.src = CanvasBallConstants.BALL_IMAGE_SRC;
  }

  ngAfterViewInit(): void {
    this.ballImage.onload = () => this._interval = setInterval(this.draw.bind(this), CanvasBallConstants.BALL_ANIMATION_INTERVAL);
  }

  ngOnDestroy(): void {
    clearInterval(this._interval);
  }

  private init() {
    const canvas = this.canvasEl.nativeElement,
      width = this.size * 13.75,
      height = width * 1.75;

    this.canvasModel = {
      canvas: canvas,
      context: canvas.getContext('2d') as CanvasRenderingContext2D,
      width: width,
      height: height,
      radius: width / 3.8,
      vertical: height / 10.5,
      velocity: 0,
      gravity: 0.98,
      mass: 0.9,
      decrement: width * 1.24
    };
  }

  private draw() {
    this.canvasModel.canvas.width = this.canvasModel.width;
    this.canvasModel.canvas.height = this.canvasModel.height;

    this.canvasModel.context.drawImage(this.ballImage, 1, this.canvasModel.vertical, this.canvasModel.width, this.canvasModel.width);

    this.canvasModel.velocity += this.canvasModel.gravity;
    this.canvasModel.vertical += this.canvasModel.velocity;

    if (this.canvasModel.vertical + this.canvasModel.radius / 2 + this.canvasModel.decrement >= this.canvasModel.height) {
      this.canvasModel.vertical = this.canvasModel.height - this.canvasModel.decrement - this.canvasModel.radius / 2 + 2;
      if (this.canvasModel.gravity >= 0.02) {
        this.canvasModel.gravity += -0.02;
        this.canvasModel.velocity *= -this.canvasModel.mass;
      }
      else {
        this.canvasModel.gravity = 0;
        this.canvasModel.velocity = 0;
        this.canvasModel.vertical = this.canvasModel.height - this.canvasModel.decrement - this.canvasModel.radius / 2;
      }
    }
  };
}