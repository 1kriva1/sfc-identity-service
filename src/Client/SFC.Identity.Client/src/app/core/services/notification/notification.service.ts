import { Injectable } from '@angular/core';
import { faCircleExclamation, faCircleInfo, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { removeItemBy } from 'ngx-sfc-common';
import { INotificationContentModel, NotificationType } from 'ngx-sfc-components';
import { map, Observable, Subject, tap } from 'rxjs';
import { MessageSeverity } from '../message/message-severity.enum';
import { INotification } from './notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationSubject: Subject<INotification> = new Subject<INotification>();

  public notifications$: Observable<INotificationContentModel[]> = this.notificationSubject.asObservable()
    .pipe(
      map((notification: INotification) => {
        const attriutes = this.getAttributes(notification.severity);
        return {
          id: Date.now(),
          title: notification.title,
          subTitle: notification.value,
          type: attriutes.type,
          icon: attriutes.icon
        } as INotificationContentModel;
      }),
      tap((notification: INotificationContentModel) => this.notifications.push(notification)),
      map(_ => this.notifications)
    );

  public notifications: INotificationContentModel[] = [];

  public notify(notification: INotification): void {
    this.notificationSubject.next(notification);
  }

  public remove(notification: INotificationContentModel): void {
    removeItemBy(this.notifications, (item: INotificationContentModel) => item.id === notification.id);
  }

  private getAttributes(severity: MessageSeverity)
    : { type: NotificationType, icon: IconDefinition } {
    switch (severity) {
      case MessageSeverity.ERROR:
      case MessageSeverity.FATAL:
      case MessageSeverity.WARNING:
        return { type: NotificationType.Failed, icon: faCircleExclamation };
      case MessageSeverity.INFO:
        return { type: NotificationType.Info, icon: faCircleInfo };
      default:
        return { type: NotificationType.Info, icon: faCircleInfo };
    }
  }
}
