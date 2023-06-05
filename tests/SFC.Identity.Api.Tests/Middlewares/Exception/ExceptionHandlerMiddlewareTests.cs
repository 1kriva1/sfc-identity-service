using Microsoft.AspNetCore.Http;
using SFC.Identity.Api.Middlewares.Exception;
using System.Net;
using SystemException = System.Exception;
using Xunit;
using System.Text.Json;
using SFC.Identity.Application.Common.Models;
using SFC.Identity.Application.Common.Exceptions;

namespace SFC.Identity.Api.Tests.Middlewares.Exception
{
    public class ExceptionHandlerMiddlewareTests
    {
        [Fact]
        [Trait("Exception", "ContentType")]
        public async Task ExceptionHandlerMiddleware_ContentType_ShouldHaveDefaultContentType()
        {
            // Arrange
            DefaultHttpContext httpContext = new();

            static Task next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("Test_error"));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal("application/json", httpContext.Response.ContentType);
        }

        [Fact]
        [Trait("Exception", "ContentType")]
        public async Task ExceptionHandlerMiddleware_ContentType_ShouldHaveDefinedContentType()
        {
            // Arrange
            string customContentType = "application/xml";
            DefaultHttpContext httpContext = new();
            httpContext.Request.ContentType = customContentType;

            static Task next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("Test_error"));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal(customContentType, httpContext.Response.ContentType);
        }

        [Fact]
        public async Task ExceptionHandlerMiddleware_WithoutException_ShouldProcessSuccessFlow()
        {
            // Arrange
            DefaultHttpContext httpContext = new();

            static Task next(HttpContext httpContext) => Task.CompletedTask;

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnInternalServerErrorIfHandlerNotProvided()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            Task next(HttpContext httpContext) => Task.FromException<SystemException>(new SystemException("test_error"));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseResponse(HttpStatusCode.InternalServerError, httpContext.Response, "Failed result.", out string _);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnConflict()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "conflict_error";

            Task next(HttpContext httpContext) => Task.FromException<ConflictException>(new ConflictException(errorMessage));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseResponse(HttpStatusCode.Conflict, httpContext.Response, errorMessage, out string _);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnBadRequest()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "bad_request_error", errorCode = "test_code", errorDescription = "test_message";

            Task next(HttpContext httpContext) => Task.FromException<BadRequestException>(new BadRequestException(errorMessage, (errorCode, errorDescription)));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseErrorResponse(HttpStatusCode.BadRequest, httpContext.Response, errorMessage, errorCode, errorDescription);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnBadRequestForIdentityException()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "bad_request_error", errorCode = "test_code", errorDescription = "test_message";

            Task next(HttpContext httpContext) => Task.FromException<IdentityException>(new IdentityException(errorMessage, new Dictionary<string, IEnumerable<string>> {
                { errorCode, new List<string>{ errorDescription } }
            }));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseErrorResponse(HttpStatusCode.BadRequest, httpContext.Response, errorMessage, errorCode, errorDescription);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnNotFound()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "not_found_error";

            Task next(HttpContext httpContext) => Task.FromException<NotFoundException>(new NotFoundException(errorMessage));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseResponse(HttpStatusCode.NotFound, httpContext.Response, errorMessage, out string _);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnUnauthorized()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "authorization_error";

            Task next(HttpContext httpContext) => Task.FromException<AuthorizationException>(new AuthorizationException(errorMessage));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseResponse(HttpStatusCode.Unauthorized, httpContext.Response, errorMessage, out string _);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnForbidden()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "forbidden_error";

            Task next(HttpContext httpContext) => Task.FromException<ForbiddenException>(new ForbiddenException(errorMessage));

            ExceptionHandlerMiddleware middleware = new(next);

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            AssertBaseResponse(HttpStatusCode.Forbidden, httpContext.Response, errorMessage, out string _);
        }

        [Fact]
        [Trait("Exception", "With handling")]
        public async Task ExceptionHandlerMiddleware_WithException_ShouldReturnBadRequestForJwtException()
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Response.Body = new MemoryStream();

            string errorMessage = "jwt_error";

            Task next(HttpContext httpContext) => Task.FromException<JwtException>(new JwtException(errorMessage));

            ExceptionHandlerMiddleware middleware = new(next);

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

            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { errorCode, new string[1] { errorDescription } } }, response?.Errors);
        }
    }
}
