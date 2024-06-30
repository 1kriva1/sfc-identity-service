import { HttpClient, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MessageService } from "../../services/message/message.service";
import { MessageSeverity } from "../../services/message/message-severity.enum";
import { catchError, of } from "rxjs";
import { LoggingInterceptor } from "./logging.interceptor";
import { HttpMethod } from "../../enums";

describe('Core.Interceptor:Logging', () => {
    const startingTime = Date.now(),
        url = '/test';

    let client: HttpClient;
    let controller: HttpTestingController;
    let messageServiceStub: Partial<MessageService> = { enqueue: () => { } };    

    beforeEach(() => {
        jasmine.clock().install();
        jasmine.clock().mockDate(new Date(startingTime));

        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                { provide: MessageService, useValue: messageServiceStub },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: LoggingInterceptor,
                    multi: true
                }
            ]
        });

        client = TestBed.inject(HttpClient);
        controller = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        controller.verify();
        jasmine.clock().uninstall();
    });

    fit('Should add message', (done) => {
        spyOn(messageServiceStub, 'enqueue' as any);

        client.get(url).subscribe(_ => done());

        controller.expectOne(url).flush({});

        expect(messageServiceStub.enqueue).toHaveBeenCalledTimes(1);
    });

    fit('Should add message with valid values', (done) => {
        spyOn(messageServiceStub, 'enqueue' as any);

        const processTime = 1000;

        client.get(url).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        jasmine.clock().mockDate(new Date(startingTime + processTime));

        testRequest.flush({});

        expect(messageServiceStub.enqueue).toHaveBeenCalledOnceWith({
            value: `${HttpMethod.GET} "${url}" in ${processTime} ms.`,
            severity: MessageSeverity.INFO,
            args: testRequest.request
        });
    });

    fit('Should add error message with valid values', (done) => {
        spyOn(messageServiceStub, 'enqueue' as any);

        const processTime = 1000;

        client.get(url).pipe(
            catchError(error => of(error))
        ).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        jasmine.clock().mockDate(new Date(startingTime + processTime));

        testRequest.error(new ProgressEvent('Error'));

        expect(messageServiceStub.enqueue).toHaveBeenCalledOnceWith({
            value: `${HttpMethod.GET} "${url}" in ${processTime} ms.`,
            severity: MessageSeverity.ERROR,
            args: testRequest.request
        });
    });
});