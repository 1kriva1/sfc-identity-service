import { MessageSeverity } from "../message/message-severity.enum";

export interface INotification {
  value: string;
  severity: MessageSeverity;
  title?: string;
  args?: any;
}
