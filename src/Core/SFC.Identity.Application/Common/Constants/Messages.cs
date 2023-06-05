using Microsoft.Extensions.Localization;
using System.Text;

namespace SFC.Identity.Application.Common.Constants
{
    public class Messages
    {
        private const string AUTHORIZATION_ERROR_MESSAGE = "User not found or incorrect password.";
        private const string USER_ALREADY_EXIST_ERROR_MESSAGE = "User already exists.";
        private const string INVALID_TOKEN_ERROR_MESSAGE = "Invalid token.";

        private static IStringLocalizer<Resources> _localizer = default!;

        public Messages(IStringLocalizer<Resources> localizer)
        {
            _localizer ??= localizer;
        }

        public static void Configure(IStringLocalizer<Resources> localizer)
        {
            _localizer = localizer;
        }

        public static string AtLeastOneRequired =>
                        GetValue(_localizer?.GetString("AtLeastOneRequired"),
                            "Either or both of '{0}' and '{1}' are required.")!;

        public static string AtLeastOneRequiredUnknownProperty =>
                        GetValue(_localizer?.GetString("AtLeastOneRequiredUnknownProperty"),
                            "Unknown property: '{0}'")!;

        public static string ValidationError =>
                        GetValue(_localizer?.GetString("ValidationError"),
                            "Validation error.")!;

        public static string RequestBodyRequired =>
                        GetValue(_localizer?.GetString("RequestBodyRequired"),
                            "Request body is required.")!;

        public static string FailedResult =>
                        GetValue(_localizer?.GetString("FailedResult"),
                            "Failed result.")!;

        public static string SuccessResult =>
                        GetValue(_localizer?.GetString("SuccessResult"),
                            "Success result.")!;

        public static string UserAlreadyExist =>
                        GetValue(_localizer?.GetString("UserAlreadyExist"),
                            USER_ALREADY_EXIST_ERROR_MESSAGE)!;

        public static string UserRegistrationError =>
                        GetValue(_localizer?.GetString("UserRegistrationError"),
                            "Error occurred during user registration process.")!;

        public static string AuthorizationError =>
                        GetValue(_localizer?.GetString("AuthorizationError"),
                            AUTHORIZATION_ERROR_MESSAGE)!;

        public static string AccountLocked =>
                        GetValue(_localizer?.GetString("AccountLocked"),
                            "User account locked out.")!;

        public static string TokenInvalid =>
                        GetValue(_localizer?.GetString("TokenInvalid"),
                            INVALID_TOKEN_ERROR_MESSAGE)!;

        public static string IncorrectTokenError =>
                        GetValue(_localizer?.GetString("IncorrectTokenError"),
                            "User not found or incorrect token.")!;

        public static string UserNotFound =>
                        GetValue(_localizer?.GetString("UserNotFound"),
                            "User not found.")!;

        private static string GetValue(LocalizedString? @string, string defaultValue)
        {
            if (@string == null)
            {
                return defaultValue;
            }

            return @string.ResourceNotFound
                ? defaultValue
                : @string.Value;
        }
    }
}
