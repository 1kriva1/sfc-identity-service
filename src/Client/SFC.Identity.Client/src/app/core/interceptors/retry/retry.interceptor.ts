import { Injectable } from '@angular/core';
import { HttpContextToken, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, retry, throwError, timer } from 'rxjs';
import { hasItem } from 'ngx-sfc-common';

export const RETRY_COUNT = new HttpContextToken(() => 2);
export const RETRY_DELAY = new HttpContextToken(() => 1000);
export const RETRY_STATUSES = new HttpContextToken(() => [500]);

@Injectable()
export class RetryInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const count = request.context.get(RETRY_COUNT),
      delay = request.context.get(RETRY_DELAY),
      statuses = request.context.get(RETRY_STATUSES);

    return next.handle(request).pipe(
      retry({
        count: count,
        delay: (error: HttpErrorResponse) => {
          if (hasItem(statuses, error.status)) {
            return timer(delay);
          }

          return throwError(() => error);
        }
      })
    )
  }
}
