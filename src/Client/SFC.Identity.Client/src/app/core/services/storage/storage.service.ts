import { Injectable } from '@angular/core';
import { isNullOrEmptyString, isObject } from 'ngx-sfc-common';
import { CommonConstants } from '../../constants';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  public set<T>(key: string, value: T | null): void {
    localStorage.setItem(this.keyValue(key),
      isObject(value) ? JSON.stringify(value) : `${value}`);
  }

  public get<T>(key: string, defaultValue: T | null = null): T | null {
    const value: string | null = localStorage.getItem(this.keyValue(key));
    return isNullOrEmptyString(value) ? defaultValue : value as T | null;
  }

  public remove(key: string): void {
    localStorage.removeItem(this.keyValue(key));
  }

  private keyValue(key: string): string {
    return `${CommonConstants.APPLICATION_PREFIX}-${key}`;
  }
}
