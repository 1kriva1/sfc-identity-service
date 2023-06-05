using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFC.Identity.Application.Common.Constants;
using System.Reflection;

namespace SFC.Identity.Application
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton<Messages>();

            return services;
        }
    }
}
