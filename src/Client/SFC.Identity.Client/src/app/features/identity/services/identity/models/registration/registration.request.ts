export interface IRegistrationRequest {
    UserName?: string;
    Email?: string;
    Password: string;
    ConfirmPassword: string;
    ReturnUrl: string | null;
}