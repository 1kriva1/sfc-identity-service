using Duende.IdentityServer;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SFC.Identity.Api.Infrastructure.Authentication;

public class AuthenticationJwtBearerEvents(ILogger<AuthenticationJwtBearerEvents> logger) : JwtBearerEvents
{
    private readonly ILogger<AuthenticationJwtBearerEvents> _logger = logger;

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(),
                "AuthenticationFailed"
        );
        logRequest(_logger, null);

        return base.AuthenticationFailed(context);
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(),
                "MessageReceived"
        );
        logRequest(_logger, null);

        return base.MessageReceived(context);
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(),
                "TokenValidated"
        );
        logRequest(_logger, null);

        return base.TokenValidated(context);
    }

    public override Task Forbidden(ForbiddenContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(),
                "Forbidden"
        );
        logRequest(_logger, null);

        return base.Forbidden(context);
    }
}