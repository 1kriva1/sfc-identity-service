import { HttpContextToken, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { HttpConstants } from "../../constants";

export const CONTENT_TYPE = new HttpContextToken(() => 'application/json');

@Injectable()
export class ContentTypeInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    if (request.headers.has(HttpConstants.CONTENT_TYPE))
      return next.handle(request);

    const jsonContentTypeRequest = request.clone({
      headers: request.headers.set(HttpConstants.CONTENT_TYPE, request.context.get(CONTENT_TYPE))
    });

    return next.handle(jsonContentTypeRequest);
  }
}
