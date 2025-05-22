using Google.Protobuf.WellKnownTypes;
using Google.Rpc;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Common.Exceptions;

using Status = Google.Rpc.Status;

namespace SFC.Identity.Infrastructure.Interceptors.Grpc.Server;
public class ServerExceptionInterceptor(ILogger<ServerExceptionInterceptor> logger) : Interceptor
{
    private readonly ILogger<ServerExceptionInterceptor> _logger = logger;

    #region Public

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw HandleExceptionAsync(ex, context);
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw HandleExceptionAsync(ex, context);
        }
    }


    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw HandleExceptionAsync(ex, context);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw HandleExceptionAsync(ex, context);
        }
    }

    #endregion Public

    #region Private

    private RpcException HandleExceptionAsync(Exception exception, ServerCallContext context)
    {
        return exception switch
        {
            AuthorizationException => HandleAuthorizationException((AuthorizationException)exception, context),
            BadRequestException => HandleBadRequestException((BadRequestException)exception, context),
            NotFoundException => HandleNotFoundException((NotFoundException)exception, context),
            _ => HandleDefault(exception, context)
        };
    }

    private RpcException HandleAuthorizationException(AuthorizationException exception, ServerCallContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Warning, new EventId(),
            $"The request does not have valid authentication credentials for the operation.");
        logRequest(_logger, null);

        return BuildRpcException(Code.Unauthenticated, exception.Message);
    }

    private RpcException HandleNotFoundException(NotFoundException exception, ServerCallContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Warning, new EventId(),
            $"Some requested entity (e.g., file or directory) was not found.");
        logRequest(_logger, null);

        return BuildRpcException(Code.NotFound, exception.Message);
    }

    private RpcException HandleBadRequestException(BadRequestException exception, ServerCallContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Warning, new EventId(),
            $"Client specified an invalid argument.");
        logRequest(_logger, null);

        BadRequest badRequest = new();

        IEnumerable<BadRequest.Types.FieldViolation> violations = exception.Errors.SelectMany(error =>
            error.Value.Select(description => new BadRequest.Types.FieldViolation { Field = error.Key, Description = description }));

        badRequest.FieldViolations.Add(violations);

        Status status = BuildStatus(Code.InvalidArgument, exception.Message);

        status.Details.Add(Any.Pack(badRequest));

        return status.ToRpcException();
    }

    private RpcException HandleDefault(Exception exception, ServerCallContext context)
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Error, new EventId(),
            $"Internal errors. Means some invariants expected by underlying system has been broken.");
        logRequest(_logger, null);

        return BuildRpcException(Code.Internal, exception.Message);
    }

    private static Status BuildStatus(Code code, string message)
    {
        Status status = new()
        {
            Code = (int)code,
            Message = message
        };

        return status;
    }

    private static RpcException BuildRpcException(Code code, string message)
    {
        Status status = BuildStatus(code, message);
        return status.ToRpcException();
    }

    /// <summary>
    /// TODO: add corelation id
    /// </summary>
    /// <returns></returns>
    private static Metadata CreateTrailers()
    {
        Metadata trailers = new Metadata();
        return trailers;
    }

    #endregion Private
}