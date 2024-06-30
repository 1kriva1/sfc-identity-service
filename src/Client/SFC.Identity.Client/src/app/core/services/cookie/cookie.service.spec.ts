import { TestBed } from '@angular/core/testing';
import { CookieService } from './cookie.service';
import { CookieService as Storage } from 'ngx-cookie-service';
import { CommonConstants } from 'ngx-sfc-common';
import { CommonConstants as ApplicationCommonConstants } from '../../constants';

describe('Core.Service:Cookie', () => {
    let service: CookieService;
    let cookieServiceStub: Partial<Storage> = {
        set: () => { },
        get: (_: string) => CommonConstants.EMPTY_STRING,
        delete: () => { }
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [{ provide: Storage, useValue: cookieServiceStub }]
        });
        service = TestBed.inject(CookieService);
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should set object value', () => {
        spyOn(cookieServiceStub, 'set' as any);

        const key = 'test',
            value = { value: true };

        service.set(key, value);

        expect(cookieServiceStub.set)
            .toHaveBeenCalledWith(
                `${ApplicationCommonConstants.APPLICATION_PREFIX}-${key}`,
                JSON.stringify(value),
                1
            );
    });

    fit('Should set primitive value', () => {
        spyOn(cookieServiceStub, 'set' as any);

        const key = 'test',
            value = 123;

        service.set(key, value);

        expect(cookieServiceStub.set)
            .toHaveBeenCalledWith(
                `${ApplicationCommonConstants.APPLICATION_PREFIX}-${key}`,
                '123',
                1
            );
    });

    fit('Should set value with defined expiration', () => {
        spyOn(cookieServiceStub, 'set' as any);

        const key = 'test',
            value = 123,
            expires = 10;

        service.set(key, value, expires);

        expect(cookieServiceStub.set)
            .toHaveBeenCalledWith(
                `${ApplicationCommonConstants.APPLICATION_PREFIX}-${key}`,
                '123',
                expires
            );
    });

    fit('Should get value', () => {
        const key = 'test', value = '123';

        spyOn(cookieServiceStub, 'get' as any).and.returnValue(value);

        const result = service.get<string>(key);

        expect(result).toEqual(value);
        expect(cookieServiceStub.get)
            .toHaveBeenCalledWith(`${ApplicationCommonConstants.APPLICATION_PREFIX}-${key}`);
    });

    fit('Should get not exist value', () => {
        spyOn(cookieServiceStub, 'get' as any).and.returnValue(null);

        const result = service.get<string>('test');

        expect(result).toBeNull();
    });

    fit('Should get not exist value. but with default value', () => {
        spyOn(cookieServiceStub, 'get' as any).and.returnValue(null);

        const defaultValue = '123';

        const result = service.get<string>('test', defaultValue);

        expect(result).toEqual(defaultValue);
    });

    fit('Should remove value', () => {
        spyOn(cookieServiceStub, 'delete' as any);

        const key = 'test';

        service.remove(key);

        expect(cookieServiceStub.delete)
            .toHaveBeenCalledWith(`${ApplicationCommonConstants.APPLICATION_PREFIX}-${key}`);
    });
});