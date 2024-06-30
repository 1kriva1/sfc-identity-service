import { Injectable } from '@angular/core';
import { HttpRequest, HttpResponse } from '@angular/common/http';
import { IRequestCacheModel } from './request-cache.model';
import { environment } from '@environments/environment';

export abstract class RequestCache {
    abstract get(request: HttpRequest<any>): HttpResponse<any> | undefined;
    abstract put(request: HttpRequest<any>, response: HttpResponse<any>): void;
}

@Injectable({
    providedIn: 'root'
})
export class RequestCacheWithMap implements RequestCache {

    private cache = new Map<string, IRequestCacheModel>();

    get(request: HttpRequest<any>): HttpResponse<any> | undefined {
        const url = request.urlWithParams;
        const cached = this.cache.get(url);

        if (!cached) {
            return undefined;
        }

        const isExpired = cached.lastRead < (Date.now() - environment.cache_age_ms);
        const expired = isExpired ? 'expired ' : '';
        console.log(`Found ${expired}cached response for "${url}".`);
        return isExpired ? undefined : cached.response;
    }

    put(request: HttpRequest<any>, response: HttpResponse<any>): void {
        const url = request.urlWithParams;
        console.log(`Caching response from "${url}".`);

        const newEntry = { url, response, lastRead: Date.now() };
        this.cache.set(url, newEntry);

        // remove expired cache entries
        const expired = Date.now() - environment.cache_age_ms;
        this.cache.forEach(entry => {
            if (entry.lastRead < expired) {
                this.cache.delete(entry.url);
            }
        });

        console.log(`Request cache size: ${this.cache.size}.`);
    }
}