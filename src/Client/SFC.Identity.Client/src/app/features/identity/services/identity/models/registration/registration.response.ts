import { BaseErrorResponse } from "@core/models/http/base-error.response";

export interface IRegistrationResponse extends BaseErrorResponse {
    ReturnUrl: string;
}