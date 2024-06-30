import { discardPeriodicTasks, fakeAsync, TestBed, tick } from "@angular/core/testing";
import { UntypedFormControl, ValidationErrors } from "@angular/forms";
import { CommonConstants, LoaderService } from "ngx-sfc-common";
import { Observable, of, throwError } from "rxjs";
import { ExistenceService } from "../../../services/existence/existence.service";
import { IExistenceResponse } from "../../../services/existence/models";
import { RegistrationValidators } from "./registration.page.validators";

describe('Features.Identity.Page:Registration.Validators', () => {
    let existenceServiceStub: Partial<ExistenceService> = {},
        loaderServiceStub: Partial<LoaderService> = {
            show: (_?: string, __?: boolean) => { return null; },
            hide: () => { }
        };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [],
            providers: [
                { provide: ExistenceService, useValue: existenceServiceStub },
                { provide: LoaderService, useValue: loaderServiceStub },
            ]
        }).compileComponents();

        existenceServiceStub = TestBed.inject(ExistenceService);
        loaderServiceStub = TestBed.inject(LoaderService);
    });

    describe('UserNameEmail validator', () => {
        fit('Should be invalid', () => {
            const testValue = 'username#',
                validationResult = RegistrationValidators.userNameEmail(new UntypedFormControl(testValue)),
                expectedResult = { sfcUserName: true };

            expect(validationResult).toEqual(expectedResult);
        });

        fit('Should be valid when value is email', () => {
            const testValue = 'username@mail.com',
                validationResult = RegistrationValidators.userNameEmail(new UntypedFormControl(testValue));
            expect(validationResult).toBeNull();
        });

        fit('Should be valid when empty value', () => {
            const testValue = CommonConstants.EMPTY_STRING,
                validationResult = RegistrationValidators.userNameEmail(new UntypedFormControl(testValue));

            expect(validationResult).toBeNull();
        });

        fit('Should be valid', () => {
            const testValue = 'username',
                validationResult = RegistrationValidators.userNameEmail(new UntypedFormControl(testValue));

            expect(validationResult).toBeNull();
        });
    });

    describe('UserNameEmail exist validator', () => {
        fit('Should show loader', fakeAsync(() => {
            spyOn(loaderServiceStub, 'show' as any);

            (existenceServiceStub as any).existByEmail = () => of({
                Exist: false,
                Success: true,
                Message: 'msg'
            } as IExistenceResponse);

            const testValue = 'test@email.com',
                controlId = 'control-id',
                validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                    (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;

            validationResult.subscribe();

            tick(300);

            expect(loaderServiceStub.show).toHaveBeenCalledOnceWith(controlId);

            discardPeriodicTasks();
        }));

        fit('Should hide loader', fakeAsync(() => {
            spyOn(loaderServiceStub, 'hide' as any);

            (existenceServiceStub as any).existByEmail = () => of({
                Exist: false,
                Success: true,
                Message: 'msg'
            } as IExistenceResponse);

            const testValue = 'test@email.com',
                controlId = 'control-id',
                validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                    (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;

            validationResult.subscribe();

            tick(300);

            expect((loaderServiceStub.hide as any).calls.allArgs())
                .toEqual([
                    [controlId],
                    [controlId]
                ]);

            discardPeriodicTasks();
        }));

        fit('Should show and hide loader on error', fakeAsync(() => {
            spyOn(loaderServiceStub, 'show' as any);
            spyOn(loaderServiceStub, 'hide' as any);

            (existenceServiceStub as any).existByEmail = () => throwError(() => new Error('Errror'));

            const testValue = 'test@email.com',
                controlId = 'control-id',
                validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                    (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;

            validationResult.subscribe();

            tick(300);

            expect(loaderServiceStub.show).toHaveBeenCalledOnceWith(controlId);
            expect(loaderServiceStub.hide).toHaveBeenCalledOnceWith(controlId);

            discardPeriodicTasks();
        }));

        fit('Should be invalid if check failed', fakeAsync(() => {
            (existenceServiceStub as any).existByEmail = () => throwError(() => new Error('Errror'));

            const testValue = 'test@email.com',
                controlId = 'control-id',
                validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                    (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>,
                expectedResult = { sfcCheckError: true };

            validationResult.subscribe(result => expect(result).toEqual(expectedResult));

            tick(300);

            discardPeriodicTasks();
        }));

        fit('Should use debounce time', fakeAsync(() => {
            spyOn(loaderServiceStub, 'hide' as any);

            (existenceServiceStub as any).existByUserName = () => {
                return of({
                    Exist: false,
                    Success: true,
                    Message: 'msg'
                } as IExistenceResponse)
            };

            const testValue = 'username',
                controlId = 'control-id',
                validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                    (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;

            validationResult.subscribe();

            expect(loaderServiceStub.hide).not.toHaveBeenCalled();

            tick(100);

            expect(loaderServiceStub.hide).not.toHaveBeenCalled();

            tick(200);

            expect(loaderServiceStub.hide).toHaveBeenCalled();

            discardPeriodicTasks();
        }));

        describe('By UserName', () => {
            fit('Should be invalid', fakeAsync(() => {
                (existenceServiceStub as any).existByUserName = () => {
                    return of({
                        Exist: true,
                        Success: true,
                        Message: 'msg'
                    } as IExistenceResponse)
                };

                const testValue = 'username',
                    controlId = 'control-id',
                    validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                        (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>,
                    expectedResult = { sfcUserNameAlreadyExist: true };                

                validationResult.subscribe(result => expect(result).toEqual(expectedResult));

                tick(300);

                discardPeriodicTasks();
            }));

            fit('Should be valid', fakeAsync(() => {
                (existenceServiceStub as any).existByUserName = () => {
                    return of({
                        Exist: false,
                        Success: true,
                        Message: 'msg'
                    } as IExistenceResponse)
                };

                const testValue = 'username',
                    controlId = 'control-id',
                    validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                        (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;                

                validationResult.subscribe(result => expect(result).toBeNull());

                tick(300);

                discardPeriodicTasks();
            }));
        });

        describe('By email', () => {
            fit('Should be invalid', fakeAsync(() => {
                (existenceServiceStub as any).existByEmail = () => of({
                    Exist: true,
                    Success: true,
                    Message: 'msg'
                } as IExistenceResponse)

                const testValue = 'test@email.com',
                    controlId = 'control-id',
                    validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                        (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>,
                    expectedResult = { sfcEmailAlreadyExist: true };                

                validationResult.subscribe(result => expect(result).toEqual(expectedResult));

                tick(300);

                discardPeriodicTasks();
            }));

            fit('Should be valid', fakeAsync(() => {
                (existenceServiceStub as any).existByEmail = () => of({
                    Exist: false,
                    Success: true,
                    Message: 'msg'
                } as IExistenceResponse)

                const testValue = 'test@email.com',
                    controlId = 'control-id',
                    validationResult = RegistrationValidators.userNameEmailExist(existenceServiceStub as any, loaderServiceStub as any, controlId)
                        (new UntypedFormControl(testValue)) as Observable<ValidationErrors | null>;                

                validationResult.subscribe(result => expect(result).toBeNull());

                tick(300);

                discardPeriodicTasks();
            }));
        });
    });
});