import { ISliderItemModel } from "ngx-sfc-components";

export class RegistrationPageConstants {
    static SLIDER_ITEMS: ISliderItemModel[] = [
        {
            imageSrc: 'app/features/identity/assets/images/registration/slider-default.png'
        },
        {
            imageSrc: 'app/features/identity/assets/images/registration/slider-default.png'
        },
        {
            imageSrc: 'app/features/identity/assets/images/registration/slider-default.png'
        }
    ];
    static PASSWORD_PATTERN = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,}$';
    static USERNAME_EMAIL_ID = 'username-email';
}