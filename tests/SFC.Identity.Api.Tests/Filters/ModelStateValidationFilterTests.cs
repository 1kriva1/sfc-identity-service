using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SFC.Identity.Api.Infrastructure.Filters;
using SFC.Identity.Api.Infrastructure.Models.Base;
using SFC.Identity.Application.Common.Constants;

namespace SFC.Identity.Api.Tests.Filters;

public class ModelStateValidationFilterTests
{
    [Fact]
    [Trait("API", "Filter")]
    public async Task API_Filter_Validation_ShouldProcessRequest()
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
        await filter.OnActionExecutionAsync(actionExecutingContext, next).ConfigureAwait(false);

        // Assert
        Assert.Null(actionExecutingContext.Result);
    }

    [Fact]
    [Trait("API", "Filter")]
    public async Task API_Filter_Validation_ShouldReturnBadRequestResult()
    {
        // Arrange 
        string errorCode = "test_code";

        // Act
        ActionExecutingContext context = await ExecuteActionFilterAsync(errorCode).ConfigureAwait(false);

        // Assert
        BadRequestObjectResult result = Assert.IsType<BadRequestObjectResult>(context.Result);

        Assert.NotNull(result.Value);
        Assert.IsType<BaseErrorResponse>(result.Value);

        BaseErrorResponse? response = result.Value as BaseErrorResponse;

        Assert.False(response?.Success);
        Assert.Equal(Localization.ValidationError, response?.Message);
        Assert.NotNull(response!.Errors);
        Assert.True(response.Errors.ContainsKey(errorCode));
        Assert.Single(response.Errors[errorCode]);
        Assert.Equal("test_message", response.Errors[errorCode].FirstOrDefault());
    }

    [Fact]
    [Trait("API", "Filter")]
    public async Task API_Filter_Validation_ShouldReturnBadRequestResultForEmptyBody()
    {
        // Act
        ActionExecutingContext context = await ExecuteActionFilterAsync(string.Empty).ConfigureAwait(false);

        // Assert
        BadRequestObjectResult result = Assert.IsType<BadRequestObjectResult>(context.Result);

        Assert.NotNull(result.Value);
        Assert.IsType<BaseErrorResponse>(result.Value);

        BaseErrorResponse? response = result.Value as BaseErrorResponse;

        Assert.False(response?.Success);
        Assert.Equal(Localization.ValidationError, response?.Message);
        Assert.NotNull(response!.Errors);
        Assert.True(response.Errors.ContainsKey("Body"));
        Assert.Single(response.Errors["Body"]);
        Assert.Equal(Localization.RequestBodyRequired, response.Errors["Body"].FirstOrDefault());
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

        Task<ActionExecutedContext> Next() => Task.FromResult(actionExecutedContext);

        // Act
        await filter.OnActionExecutionAsync(actionExecutingContext, Next).ConfigureAwait(false);

        return actionExecutingContext;
    }
}