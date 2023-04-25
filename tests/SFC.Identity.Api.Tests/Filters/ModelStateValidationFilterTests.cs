using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFC.Identity.Api.Filters;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Models;
using Xunit;

namespace SFC.Identity.Api.Tests.Filters
{
    public class ModelStateValidationFilterTests
    {
        [Fact]
        public async Task ValidationFilter_InvokeWithValidModelState_ShouldProcessRequest()
        {
            // Arrange 
            ValidationFilterAttribute filter = new();

            DefaultHttpContext httpContext = new();

            ActionContext actionContext = new(httpContext, new(), new(), new());

            ActionExecutingContext actionExecutingContext = new(actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(), null!);

            ActionExecutedContext actionExecutedContext = new(actionContext, new List<IFilterMetadata>(), null!);

            Task<ActionExecutedContext> next() => Task.FromResult(actionExecutedContext);

            // Act
            await filter.OnActionExecutionAsync(actionExecutingContext, next);

            // Assert
            Assert.Null(actionExecutingContext.Result);
        }

        [Fact]
        public async Task ValidationFilter_InvokeWithInvalidModelState_ShouldReturnBadRequestResult()
        {
            // Arrange 
            string errorCode = "test_code";

            // Act
            ActionExecutingContext context = await ExecuteActionFilterAsync(errorCode);

            // Assert
            BadRequestObjectResult result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.NotNull(result.Value);
            Assert.IsType<BaseErrorResponse>(result.Value);
            BaseErrorResponse? response = result.Value as BaseErrorResponse;
            Assert.False(response?.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, response?.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { errorCode, new string[1] { "test_message" } } }, response?.Errors);
        }

        [Fact]
        public async Task ValidationFilter_InvokeWithInvalidModelState_ShouldReturnBadRequestResultForEmptyBody()
        {
            // Act
            ActionExecutingContext context = await ExecuteActionFilterAsync(string.Empty);

            // Assert
            BadRequestObjectResult result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.NotNull(result.Value);
            Assert.IsType<BaseErrorResponse>(result.Value);
            BaseErrorResponse? response = result.Value as BaseErrorResponse;
            Assert.False(response?.Success);
            Assert.Equal(ErrorConstants.VALIDATION_ERROR_MESSAGE, response?.Message);
            Assert.Equal(new Dictionary<string, IEnumerable<string>> { { "Body", new string[1] { "Request body is required." } } }, response?.Errors);
        }

        private async Task<ActionExecutingContext> ExecuteActionFilterAsync(string errorCode)
        {
            // Arrange 
            ValidationFilterAttribute filter = new();

            DefaultHttpContext httpContext = new();

            string errorMessage = "test_message";
            ModelStateDictionary modelState = new();
            modelState.AddModelError(errorCode, errorMessage);

            ActionContext actionContext = new(httpContext, new(), new(), modelState);

            List<IFilterMetadata> filterMetadata = new();

            ActionExecutingContext actionExecutingContext = new(actionContext,
                filterMetadata,
                new Dictionary<string, object?>(), null!);

            ActionExecutedContext actionExecutedContext = new(actionContext, filterMetadata, null!);

            Task<ActionExecutedContext> next() => Task.FromResult(actionExecutedContext);

            // Act
            await filter.OnActionExecutionAsync(actionExecutingContext, next);

            return actionExecutingContext;
        }
    }
}
