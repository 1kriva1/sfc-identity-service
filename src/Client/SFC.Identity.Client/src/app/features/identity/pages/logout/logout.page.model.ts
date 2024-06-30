import { empty } from "ngx-sfc-common";

export interface ILogoutPageModel {
    clientName: string | empty;
    postLogoutRedirectUrl: string | empty;
    signOutIFrameUrl: string | empty;
    automaticRedirectAfterSignOut: boolean;
    showLogoutPrompt: boolean;
    buttonText: string;
}