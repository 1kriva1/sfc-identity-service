using Microsoft.Extensions.Hosting;

using Environments = SFC.Identity.Infrastructure.Constants.Environments;

namespace SFC.Identity.Infrastructure.Extensions;
public static class HostEnvironmentEnvExtensions
{
    public static bool IsTesting(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Testing);
    }
}