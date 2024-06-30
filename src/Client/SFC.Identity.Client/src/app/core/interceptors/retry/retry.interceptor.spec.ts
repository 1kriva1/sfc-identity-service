import { HttpClient, HttpContext, HttpErrorResponse, HTTP_INTERCEPTORS } from "@angular/common/http";
import { fakeAsync, TestBed, tick } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { catchError, of } from "rxjs";
import { RetryInterceptor, RETRY_COUNT, RETRY_DELAY, RETRY_STATUSES } from "./retry.interceptor";

describe('Core.Interceptor:Retry', () => {
    const url = '/test';
    let client: HttpClient;
    let controller: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: RetryInterceptor,
                    multi: true
                }
            ]
        });

        client = TestBed.inject(HttpClient);
        controller = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        controller.verify();
    });

    fit('Should not retry', (done) => {
        const errorOptions = { status: 404, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error');

        client.get(url).pipe(
            catchError((error: HttpErrorResponse) => {
                expect(error.error).toEqual(errorEvent);
                expect(error.status).toEqual(errorOptions.status);
                expect(error.statusText).toEqual(errorOptions.statusText);
                return of(error)
            })
        ).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        testRequest.error(errorEvent, errorOptions);
    });

    fit('Should retry default times', fakeAsync(() => {
        const errorOptions = { status: 500, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error');

        client.get(url).subscribe(
            {
                next: (response) => response,
                error: (errorResponse: HttpErrorResponse) => {
                    expect(errorResponse.error).toEqual(errorEvent);
                    expect(errorResponse.status).toEqual(errorOptions.status);
                    expect(errorResponse.statusText).toEqual(errorOptions.statusText);
                }
            });

        for (let i = 0, c = 3; i < c; i += 1) {
            const request = controller.expectOne(url);
            request.flush(errorEvent, errorOptions);
            tick(1000);
        }
    }));

    fit('Should retry defined times', fakeAsync(() => {
        const errorOptions = { status: 500, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error'),
            retryCount = 5;

        client.get(url, { context: new HttpContext().set(RETRY_COUNT, retryCount) }).subscribe(
            {
                next: (response) => response,
                error: (errorResponse: HttpErrorResponse) => {
                    expect(errorResponse.error).toEqual(errorEvent);
                    expect(errorResponse.status).toEqual(errorOptions.status);
                    expect(errorResponse.statusText).toEqual(errorOptions.statusText);
                }
            });

        for (let i = 0, c = 6; i < c; i += 1) {
            const request = controller.expectOne(url);
            request.flush(errorEvent, errorOptions);
            tick(1000);
        }
    }));

    fit('Should retry with defined statuses', fakeAsync(() => {
        const retryStatus = 400,
            errorOptions = { status: retryStatus, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error');

        client.get(url, { context: new HttpContext().set(RETRY_STATUSES, [retryStatus, 401]) }).subscribe(
            {
                next: (response) => response,
                error: (errorResponse: HttpErrorResponse) => {
                    expect(errorResponse.error).toEqual(errorEvent);
                    expect(errorResponse.status).toEqual(errorOptions.status);
                    expect(errorResponse.statusText).toEqual(errorOptions.statusText);
                }
            });

        for (let i = 0, c = 3; i < c; i += 1) {
            const request = controller.expectOne(url);
            request.flush(errorEvent, errorOptions);
            tick(1000);
        }
    }));

    fit('Should retry with defined delay', fakeAsync(() => {
        const errorOptions = { status: 500, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error'),
            delay = 900;

        client.get(url, { context: new HttpContext().set(RETRY_DELAY, delay) }).subscribe(
            {
                next: (response) => response,
                error: (errorResponse: HttpErrorResponse) => {
                    expect(errorResponse.error).toEqual(errorEvent);
                    expect(errorResponse.status).toEqual(errorOptions.status);
                    expect(errorResponse.statusText).toEqual(errorOptions.statusText);
                }
            });

        for (let i = 0, c = 3; i < c; i += 1) {
            const request = controller.expectOne(url);
            request.flush(errorEvent, errorOptions);
            tick(delay);
        }
    }));
});