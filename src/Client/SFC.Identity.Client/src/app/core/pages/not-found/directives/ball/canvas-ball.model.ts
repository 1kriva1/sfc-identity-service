export interface ICanvasBallModel {
    canvas: HTMLCanvasElement;
    context: CanvasRenderingContext2D;
    width: number;
    height: number;
    radius: number;
    vertical: number;
    velocity: number;
    gravity: number;
    mass: number;
    decrement: number;
}