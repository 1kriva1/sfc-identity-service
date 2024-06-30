import { AbstractControl, AsyncValidatorFn, ValidationErrors } from "@angular/forms";
import { isEmail, isNullOrEmptyString, LoaderService } from "ngx-sfc-common";
import { finalize, map } from 'rxjs/operators';
import { Observable, distinctUntilChanged, timer, switchMap, tap, catchError, of } from 'rxjs';
import { ExistenceService } from "../../../services/existence/existence.service";
import { IExistenceResponse } from "../../../services/existence/models";

export class RegistrationValidators {
    static userNameEmailExist(existenceService: ExistenceService, loaderService: LoaderService, controlId: string): AsyncValidatorFn {
        const debounceTime = 300;

        return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
            const value = control.value,
                isEmailValue = isEmail(value),
                asyncValidationFunc: Observable<IExistenceResponse> = isEmailValue
                    ? existenceService.existByEmail(value)
                    : existenceService.existByUserName(value);

            return timer(debounceTime).pipe(
                tap(() => loaderService.show(controlId)),
                switchMap(() => asyncValidationFunc),
                distinctUntilChanged(),
                map((result: IExistenceResponse) => {
                    loaderService.hide(controlId);
                    const isExist = result.Exist;
                    return result.Exist
                        ? isEmailValue ? { sfcEmailAlreadyExist: isExist } : { sfcUserNameAlreadyExist: isExist }
                        : null;
                }),
                catchError(() => of({ sfcCheckError: true })),
                finalize(() => loaderService.hide(controlId))
            );
        };
    }

    static userNameEmail = (control: AbstractControl): ValidationErrors | null => {
        const value = control.value;

        if (isNullOrEmptyString(value))
            return null;

        if (isEmail(value))
            return null;

        return !/^[a-zA-Z0-9-._@+]+$/.test(value) ? { sfcUserName: true } : null;
    }
}
