import { HttpClient, HttpContext, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing'
import { HttpConstants } from "../../constants/http.constants";
import { ContentTypeInterceptor, CONTENT_TYPE } from "./content-type.interceptor";

describe('Core.Interceptor:ContentType', () => {
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
                    useClass: ContentTypeInterceptor,
                    multi: true
                }
            ]
        })

        client = TestBed.inject(HttpClient)
        controller = TestBed.inject(HttpTestingController)
    });

    afterEach(() => {
        controller.verify();
    });

    fit('Should not add application/json type if content type already defined', (done) => {
        const contentType = 'application/xml';

        client.get(url, { headers: { 'Content-Type': contentType } }).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.CONTENT_TYPE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.CONTENT_TYPE)).toEqual(contentType);

        testRequest.flush({});
    });

    fit('Should add default content-type', (done) => {
        client.get(url).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.CONTENT_TYPE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.CONTENT_TYPE)).toEqual('application/json');

        testRequest.flush({});
    });

    fit('Should add defined content-type', (done) => {
        const contentType = 'application/xml';

        client.get(url, { context: new HttpContext().set(CONTENT_TYPE, contentType) }).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        expect(testRequest.request.headers.has(HttpConstants.CONTENT_TYPE)).toBeTrue();
        expect(testRequest.request.headers.get(HttpConstants.CONTENT_TYPE)).toEqual(contentType);

        testRequest.flush({});
    });
})