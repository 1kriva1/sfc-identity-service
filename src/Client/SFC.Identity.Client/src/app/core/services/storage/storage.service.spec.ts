import { TestBed } from '@angular/core/testing';
import { StorageService } from './storage.service';

describe('Share.Service:Storage', () => {
    let service: StorageService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(StorageService);

        let store: any = {};
        const mockLocalStorage = {
            getItem: (key: string): string => {
                return key in store ? store[key] : null;
            },
            setItem: (key: string, value: string) => {
                store[key] = `${value}`;
            },
            removeItem: (key: string) => {
                delete store[key];
            }
        };

        spyOn(window.localStorage, 'getItem')
            .and.callFake(mockLocalStorage.getItem);
        spyOn(window.localStorage, 'setItem')
            .and.callFake(mockLocalStorage.setItem);
        spyOn(window.localStorage, 'removeItem')
            .and.callFake(mockLocalStorage.removeItem);
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should set object value', () => {
        const key = 'test',
            value = { value: true };

        service.set('test', value);

        const result: any = JSON.parse(service.get(key) as string);

        expect(result).toEqual(value);
    });

    fit('Should set primitive value', () => {
        const key = 'test',
            value = 123;

        service.set('test', value);

        const result = service.get<string>(key) as string;

        expect(+result).toEqual(value);
    });

    fit('Should get not exist value', () => {
        const key = 'test',
            value = 123;

        service.set('test1', value);

        const result = service.get<string>(key);

        expect(result).toBeNull();
    });

    fit('Should remove value', () => {
        const key = 'test',
            value = 123;

        service.set('test', value);

        let result = service.get<string>(key) as string;

        expect(+result).toEqual(value);

        service.remove(key)

        result = service.get<string>(key) as string;

        expect(result).toBeNull();
    });
});