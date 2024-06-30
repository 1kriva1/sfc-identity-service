import { BaseResponse } from "./base.response";

export class BaseErrorResponse extends BaseResponse {
  Errors!: [string, string[]] | null;
}
