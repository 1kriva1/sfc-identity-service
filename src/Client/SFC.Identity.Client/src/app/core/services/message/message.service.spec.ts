import { TestBed } from '@angular/core/testing';
import { NotificationService } from '../notification/notification.service';
import { MessageSeverity } from './message-severity.enum';
import { IMessage } from './message.model';
import { MessageService } from './message.service';

describe('Core.Service:Message', () => {
    let service: MessageService;
    let notificationServiceStub: Partial<NotificationService> = { notify: (message: IMessage) => { } };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [{ provide: NotificationService, useValue: notificationServiceStub }]
        });
        service = TestBed.inject(MessageService);

        console.log = jasmine.createSpy("log").and.callThrough();
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should enqueue message', () => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT };

        expect(service.count).toEqual(0);

        service.enqueue(message);

        expect(service.count).toEqual(1);
    });

    fit('Should be valid message value', () => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT, notify: false, title: 'title', args: [] };

        service.enqueue(message);

        expect(service.dequeue()).toEqual(message);
    });

    fit('Should output log message on enqueue', () => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT, args: { test: 1 } };

        service.enqueue(message);

        expect(console.log).toHaveBeenCalledWith(`${message.severity}: ${message.value}`, message.args);
    });

    fit('Should output info message on enqueue', () => {
        console.info = jasmine.createSpy("info").and.callThrough();

        const message: IMessage = { value: 'test', severity: MessageSeverity.INFO, args: { test: 1 } };

        service.enqueue(message);

        expect(console.info).toHaveBeenCalledWith(`${message.severity}: ${message.value}`, message.args);
    });

    fit('Should output warn message on enqueue', () => {
        console.warn = jasmine.createSpy("warn").and.callThrough();

        const message: IMessage = { value: 'test', severity: MessageSeverity.WARNING, args: { test: 1 } };

        service.enqueue(message);

        expect(console.warn).toHaveBeenCalledWith(`${message.severity}: ${message.value}`, message.args);
    });

    fit('Should output error message on enqueue', () => {
        console.error = jasmine.createSpy("error").and.callThrough();

        const message: IMessage = { value: 'test', severity: MessageSeverity.ERROR, args: { test: 1 } };

        service.enqueue(message);

        expect(console.error).toHaveBeenCalledWith(`${message.severity}: ${message.value}`, message.args);
    });

    fit('Should call notify', () => {
        spyOn(notificationServiceStub, 'notify' as any);

        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT, notify: true };

        service.enqueue(message);

        expect(notificationServiceStub.notify).toHaveBeenCalledWith(message);
    });

    fit('Should dequeue message', () => {
        const message1: IMessage = { value: 'test1', severity: MessageSeverity.DEFAULT },
            message2: IMessage = { value: 'test2', severity: MessageSeverity.INFO };

        service.enqueue(message1);

        service.enqueue(message2);

        expect(service.count).toEqual(2);

        const messageResult1: IMessage | undefined = service.dequeue(),
            messageResult2: IMessage | undefined = service.dequeue(),
            messageResult3: IMessage | undefined = service.dequeue();

        expect(messageResult1).toEqual(message1);

        expect(messageResult2).toEqual(message2);

        expect(messageResult3).toBeUndefined();
    });

    fit('Should return valid count', () => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT };

        expect(service.count).toEqual(0);

        service.enqueue(message);

        expect(service.count).toEqual(1);

        service.dequeue();

        expect(service.count).toEqual(0);
    });

    fit('Should clear messages', () => {
        const message1: IMessage = { value: 'test1', severity: MessageSeverity.DEFAULT },
            message2: IMessage = { value: 'test2', severity: MessageSeverity.INFO };

        expect(service.count).toEqual(0);

        service.enqueue(message1);

        service.enqueue(message2);

        expect(service.count).toEqual(2);

        service.clear();

        expect(service.count).toEqual(0);
    });
});
