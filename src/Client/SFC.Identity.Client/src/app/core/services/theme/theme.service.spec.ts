import { TestBed } from '@angular/core/testing';
import { Theme } from 'ngx-sfc-common';
import { StorageService } from '../storage/storage.service';
import { ThemeService } from './theme.service';
import { CommonConstants } from '../../constants/common.constants';

describe('Core.Service:Theme', () => {
    let service: ThemeService;
    let storageServiceMock: Partial<StorageService> = {
        set: () => { },
        get: () => Theme.Default as any
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                { provide: StorageService, useValue: storageServiceMock },
            ]
        });
        service = TestBed.inject(ThemeService);
    });

    fit('Should be created', () => {
        expect(service).toBeTruthy();
    });

    fit('Should theme observable be defined', () => {
        expect(service.theme$).toBeDefined();
    });

    fit('Should theme has default value', () => {
        expect(service.theme).toEqual(Theme.Default);
    });

    describe('Toggle', () => {
        fit('Should toggle theme', () => {
            service.toggle();

            expect(service.theme).toEqual(Theme.Dark);

            service.toggle();

            expect(service.theme).toEqual(Theme.Default);
        });

        fit('Should emit on toggle', (done) => {
            service.toggle();

            service.theme$.subscribe((theme: Theme) => {
                expect(theme).toEqual(Theme.Dark);
                done();
            });
        });

        fit('Should call set on storage service', () => {
            spyOn((storageServiceMock as any), 'set').and.callThrough();

            service.toggle();

            expect(storageServiceMock.set).toHaveBeenCalledOnceWith(CommonConstants.THEME_KEY, Theme.Dark);
        });
    });

    describe('Set', () => {
        fit('Should set theme', () => {
            service.set(Theme.Dark);

            expect(service.theme).toEqual(Theme.Dark);

            service.set(Theme.Default);

            expect(service.theme).toEqual(Theme.Default);
        });

        fit('Should emit on set', (done) => {
            service.set(Theme.Dark);

            service.theme$.subscribe((theme: Theme) => {
                expect(theme).toEqual(Theme.Dark);
                done();
            });
        });

        fit('Should call set on storage service', () => {
            spyOn((storageServiceMock as any), 'set').and.callThrough();

            service.set(Theme.Dark);

            expect(storageServiceMock.set).toHaveBeenCalledOnceWith(CommonConstants.THEME_KEY, Theme.Dark);
        });
    });
});