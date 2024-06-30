import { BaseErrorResponse } from "@core/models/http/base-error.response";

export interface ILoginResponse extends BaseErrorResponse {
  ReturnUrl: string;
}
