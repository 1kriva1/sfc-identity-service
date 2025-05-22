using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.Extensions.Logging;

namespace SFC.Identity.Infrastructure.Interceptors.Grpc.Server;
public class ServerLoggingInterceptor(ILogger<ServerLoggingInterceptor> logger) : Interceptor
{
    private readonly ILogger<ServerLoggingInterceptor> _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        LogRequest<TRequest, TResponse>(MethodType.Unary, context);

        TResponse response = await continuation(request, context).ConfigureAwait(false);

        LogResponse<TRequest, TResponse>(MethodType.Unary, context, response);

        return response;
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogRequest<TRequest, TResponse>(MethodType.ClientStreaming, context);
        return base.ClientStreamingServerHandler(requestStream, context, continuation);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogRequest<TRequest, TResponse>(MethodType.ServerStreaming, context);
        return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        LogRequest<TRequest, TResponse>(MethodType.DuplexStreaming, context);
        return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
    }

    private void LogRequest<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Information, new EventId(),
            $"Starting call. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
        logRequest(_logger, null);

        WriteMetadata(context.RequestHeaders, "caller-user");
        WriteMetadata(context.RequestHeaders, "caller-machine");
        WriteMetadata(context.RequestHeaders, "caller-os");

        void WriteMetadata(Metadata headers, string key)
        {
            string headerValue = headers.GetValue(key) ?? "(unknown)";

            Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Information, new EventId(),
                $"{key}: {headerValue}");
            logRequest(_logger, null);
        }
    }

    private void LogResponse<TRequest, TResponse>(MethodType methodType, ServerCallContext context, TResponse response)
        where TRequest : class
        where TResponse : class
    {
        Action<ILogger, Exception?> logRequest = LoggerMessage.Define(LogLevel.Information, new EventId(),
                $"Call completed. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
        logRequest(_logger, null);
    }
}