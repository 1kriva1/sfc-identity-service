import { TestBed } from '@angular/core/testing';
import { faCircleExclamation, faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { INotificationContentModel, NotificationType } from 'ngx-sfc-components';
import { MessageSeverity } from '../message/message-severity.enum';
import { IMessage } from '../message/message.model';
import { NotificationService } from '../notification/notification.service';

describe('Core.Service:Notification', () => {
    const startingTime = Date.now();
    let service: NotificationService;

    beforeEach(() => {
        jasmine.clock().install();
        jasmine.clock().mockDate(new Date(startingTime));
        TestBed.configureTestingModule({});
        service = TestBed.inject(NotificationService);
    });

    afterEach(() => {
        jasmine.clock().uninstall();
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should notifications observable be defined', () => {
        expect(service.notifications$).toBeDefined();
    });

    fit('Should notify message', done => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT };

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            expect(notifications.length).toEqual(1);
            expect(service.notifications.length).toEqual(1);
            done();
        });

        service.notify(message);
    });

    fit('Should notify twice', done => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT };
        let assertCount = 0;

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            let length = ++assertCount;
            expect(notifications.length).toEqual(length);
            expect(service.notifications.length).toEqual(length);                
        });

        service.notify(message);

        service.notify(message);

        done();
    });

    fit('Should notify model has valid values for default severity', done => {
        const message: IMessage = { value: 'test', title: 'title', severity: MessageSeverity.DEFAULT };

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            const notificationContentModel: INotificationContentModel = {
                id: startingTime,
                title: message.title,
                subTitle: message.value,
                type: NotificationType.Info,
                icon: faCircleInfo
            };
            expect(notifications[0]).toEqual(notificationContentModel);
            expect(service.notifications[0]).toEqual(notificationContentModel);
            done();
        });

        service.notify(message);
    });

    fit('Should notify model has valid values for info severity', done => {
        const message: IMessage = { value: 'test', title: 'title', severity: MessageSeverity.INFO };

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            const notificationContentModel: INotificationContentModel = {
                id: startingTime,
                title: message.title,
                subTitle: message.value,
                type: NotificationType.Info,
                icon: faCircleInfo
            };
            expect(notifications[0]).toEqual(notificationContentModel);
            expect(service.notifications[0]).toEqual(notificationContentModel);
            done();
        });

        service.notify(message);
    });

    fit('Should notify model has valid values for error severity', done => {
        const message: IMessage = { value: 'test', title: 'title', severity: MessageSeverity.ERROR };

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            const notificationContentModel: INotificationContentModel = {
                id: startingTime,
                title: message.title,
                subTitle: message.value,
                type: NotificationType.Failed,
                icon: faCircleExclamation
            };
            expect(notifications[0]).toEqual(notificationContentModel);
            expect(service.notifications[0]).toEqual(notificationContentModel);
            done();
        });

        service.notify(message);
    });

    fit('Should remove notification', done => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT },
            notificationContentModel: INotificationContentModel = {
                id: startingTime,
                title: message.title,
                subTitle: message.value,
                type: NotificationType.Info,
                icon: faCircleInfo
            };

        let assertCount = 0;

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            let length = ++assertCount;
            expect(notifications.length).toEqual(length);
            expect(service.notifications.length).toEqual(length);           
        });

        expect(service.notifications.length).toEqual(0);

        service.notify(message);

        expect(service.notifications.length).toEqual(1);

        service.notify(message);

        expect(service.notifications.length).toEqual(2);

        service.remove(notificationContentModel);

        expect(service.notifications.length).toEqual(1);

        assertCount = 1;

        service.notify(message);

        done();
    });

    fit('Should not remove notification with wrong id', done => {
        const message: IMessage = { value: 'test', severity: MessageSeverity.DEFAULT },
            notificationContentModel: INotificationContentModel = {
                id: startingTime,
                title: message.title,
                subTitle: message.value,
                type: NotificationType.Info,
                icon: faCircleInfo
            };

        let assertCount = 0;

        service.notifications$.subscribe((notifications: INotificationContentModel[]) => {
            let length = ++assertCount;
            expect(notifications.length).toEqual(length);
            expect(service.notifications.length).toEqual(length);
            done();
        });

        expect(service.notifications.length).toEqual(0);

        service.notify(message);

        expect(service.notifications.length).toEqual(1);

        notificationContentModel.id = 'test';

        service.remove(notificationContentModel);

        expect(service.notifications.length).toEqual(1);
    });
});