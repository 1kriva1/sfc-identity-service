import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpContextToken,
  HttpResponse,
} from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { LoaderService } from 'ngx-sfc-common';

export const LOADER = new HttpContextToken(() => false);

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {

  constructor(private loaderService: LoaderService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.context.get(LOADER)) {
      this.loaderService.show();

      return next.handle(request).pipe(
        map((event: HttpEvent<any>) => {
          if (event instanceof HttpResponse)
            this.loaderService.hide();

          return event;
        }),
        catchError(error => {
          this.loaderService.hide();
          return throwError(() => error);
        }));
    }

    return next.handle(request);
  }
}
