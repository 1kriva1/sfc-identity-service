using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;
using System.Text.Json;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Base;
using SFC.Identity.Api.Middlewares;
using SystemException = System.Exception;

namespace SFC.Identity.Api.UnitTests.Middlewares;

public class ExceptionHandlerMiddlewareTests
{
    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldHaveDefaultContentType()
    {
        // Arrange
        DefaultHttpContext httpContext = new();

        static Task Next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("Test_error"));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(CommonConstants.CONTENT_TYPE, httpContext.Response.ContentType);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldHaveDefinedContentType()
    {
        // Arrange
        string customContentType = "application/xml";
        DefaultHttpContext httpContext = new();
        httpContext.Request.ContentType = customContentType;

        static Task Next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("Test_error"));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(customContentType, httpContext.Response.ContentType);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldProcessSuccessFlow()
    {
        // Arrange
        DefaultHttpContext httpContext = new();

        static Task Next(HttpContext httpContext) => Task.CompletedTask;

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnInternalServerErrorIfHandlerNotProvided()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        static Task Next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("test_error"));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.InternalServerError, httpContext.Response, Messages.FailedResult, out string _);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnConflict()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "conflict_error";

        Task Next(HttpContext httpContext) => Task.FromException<ConflictException>(new ConflictException(errorMessage));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.Conflict, httpContext.Response, errorMessage, out string _);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnBadRequest()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "bad_request_error", errorCode = "test_code", errorDescription = "test_message";

        Task Next(HttpContext httpContext) => Task.FromException<BadRequestException>(new BadRequestException(errorMessage, (errorCode, errorDescription)));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseErrorResponse(HttpStatusCode.BadRequest, httpContext.Response, errorMessage, errorCode, errorDescription);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnBadRequestForIdentityException()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "bad_request_error", errorCode = "test_code", errorDescription = "test_message";

        Task Next(HttpContext httpContext) => Task.FromException<IdentityException>(new IdentityException(errorMessage, new Dictionary<string, IEnumerable<string>> {
            { errorCode, new List<string>{ errorDescription } }
        }));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseErrorResponse(HttpStatusCode.BadRequest, httpContext.Response, errorMessage, errorCode, errorDescription);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnNotFound()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "not_found_error";

        Task Next(HttpContext httpContext) => Task.FromException<NotFoundException>(new NotFoundException(errorMessage));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.NotFound, httpContext.Response, errorMessage, out string _);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnUnauthorized()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "authorization_error";

        Task Next(HttpContext httpContext) => Task.FromException<AuthorizationException>(new AuthorizationException(errorMessage));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.Unauthorized, httpContext.Response, errorMessage, out string _);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnForbidden()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "forbidden_error";

        Task Next(HttpContext httpContext) => Task.FromException<ForbiddenException>(new ForbiddenException(errorMessage));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.Forbidden, httpContext.Response, errorMessage, out string _);
    }

    [Fact]
    [Trait("API", "Middleware")]
    public async Task API_Middleware_Exception_ShouldReturnBadRequestForJwtException()
    {
        // Arrange
        DefaultHttpContext httpContext = new();
        httpContext.Response.Body = new MemoryStream();

        string errorMessage = "jwt_error";

        Task Next(HttpContext httpContext) => Task.FromException<JwtException>(new JwtException(errorMessage));

        ExceptionHandlerMiddleware middleware = new(Next);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        AssertBaseResponse(HttpStatusCode.BadRequest, httpContext.Response, errorMessage, out string _);
    }

    private void AssertBaseResponse(HttpStatusCode statusCode, HttpResponse httpResponse, string errorMessage, out string responseBody)
    {
        Assert.Equal((int)statusCode, httpResponse.StatusCode);

        httpResponse.Body.Seek(0, SeekOrigin.Begin);

        responseBody = new StreamReader(httpResponse.Body).ReadToEnd();

        Assert.False(string.IsNullOrEmpty(responseBody));

        BaseResponse? response = JsonSerializer.Deserialize<BaseResponse>(responseBody);

        Assert.NotNull(response);
        Assert.Equal(errorMessage, response.Message);
        Assert.False(response.Success);
    }

    private void AssertBaseErrorResponse(HttpStatusCode statusCode, HttpResponse httpResponse, string errorMessage,
        string errorCode, string errorDescription)
    {
        AssertBaseResponse(statusCode, httpResponse, errorMessage, out string responseBody);

        BaseErrorResponse? response = JsonSerializer.Deserialize<BaseErrorResponse>(responseBody);

        Assert.NotNull(response!.Errors);
        Assert.True(response.Errors.ContainsKey(errorCode));
        Assert.Single(response.Errors[errorCode]);
        Assert.Equal(errorDescription, response.Errors[errorCode].FirstOrDefault());
    }
}
