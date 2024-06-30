import { HttpInterceptor, HttpRequest, HttpHandler, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize, tap } from 'rxjs/operators';
import { MessageSeverity } from '../../services/message/message-severity.enum';
import { MessageService } from '../../services/message/message.service';

@Injectable()
export class LoggingInterceptor implements HttpInterceptor {

  constructor(private messageService: MessageService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    const started = Date.now();
    let severity: MessageSeverity;

    // extend server response observable with logging
    return next.handle(request)
      .pipe(
        tap({
          // Succeeds when there is a response; ignore other events
          next: (response) => (severity = response instanceof HttpResponse ? MessageSeverity.INFO : MessageSeverity.DEFAULT),
          // Operation failed; error is an HttpErrorResponse
          error: () => (severity = MessageSeverity.ERROR)
        }),
        // Log when response observable either completes or errors
        finalize(() => {
          const elapsed = Date.now() - started,
            msg = `${request.method} "${request.urlWithParams}" in ${elapsed} ms.`;
          this.messageService.enqueue({ value: msg, severity: severity, args: request });
        })
      );
  }
}
