import { MessageSeverity } from "./message-severity.enum";

export interface IMessage {
  value: string;
  severity: MessageSeverity;
  title?: string;
  notify?: boolean;
  args?: any;
}
