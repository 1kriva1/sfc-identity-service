import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CACHE } from '@core/interceptors/cache/cache.interceptor';
import { environment } from '@environments/environment';
import { Observable } from 'rxjs';
import { ExistenceServiceConstants } from './existence.constants';
import { IExistenceResponse } from './models/existence.response';

@Injectable({
  providedIn: 'any'
})
export class ExistenceService {

  constructor(private http: HttpClient) { }

  public existByUserName(username: string): Observable<IExistenceResponse> {
    return this.http.get<IExistenceResponse>(
      `${environment.identity_url}${ExistenceServiceConstants.URI_PART}/name/${username}`,
      { context: new HttpContext().set(CACHE, true) }
    );
  }

  public existByEmail(email: string): Observable<IExistenceResponse> {
    return this.http.get<IExistenceResponse>(
      `${environment.identity_url}${ExistenceServiceConstants.URI_PART}/email/${email}`,
      { context: new HttpContext().set(CACHE, true) }
    );
  }
}
