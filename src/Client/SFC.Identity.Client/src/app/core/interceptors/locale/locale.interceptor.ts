import { HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { HttpConstants } from "../../constants";
import { Locale } from "../../enums";
import { CommonConstants } from "../../constants";
import { CookieService } from "../../services";

@Injectable()
export class LocaleInterceptor implements HttpInterceptor {

  private readonly SERVER_UKRAINE_LOCALE = 'uk-UA';

  constructor(private cookieService: CookieService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    if (request.headers.has(HttpConstants.ACCEPT_LANGUAGE))
      return next.handle(request);

    const userLocale: Locale = this.cookieService.get<Locale>(CommonConstants.LOCALE_KEY) || Locale.English,
      acceptLanguageRequest = request.clone({
        headers: request.headers.set(HttpConstants.ACCEPT_LANGUAGE, userLocale == Locale.Ukraine
          ? this.SERVER_UKRAINE_LOCALE
          : userLocale)
      });

    return next.handle(acceptLanguageRequest);
  }
}
