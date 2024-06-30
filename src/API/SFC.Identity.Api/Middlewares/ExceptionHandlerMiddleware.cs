using SFC.Identity.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Base;
using ExceptionType = System.Exception;

namespace SFC.Identity.Api.Middlewares;

using Handler = Func<ExceptionType, ExceptionResponse>;

internal record struct ExceptionResponse(HttpStatusCode StatusCode, BaseResponse Result) { }

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IDictionary<Type, Handler> _exceptionHandlers;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _exceptionHandlers = new Dictionary<Type, Handler>
        {
            { typeof(ConflictException), HandleConflictException },
            { typeof(IdentityException), HandleIdentityException },
            { typeof(AuthorizationException), HandleAuthorizationException },
            { typeof(ForbiddenException), HandleForbiddenException },
            { typeof(BadRequestException), HandleBadRequestException }
        };
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ExceptionType ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, ExceptionType exception)
    {
        Type exceptionType = exception.GetType();

        ExceptionResponse response = _exceptionHandlers.TryGetValue(exceptionType, out Handler? handler)
            ? handler.Invoke(exception)
            : new(HttpStatusCode.InternalServerError, new BaseResponse(
                Messages.FailedResult,
                false));

        context.Response.StatusCode = (int)response.StatusCode;

        context.Response.ContentType = context.Request.ContentType ?? CommonConstants.CONTENT_TYPE;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response.Result));
    }

    private static ExceptionResponse HandleConflictException(ExceptionType exception)
    {
        return new ExceptionResponse(HttpStatusCode.Conflict, new BaseResponse(exception.Message, false));
    }

    private static ExceptionResponse HandleIdentityException(ExceptionType exception)
    {
        Dictionary<string, IEnumerable<string>> validationErrors = ((IdentityException)exception).Errors;

        return new ExceptionResponse(HttpStatusCode.BadRequest, new BaseErrorResponse(exception.Message, validationErrors));
    }

    private ExceptionResponse HandleAuthorizationException(ExceptionType exception)
    {
        return new ExceptionResponse(HttpStatusCode.Unauthorized, new BaseResponse(exception.Message, false));
    }

    private ExceptionResponse HandleForbiddenException(ExceptionType exception)
    {
        return new ExceptionResponse(HttpStatusCode.Forbidden, new BaseResponse(exception.Message, false));
    }

    private ExceptionResponse HandleBadRequestException(ExceptionType exception)
    {
        Dictionary<string, IEnumerable<string>> validationErrors = ((BadRequestException)exception).Errors;

        return new ExceptionResponse(HttpStatusCode.BadRequest, new BaseErrorResponse(exception.Message, validationErrors));
    }
}
