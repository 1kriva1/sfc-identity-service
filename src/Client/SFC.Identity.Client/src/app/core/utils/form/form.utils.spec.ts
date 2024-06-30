import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { markFormTouchedAndDirty } from "./form.utils";

describe('Core.Utils: Form', () => {
    fit('Should mark control as dirty', () => {
        const builder: FormBuilder = new FormBuilder();
        const group: FormGroup = builder.group({
            field1: [null, [Validators.required]],
            field2: [null]
        });

        markFormTouchedAndDirty(group);

        const field1 = group.get('field1'),
            field2 = group.get('field2');

        expect(field1?.dirty).toBeTrue();
        expect(field2?.dirty).toBeFalse();
    });

    fit('Should mark control group as dirty', () => {
        const builder: FormBuilder = new FormBuilder();
        const group: FormGroup = builder.group({
            field1: [null, [Validators.required]],
            field2: builder.group({
                field3: [null, [Validators.required]]
            })
        });

        markFormTouchedAndDirty(group);

        const field1 = group.get('field1'),
            field3 = group.get('field2')?.get('field3');

        expect(field1?.dirty).toBeTrue();
        expect(field3?.dirty).toBeTrue();
    });
});
