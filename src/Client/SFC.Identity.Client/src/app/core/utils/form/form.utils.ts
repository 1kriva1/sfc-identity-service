import { AbstractControl, FormGroup } from "@angular/forms";

export function markFormTouchedAndDirty(form: FormGroup): void {
  const showValidationMessages = (control: AbstractControl) => {
    if (control instanceof FormGroup) {
      Object.values(control.controls).forEach(prop =>
        showValidationMessages(prop));
    } else {
      if (control.invalid)
        control.markAsDirty();
    }
  };

  Object.values(form.controls).forEach(control => showValidationMessages(control));
}
