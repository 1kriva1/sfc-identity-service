using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SFC.Identity.Api.Filters;
using SFC.Identity.Application;
using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Models.Base;

namespace SFC.Identity.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddControllers(this IServiceCollection services)
    {
        services.AddControllers(configure =>
        {
            // Return 406 when Accept is not application/json
            configure.ReturnHttpNotAcceptable = true;

            // Accept and Content-Type headers filters
            configure.Filters.Add(new ProducesAttribute(CommonConstants.CONTENT_TYPE));
            configure.Filters.Add(new ConsumesAttribute(CommonConstants.CONTENT_TYPE));

            // Global responses filters
            configure.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            configure.Filters.Add(new ProducesResponseTypeAttribute(typeof(BaseResponse), StatusCodes.Status500InternalServerError));

            // Custom filters
            configure.Filters.Add(new ValidationFilterAttribute());
        })
        .AddJsonOptions(configure =>
        {
            configure.JsonSerializerOptions.PropertyNamingPolicy = null;
            configure.JsonSerializerOptions.DictionaryKeyPolicy = null;
            configure.JsonSerializerOptions.WriteIndented = true;
        })
        .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
        .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (type, factory) =>
                factory.Create(typeof(Resources)));
    }
}
