using Microsoft.AspNetCore.Mvc;

using SFC.Identity.Api.Infrastructure.Extensions;
using SFC.Identity.Api.Infrastructure.Filters;
using SFC.Identity.Api.Infrastructure.Models.Base;
using SFC.Identity.Application;
using SFC.Identity.Infrastructure.Constants;

namespace SFC.Identity.Api.Infrastructure.Extensions;

public static class ControllersExtensions
{
    public static void AddControllers(this IServiceCollection services)
    {
        services.AddControllers(configure =>
        {
            // Return 406 when Accept is not application/json
            configure.ReturnHttpNotAcceptable = true;

            // Accept and Content-Type headers filters
            configure.Filters.Add(new ProducesAttribute(CommonConstants.ContentType));
            configure.Filters.Add(new ConsumesAttribute(CommonConstants.ContentType));

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