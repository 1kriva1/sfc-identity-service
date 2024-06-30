export class RegistrationPageLocalization {
    static SIGN_UP_BUTTON_TEXT = $localize`:@@feature.identity.registration.page.sign-up:Sign up!`;
    static DELIMETER_TEXT = $localize`:@@feature.identity.registration.page.delimeter:or`;
    static PAGE_TITLE = $localize`:@@feature.identity.registration.page-title:Registration`;
    static INPUT = {
        USERNAME_EMAIL: {
            LABEL_PLACEHOLDER: $localize`:@@feature.identity.registration.page.username-email.label-placeholder:Username or Email`,
            VALIDATIONS: {
                REQUIRED: $localize`:@@feature.identity.registration.page.username-email.validation.required:User name or email is required.`,
                PATTERN: $localize`:@@feature.identity.registration.page.username-email.validation.pattern:User name can only have letters, numbers and special characters -._@+`,
                USERNAME_ALREADY_EXIST: $localize`:@@feature.identity.registration.page.username-email.validation.username-already-exist:User name already exist.`,
                EMAIL_ALREADY_EXIST: $localize`:@@feature.identity.registration.page.username-email.validation.email-already-exist:Email already exist.`,
                CHECK_ERROR: $localize`:@@feature.identity.registration.page.username-email.validation.check-error:Could not check if value already exist.`
            }
        },
        PASSWORD: {
            LABEL_PLACEHOLDER: $localize`:@@feature.identity.registration.page.password.label-placeholder:Password`,
            VALIDATIONS: {
                REQUIRED: $localize`:@@feature.identity.registration.page.password.validation.required:Password required.`,
                PATTERN: $localize`:@@feature.identity.registration.page.password.validation.pattern:Password must have at least 6 characters, one non alphanumeric character, one digit ('0'-'9'), one uppercase ('A'-'Z'), one lowercase ('a'-'z').`
            }
        },
        CONFIRM_PASSWORD: {
            LABEL_PLACEHOLDER: $localize`:@@feature.identity.registration.page.confirm-password.label-placeholder:Password`,
            VALIDATIONS: {
                REQUIRED: $localize`:@@feature.identity.registration.page.confirm-password.validation.required:Password required.`,
                MATCH: $localize`:@@feature.identity.registration.page.confirm-password.validation.match:\'Password\' and \'Confirm password\' do not match.`
            }
        }
    };
}