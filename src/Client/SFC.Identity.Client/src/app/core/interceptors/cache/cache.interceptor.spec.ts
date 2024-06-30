import { HttpClient, HttpContext, HttpRequest, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RequestCache } from "../../services/request-cache/request-cache.service";
import { CACHE, CacheInterceptor } from "./cache.interceptor";

describe('Core.Interceptor:Cache', () => {
    const url = '/test';
    let client: HttpClient;
    let controller: HttpTestingController;
    let requestCacheStub: Partial<RequestCache> = { get: (_: HttpRequest<any>) => { return undefined }, put: () => { } };

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                { provide: RequestCache, useValue: requestCacheStub },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: CacheInterceptor,
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

    fit('Should not return cache response by any other http method except GET', (done) => {
        const assertResponse = { value: 'test' },
            cacheResponse = { value: 'cache' };

        (requestCacheStub as any).get = () => cacheResponse;

        client.post(url, {}).subscribe(response => {
            expect(response).toEqual(assertResponse);
            done();
        });

        controller.expectOne(url).flush(assertResponse);
    });

    fit('Should not return cache response by default', (done) => {
        const assertResponse = { value: 'test' },
            cacheResponse = { value: 'cache' };

        (requestCacheStub as any).get = () => cacheResponse;

        client.get(url).subscribe(response => {
            expect(response).toEqual(assertResponse);
            done();
        });

        controller.expectOne(url).flush(assertResponse);
    });

    fit('Should return cache response', (done) => {
        (requestCacheStub as any).get = (_: HttpRequest<any>) => { return { value: 'cache' } };

        client.get(url, { context: new HttpContext().set(CACHE, true) }).subscribe();

        expect(controller.match(url)).toEqual([]);
        done();
    });

    fit('Should save previous response to cache', (done) => {
        spyOn(requestCacheStub, 'put' as any);

        const responseBody = { value: 'test' };

        (requestCacheStub as any).get = () => null;

        client.get(url, { context: new HttpContext().set(CACHE, true) }).subscribe(response => {
            expect(response).toEqual(responseBody);
            done();
        });

        controller.expectOne(url).flush(responseBody, { status: 200, statusText: 'status text' });

        expect(requestCacheStub.put).toHaveBeenCalledTimes(1);
    });
});