using Microsoft.Extensions.Hosting;

using SFC.Identity.Application.Common.Constants;

using Environments = SFC.Identity.Application.Common.Constants.Environments;

namespace SFC.Identity.Infrastructure.Extensions;
public static class HostEnvironmentEnvExtensions
{
    public static bool IsTesting(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Testing);
    }
}
