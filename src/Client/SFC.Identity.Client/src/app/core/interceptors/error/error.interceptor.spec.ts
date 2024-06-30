import { HttpClient, HTTP_INTERCEPTORS } from "@angular/common/http";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ErrorInterceptor } from "./error.interceptor";
import { MessageService } from "../../services/message/message.service";
import { IMessage } from "../../services/message/message.model";
import { MessageSeverity } from "../../services/message/message-severity.enum";
import { catchError, of } from "rxjs";

describe('Core.Interceptor:Error', () => {
    const url = '/test';
    let client: HttpClient;
    let controller: HttpTestingController;
    let messageServiceStub: Partial<MessageService> = { enqueue: () => { } };

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ],
            providers: [
                { provide: MessageService, useValue: messageServiceStub },
                {
                    provide: HTTP_INTERCEPTORS,
                    useClass: ErrorInterceptor,
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

    fit('Should re-throw error', (done) => {
        const errorOptions = { status: 0, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error');

        client.get(url).pipe(
            catchError((error: any) => {
                expect(error).toEqual(errorEvent);
                return of(error)
            })
        ).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        testRequest.error(errorEvent, errorOptions);
    });

    fit('Should handle network error', (done) => {
        spyOn(messageServiceStub, 'enqueue' as any);

        const errorOptions = { status: 0, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error'),
            messageAssert: IMessage = {
                value: `An error occurred: ${errorOptions.statusText}`,
                title: 'Opps, error occured!',
                severity: MessageSeverity.ERROR,
                args: errorEvent,
                notify: true
            };

        client.get(url).pipe(
            catchError((error: any) => {
                expect(error).toEqual(errorEvent);
                return of(error);
            })
        ).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        testRequest.error(errorEvent, errorOptions);

        expect(messageServiceStub.enqueue).toHaveBeenCalledOnceWith(messageAssert);
    });

    fit('Should handle server error', (done) => {
        spyOn(messageServiceStub, 'enqueue' as any);

        const errorOptions = { status: 404, statusText: 'status text' },
            errorEvent = new ProgressEvent('Error'),
            messageAssert: IMessage = {
                value: `Service returned code ${errorOptions.status}.`,
                title: 'Opps, error occured!',
                severity: MessageSeverity.ERROR,
                args: errorEvent
            };

        client.get(url).pipe(
            catchError((error: any) => {
                expect(error).toEqual(errorEvent);
                return of(error);
            })
        ).subscribe(_ => done());

        const testRequest = controller.expectOne(url);

        testRequest.error(errorEvent, errorOptions);

        expect(messageServiceStub.enqueue).toHaveBeenCalledOnceWith(messageAssert);
    });
});