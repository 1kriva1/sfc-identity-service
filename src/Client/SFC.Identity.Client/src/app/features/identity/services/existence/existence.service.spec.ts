import { HttpContext } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { CACHE } from '@core/interceptors/cache/cache.interceptor';
import { environment } from '@environments/environment';
import { IExistenceResponse } from './models/existence.response';
import { ExistenceService } from './existence.service';

describe('Features.Identity.Service:Existence', () => {
  let service: ExistenceService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(ExistenceService);
  });

  afterEach(() => {
    httpMock.verify();
  });

  fit('Should be created', () => {
    expect(service).toBeTruthy();
  });

  fit('Should call exist by username', (done) => {
    const userName = 'test-username',
      response: IExistenceResponse = {
        Exist: true,
        Success: true,
        Message: 'msg'
      };

    service.existByUserName(userName).subscribe((existenceResponse: IExistenceResponse) => {
      expect(existenceResponse).toEqual(response);
      done();
    });

    const testRequest = httpMock.expectOne(`${environment.identity_url}/api/existence/name/${userName}`);

    expect(testRequest.request.body).toBeNull();
    expect(testRequest.request.context).toEqual(new HttpContext().set(CACHE, true));

    testRequest.flush(response);
  });

  fit('Should call exist by email', (done) => {
    const email = 'test-email',
      response: IExistenceResponse = {
        Exist: true,
        Success: true,
        Message: 'msg'
      };

    service.existByEmail(email).subscribe((existenceResponse: IExistenceResponse) => {
      expect(existenceResponse).toEqual(response);
      done();
    });

    const testRequest = httpMock.expectOne(`${environment.identity_url}/api/existence/email/${email}`);

    expect(testRequest.request.body).toBeNull();
    expect(testRequest.request.context).toEqual(new HttpContext().set(CACHE, true));

    testRequest.flush(response);
  });
});