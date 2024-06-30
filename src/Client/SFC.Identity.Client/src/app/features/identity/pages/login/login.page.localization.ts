export class LoginPageLocalization {
  static SIGN_IN_BUTTON_TEXT = $localize`:@@feature.identity.login.page.sign-in:Sign in!`;
  static DELIMETER_TEXT = $localize`:@@feature.identity.login.page.delimeter:or`;
  static PAGE_TITLE = $localize`:@@feature.identity.login.page-title:Login`;
  static INPUT = {
    USERNAME_EMAIL: {
      LABEL_PLACEHOLDER: $localize`:@@feature.identity.login.page.username-email.label-placeholder:Username or Email`,
      VALIDATIONS: {
        REQUIRED: $localize`:@@feature.identity.login.page.username-email.validation.required:User name or email is required.`
      }
    },
    PASSWORD: {
      LABEL_PLACEHOLDER: $localize`:@@feature.identity.login.page.password.label-placeholder:Password`,
      VALIDATIONS: {
        REQUIRED: $localize`:@@feature.identity.login.page.password.validation.required:Password required.`
      }
    },
    REMEMBER: {
      LABEL: $localize`:@@feature.identity.login.page.remember.label:Remember me!`
    }
  };
}
