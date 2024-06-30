import { Injectable } from '@angular/core';
import { NotificationService } from '../notification/notification.service';
import { MessageSeverity } from './message-severity.enum';
import { IMessage } from './message.model';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private notificationService: NotificationService) { }

  private messages: IMessage[] = [];

  public get count(): number {
    return this.messages.length;
  }

  public enqueue(message: IMessage): void {
    this.logMessage(message);
    this.messages.push(message);

    if (message.notify)
      this.notificationService.notify(message);
  }

  public dequeue(): IMessage | undefined {
    return this.messages.shift();
  }

  public clear(): void {
    this.messages = [];
  }

  private logMessage(message: IMessage): void {
    const messageValue = `${message.severity}: ${message.value}`;

    switch (message.severity) {
      case MessageSeverity.ERROR:
      case MessageSeverity.FATAL:
        console.error(messageValue, message.args);
        break;
      case MessageSeverity.WARNING:
        console.warn(messageValue, message.args);
        break;
      case MessageSeverity.INFO:
        console.info(messageValue, message.args);
        break;
      default:
        console.log(messageValue, message.args);
        break;
    }
  }
}
