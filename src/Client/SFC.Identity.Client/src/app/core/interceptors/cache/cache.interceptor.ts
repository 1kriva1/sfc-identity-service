import { Injectable } from '@angular/core';
import {
    HttpEvent, HttpRequest, HttpResponse,
    HttpInterceptor, HttpHandler, HttpContextToken
} from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';
import { RequestCache } from '../../services/request-cache/request-cache.service';
import { HttpMethod } from '../../enums/http-method.enum';

export const CACHE = new HttpContextToken(() => false);

/**
 * If request is cacheable and
 * response is in cache return the cached response as observable.
 * If has 'x-refresh' header that is true,
 * then also re-run request, using response from next(),
 * returning an observable that emits the cached response first.
 *
 * If not in cache or not cacheable,
 * pass request through to next()
 */
@Injectable()
export class CacheInterceptor implements HttpInterceptor {

    constructor(private cache: RequestCache) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // continue if not cacheable.
        if (!this.isCacheable(request)) { return next.handle(request); }

        const cachedResponse = this.cache.get(request);

        // cache-or-fetch
        return cachedResponse ?
            of(cachedResponse) : this.sendRequest(request, next, this.cache);
    }

    /** Is this request cacheable? */
    private isCacheable(request: HttpRequest<any>): boolean {
        // Only GET requests are cacheable
        return request.method === HttpMethod.GET &&
            request.context.get(CACHE);
    }

    /**
     * Get server response observable by sending request to `next()`.
     * Will add the response to the cache on the way out.
     */
    private sendRequest(
        request: HttpRequest<any>,
        next: HttpHandler,
        cache: RequestCache): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            tap(event => {
                // There may be other events besides the response.
                if (event instanceof HttpResponse) {
                    cache.put(request, event); // Update the cache.
                }
            })
        );
    }
}