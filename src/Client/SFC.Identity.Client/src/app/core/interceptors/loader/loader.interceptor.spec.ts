import { HttpClient, HttpContext, HttpEvent, HttpResponse, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoaderService } from "ngx-sfc-common";
import { LOADER, LoaderInterceptor } from "./loader.interceptor";
import { catchError, of } from "rxjs";

describe('Core.Interceptor:Loader', () => {
    const url = '/test';
    let client: HttpClient;
    let controller: HttpTestingController;
    let loaderServiceStub: Partial<LoaderService> = { show: () => null, hide: () => { } };

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                { provide: LoaderService, useValue: loaderServiceStub },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: LoaderInterceptor,
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

    fit('Should not show loader by default', (done) => {
        spyOn(loaderServiceStub, 'show' as any);

        client.get(url).subscribe(_ => done());

        expect(loaderServiceStub.show).not.toHaveBeenCalled();

        controller.expectOne(url).flush({});
    });

    fit('Should show loader', (done) => {
        spyOn(loaderServiceStub, 'show' as any);

        client.get(url, { context: new HttpContext().set(LOADER, true) }).subscribe(_ => done());

        expect(loaderServiceStub.show).toHaveBeenCalledTimes(1);

        controller.expectOne(url).flush({});
    });

    fit('Should hide loader if show ', (done) => {
        spyOn(loaderServiceStub, 'show' as any);
        spyOn(loaderServiceStub, 'hide' as any);

        client.get(url, { context: new HttpContext().set(LOADER, true) }).subscribe(_ => done());

        expect(loaderServiceStub.show).toHaveBeenCalledTimes(1);

        controller.expectOne(url).flush({});

        expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
    });

    fit('Should hide loader if event is HttpResponse', (done) => {
        spyOn(loaderServiceStub, 'hide' as any);

        client.get(url, { context: new HttpContext().set(LOADER, true) }).subscribe(_ => done());

        controller.expectOne(url).flush({} as HttpResponse<any>);

        expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
    });

    fit('Should not hide loader if event is not HttpResponse', (done) => {
        spyOn(loaderServiceStub, 'hide' as any);

        client.get(url, { context: new HttpContext().set(LOADER, true) }).subscribe(_ => done());

        controller.expectOne(url).event({} as HttpEvent<any>);

        expect(loaderServiceStub.hide).not.toHaveBeenCalled();

        done();
    });

    fit('Should hide loader if error occurred', (done) => {
        spyOn(loaderServiceStub, 'hide' as any);

        client.get(url, { context: new HttpContext().set(LOADER, true) }).pipe(
            catchError(error => of(error))).subscribe(_ => done());

        controller.expectOne(url).error({} as ProgressEvent);

        expect(loaderServiceStub.hide).toHaveBeenCalledTimes(1);
    });

    fit('Should throw error', (done) => {
        spyOn(loaderServiceStub, 'hide' as any);

        const assertError: unknown = { test: 1 };

        client.get(url, { context: new HttpContext().set(LOADER, true) }).pipe(
            catchError(errorResponse => {
                expect(errorResponse).toBeDefined();
                expect(errorResponse.error).toEqual(assertError);
                return of(errorResponse);
            })).subscribe(_ => done());

        controller.expectOne(url).error(assertError as ProgressEvent);
    });
});