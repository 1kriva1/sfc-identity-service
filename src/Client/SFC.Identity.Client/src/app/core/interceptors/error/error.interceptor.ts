import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { MessageSeverity } from '../../services/message/message-severity.enum';
import { IMessage } from '../../services/message/message.model';
import { MessageService } from '../../services/message/message.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private messageService: MessageService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((errorResponse: HttpErrorResponse) => {
          if (errorResponse.status === 0) {
            // A client-side or network error occurred. Handle it accordingly.
            const message: IMessage = {
              value: `${$localize`:@@core.interceptor.error.error-value-part:An error occurred:`} ${errorResponse.statusText}`,
              title: $localize`:@@core.interceptor.error.error-title:Opps, error occured!`,
              severity: MessageSeverity.ERROR,
              args: errorResponse.error,
              notify: true
            };
            this.messageService.enqueue(message);
          } else {
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong.
            const message: IMessage = {
              value: `Service returned code ${errorResponse.status}.`,
              title: 'Opps, error occured!',
              severity: MessageSeverity.ERROR,
              args: errorResponse.error
            };
            this.messageService.enqueue(message);
          }

          // Return an observable with a user-facing error message.
          return throwError(() => errorResponse.error);
        })
      )
  }
}
