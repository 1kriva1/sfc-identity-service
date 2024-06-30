import { BaseErrorResponse } from "@core/models";
import { empty } from "ngx-sfc-common";

export interface ILogoutResponse extends BaseErrorResponse {
    ClientName: string | empty;
    PostLogoutRedirectUrl: string | empty;    
    SignOutIFrameUrl: string | empty;
    AutomaticRedirectAfterSignOut: boolean;
    ShowLogoutPrompt: boolean;
}