import { HttpResponse } from "@angular/common/http";

export interface IRequestCacheModel {
    url: string;
    response: HttpResponse<any>;
    lastRead: number;
}