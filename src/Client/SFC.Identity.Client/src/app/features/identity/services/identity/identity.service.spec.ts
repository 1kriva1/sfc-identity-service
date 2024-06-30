import { HttpContext } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { HttpMethod } from '@core/enums';
import { LOADER } from '@core/interceptors/loader/loader.interceptor';
import { environment } from '@environments/environment';
import { IdentityServiceConstants } from './identity.constants';
import { IdentityService } from './identity.service';
import {
    ILoginRequest, ILoginResponse, ILogoutResponse,
    IRegistrationRequest, IRegistrationResponse
} from './models';

describe('Features.Identity.Service:Identity', () => {
    let service: IdentityService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [
                HttpClientTestingModule,
            ]
        });

        service = TestBed.inject(IdentityService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should register player', () => {
        const request: IRegistrationRequest = {
            UserName: 'username',
            Password: 'pass',
            ConfirmPassword: 'pass',
            ReturnUrl: 'https:\\localhost:4200'
        }, response: IRegistrationResponse = {
            Success: true,
            Errors: null,
            Message: 'msg',
            ReturnUrl: 'https:\\localhost:4200'
        };

        service.register(request).subscribe((registrationResponse: IRegistrationResponse) =>
            expect(registrationResponse).toEqual(response));

        const testRequest = httpMock.expectOne(`${environment.identity_url}${IdentityServiceConstants.URI_PART}/register`);

        expect(testRequest.request.method).toEqual(HttpMethod.POST);
        expect(testRequest.request.body).toEqual(request);
        expect(testRequest.request.context).toEqual(new HttpContext().set(LOADER, false));

        testRequest.flush(response);
    });

    fit('Should login player', () => {
        const request: ILoginRequest = {
            UserName: 'username',
            Password: 'pass',
            RememberMe: true,
            ReturnUrl: 'https:\\localhost:4200'
        }, response: ILoginResponse = {
            Success: true,
            Errors: null,
            Message: 'msg',
            ReturnUrl: 'https:\\localhost:4200'
        };

        service.login(request).subscribe((loginResponse: ILoginResponse) =>
            expect(loginResponse).toEqual(response));

        const testRequest = httpMock.expectOne(`${environment.identity_url}${IdentityServiceConstants.URI_PART}/login`);

        expect(testRequest.request.method).toEqual(HttpMethod.POST);
        expect(testRequest.request.body).toEqual(request);
        expect(testRequest.request.context).toEqual(new HttpContext().set(LOADER, false));

        testRequest.flush(response);
    });

    fit('Should logout player', () => {
        const response: ILogoutResponse = {
            Success: true,
            Errors: null,
            Message: 'msg',
            AutomaticRedirectAfterSignOut: true,
            ClientName: 'sfc',
            PostLogoutRedirectUrl: 'https:\\localhost:4200',
            ShowLogoutPrompt: false,
            SignOutIFrameUrl: 'https:\\localhost:4200'
        }, logoutId = 'logout_id';

        service.logout(logoutId).subscribe((logoutResponse: ILogoutResponse) =>
            expect(logoutResponse).toEqual(response));

        const testRequest = httpMock.expectOne(`${environment.identity_url}${IdentityServiceConstants.URI_PART}/logout?${IdentityServiceConstants.LOGOUT_ID_PARAM_KEY}=${logoutId}`);

        expect(testRequest.request.method).toEqual(HttpMethod.GET);
        expect(testRequest.request.params.get(IdentityServiceConstants.LOGOUT_ID_PARAM_KEY))
            .toEqual(logoutId);
        expect(testRequest.request.context).toEqual(new HttpContext().set(LOADER, false));

        testRequest.flush(response);
    });

    fit('Should post logout player', () => {
        const response: ILogoutResponse = {
            Success: true,
            Errors: null,
            Message: 'msg',
            AutomaticRedirectAfterSignOut: true,
            ClientName: 'sfc',
            PostLogoutRedirectUrl: 'https:\\localhost:4200',
            ShowLogoutPrompt: false,
            SignOutIFrameUrl: 'https:\\localhost:4200'
        }, logoutId = 'logout_id';

        service.postLogout(logoutId).subscribe((logoutResponse: ILogoutResponse) =>
            expect(logoutResponse).toEqual(response));

        const testRequest = httpMock.expectOne(`${environment.identity_url}${IdentityServiceConstants.URI_PART}/logout?${IdentityServiceConstants.LOGOUT_ID_PARAM_KEY}=${logoutId}`);

        expect(testRequest.request.method).toEqual(HttpMethod.POST);
        expect(testRequest.request.body).toBeNull();
        expect(testRequest.request.params.get(IdentityServiceConstants.LOGOUT_ID_PARAM_KEY))
            .toEqual(logoutId);
        expect(testRequest.request.context).toEqual(new HttpContext().set(LOADER, false));

        testRequest.flush(response);
    });
});