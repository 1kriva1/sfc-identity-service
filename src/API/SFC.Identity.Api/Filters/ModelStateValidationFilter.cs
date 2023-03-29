using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Models;

namespace SFC.Identity.Api.Filters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                BaseErrorResponse result;

                if (context.ModelState.ErrorCount == 1 && context.ModelState.Any(e => string.IsNullOrEmpty(e.Key)))
                {
                    Dictionary<string, IEnumerable<string>> emptyBodyError = new() { { "Body", new List<string> { "Request body is required." } } };
                    result = new BaseErrorResponse(ErrorConstants.VALIDATION_ERROR_MESSAGE, emptyBodyError);
                }
                else
                {
                    result = new(ErrorConstants.VALIDATION_ERROR_MESSAGE, context.ModelState.ToDictionary(
                        state => state.Key,
                        state => state.Value?.Errors.Select(e => e.ErrorMessage) ?? Array.Empty<string>())
                   );
                }

                context.Result = new BadRequestObjectResult(result);
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
