import { HttpClient, HttpContext } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { IdentityServiceConstants } from "./identity.constants";
import { environment } from '@environments/environment';
import {
  ILoginRequest, ILoginResponse, ILogoutResponse,
  IRegistrationRequest, IRegistrationResponse
} from "./models";
import { LOADER } from "@core/interceptors/loader/loader.interceptor";

@Injectable({
  providedIn: 'root'
})
export class IdentityService {

  constructor(private http: HttpClient) { }

  public register(request: IRegistrationRequest): Observable<IRegistrationResponse> {
    return this.http.post<IRegistrationResponse>(
      `${environment.identity_url}${IdentityServiceConstants.URI_PART}/register`,
      request,
      { context: new HttpContext().set(LOADER, false) }
    );
  }

  public login(request: ILoginRequest): Observable<ILoginResponse> {
    return this.http.post<ILoginResponse>(
      `${environment.identity_url}${IdentityServiceConstants.URI_PART}/login`,
      request,
      { context: new HttpContext().set(LOADER, false) }
    );
  }

  public logout(logoutId: string | null): Observable<ILogoutResponse> {
    return this.http.get<ILogoutResponse>(
      `${environment.identity_url}${IdentityServiceConstants.URI_PART}/logout`,
      {
        params: { [IdentityServiceConstants.LOGOUT_ID_PARAM_KEY]: logoutId! },
        context: new HttpContext().set(LOADER, false)
      }
    );
  }

  public postLogout(logoutId: string | null): Observable<ILogoutResponse> {
    return this.http.post<ILogoutResponse>(
      `${environment.identity_url}${IdentityServiceConstants.URI_PART}/logout`,
      null,
      {
        params: { [IdentityServiceConstants.LOGOUT_ID_PARAM_KEY]: logoutId! },
        context: new HttpContext().set(LOADER, false)
      }
    );
  }
}
