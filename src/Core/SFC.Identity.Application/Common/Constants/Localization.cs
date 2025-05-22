using Microsoft.Extensions.Localization;
using System.Text;

namespace SFC.Identity.Application.Common.Constants;

public class Localization
{
    private const string AUTHORIZATION_ERROR_MESSAGE = "User not found or incorrect password.";
    private const string USER_ALREADY_EXIST_ERROR_MESSAGE = "User already exists.";

    private static IStringLocalizer<Resources> s_localizer = default!;

    public Localization(IStringLocalizer<Resources> localizer)
    {
        s_localizer ??= localizer;
    }

    public static void Configure(IStringLocalizer<Resources> localizer)
    {
        s_localizer = localizer;
    }

    public static string AtLeastOneRequired =>
                    GetValue(s_localizer?.GetString("AtLeastOneRequired"),
                        "Either or both of '{0}' and '{1}' are required.")!;

    public static string AtLeastOneRequiredUnknownProperty =>
                    GetValue(s_localizer?.GetString("AtLeastOneRequiredUnknownProperty"),
                        "Unknown property: '{0}'")!;

    public static string ValidationError =>
                    GetValue(s_localizer?.GetString("ValidationError"),
                        "Validation error.")!;

    public static string RequestBodyRequired =>
                    GetValue(s_localizer?.GetString("RequestBodyRequired"),
                        "Request body is required.")!;

    public static string FailedResult =>
                    GetValue(s_localizer?.GetString("FailedResult"),
                        "Failed result.")!;

    public static string SuccessResult =>
                    GetValue(s_localizer?.GetString("SuccessResult"),
                        "Success result.")!;

    public static string UserAlreadyExist =>
                    GetValue(s_localizer?.GetString("UserAlreadyExist"),
                        USER_ALREADY_EXIST_ERROR_MESSAGE)!;

    public static string UserRegistrationError =>
                    GetValue(s_localizer?.GetString("UserRegistrationError"),
                        "Error occurred during user registration process.")!;

    public static string AuthorizationError =>
                    GetValue(s_localizer?.GetString("AuthorizationError"),
                        AUTHORIZATION_ERROR_MESSAGE)!;

    public static string AccountLocked =>
                    GetValue(s_localizer?.GetString("AccountLocked"),
                        "User account locked out.")!;

    public static string UserNotFound =>
                    GetValue(s_localizer?.GetString("UserNotFound"),
                        "User not found.")!;

    private static string GetValue(LocalizedString? @string, string defaultValue)
    {
        return @string == null
            ? defaultValue
            : @string.ResourceNotFound
            ? defaultValue
            : @string.Value;
    }
}
