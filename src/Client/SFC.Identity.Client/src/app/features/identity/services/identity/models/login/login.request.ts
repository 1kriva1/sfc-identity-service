export interface ILoginRequest {
  Email?: string;
  UserName?: string;
  Password: string;
  RememberMe: boolean;
  ReturnUrl: string | null;
}
