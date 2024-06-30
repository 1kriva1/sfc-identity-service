import { HttpClient, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing'
import { HttpConstants } from "../../constants/http.constants";
import { LocaleInterceptor } from "./locale.interceptor";
import { CookieService } from "../../services";
import { Locale } from "../../enums";

describe('Core.Interceptor:Locale', () => {
    const url = '/test';
    let client: HttpClient;
    let controller: HttpTestingController;
    let cookieServiceStub: Partial<CookieService> = {
        set: () => { },
        get: () => Locale.English as any
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: LocaleInterceptor,
                    multi: true
                },
                { provide: CookieService, useValue: cookieServiceStub }
            ]
        })

        client = TestBed.inject(HttpClient)
        controller = TestBed.inject(HttpTestingController)
    });

    afterEach(() => {
        controller.verify();
    });

    fit('Should not add accept-language from storage if accept-language already defined', (done) => {
        spyOn(cookieServiceStub, 'get' as any).and.returnValue(Locale.English);

        const acceptLanguage = Locale.Ukraine;

        client.get(url, { headers: { 'Accept-Language': acceptLanguage } }).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.ACCEPT_LANGUAGE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.ACCEPT_LANGUAGE)).toEqual(acceptLanguage);

        testRequest.flush({});
    });

    fit('Should add accept-language from storage', (done) => {
        spyOn(cookieServiceStub, 'get' as any).and.returnValue(Locale.English);

        client.get(url).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.ACCEPT_LANGUAGE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.ACCEPT_LANGUAGE)).toEqual(Locale.English);

        testRequest.flush({});
    });

    fit('Should replace ukraine locale value', (done) => {
        spyOn(cookieServiceStub, 'get' as any).and.returnValue(Locale.Ukraine);

        client.get(url).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.ACCEPT_LANGUAGE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.ACCEPT_LANGUAGE)).toEqual('uk-UA');

        testRequest.flush({});
    });
});