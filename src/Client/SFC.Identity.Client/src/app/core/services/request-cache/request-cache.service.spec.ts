import { HttpRequest, HttpResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { environment } from '@environments/environment';
import { RequestCacheWithMap } from './request-cache.service';

describe('Core.Service:RequestCache', () => {
    const startingTime = Date.now();
    let service: RequestCacheWithMap;

    beforeEach(() => {
        jasmine.clock().install();
        jasmine.clock().mockDate(new Date(startingTime));
        TestBed.configureTestingModule({});
        service = TestBed.inject(RequestCacheWithMap);
    });

    afterEach(() => {
        jasmine.clock().uninstall();
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should not found cache', () => {
        const request: HttpRequest<any> = { urlWithParams: '/test' } as HttpRequest<any>,
            response: HttpResponse<any> | undefined = service.get(request);

        expect(response).toBeUndefined();
    });

    fit('Should found cache', () => {
        const request: HttpRequest<any> = { urlWithParams: '/test' } as HttpRequest<any>,
            response: HttpResponse<any> = { body: { value: 1 } } as HttpResponse<any>;

        service.put(request, response);

        expect(service.get(request)).toBeDefined();
    });

    fit('Should remove expired cache', () => {
        const request1: HttpRequest<any> = { urlWithParams: '/test1' } as HttpRequest<any>,
            request2: HttpRequest<any> = { urlWithParams: '/test2' } as HttpRequest<any>,
            response: HttpResponse<any> = { body: { value: 1 } } as HttpResponse<any>;

        service.put(request1, response);

        expireCache();

        service.put(request2, response);

        const cacheResponse: HttpResponse<any> | undefined = service.get(request1)

        expect(cacheResponse).toBeUndefined();
    });

    fit('Should not found cache if expired', () => {
        const request: HttpRequest<any> = { urlWithParams: '/test' } as HttpRequest<any>,
            response: HttpResponse<any> = { body: { value: 1 } } as HttpResponse<any>;

        service.put(request, response);

        expireCache();

        expect(service.get(request)).toBeUndefined();
    });

    function expireCache() {
        const updatedDate = new Date(startingTime);
        updatedDate.setMilliseconds(updatedDate.getMilliseconds() + environment.cache_age_ms + 1);

        jasmine.clock().mockDate(updatedDate);
    }
});