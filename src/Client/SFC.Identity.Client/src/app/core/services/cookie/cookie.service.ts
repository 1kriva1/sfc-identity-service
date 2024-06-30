import { Injectable } from '@angular/core';
import { CookieService as Storage } from 'ngx-cookie-service';
import { isNullOrEmptyString, isObject } from 'ngx-sfc-common';
import { CommonConstants } from '../../constants';

@Injectable({
  providedIn: 'root'
})
export class CookieService {

  private readonly DEFAULT_EXPIRES = 1;

  constructor(private cookieService: Storage) { }

  public set<T>(key: string, value: T | null,
    expires: number | Date | undefined = this.DEFAULT_EXPIRES): void {
    this.cookieService.set(this.keyValue(key),
      isObject(value) ? JSON.stringify(value) : `${value}`,
      expires);
  }

  public get<T>(key: string, defaultValue: T | null = null): T | null {
    const value: string | null = this.cookieService.get(this.keyValue(key));
    return isNullOrEmptyString(value) ? defaultValue : value as unknown as T;
  }

  public remove(key: string): void {
    this.cookieService.delete(this.keyValue(key));
  }

  private keyValue(key: string): string {
    return `${CommonConstants.APPLICATION_PREFIX}-${key}`;
  }
}
