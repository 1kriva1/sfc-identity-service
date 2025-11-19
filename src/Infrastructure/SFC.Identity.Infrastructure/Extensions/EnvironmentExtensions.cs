using Microsoft.Extensions.Hosting;

using SFC.Identity.Infrastructure.Constants;

using Environments = SFC.Identity.Infrastructure.Constants.Environments;

namespace SFC.Identity.Infrastructure.Extensions;
public static class EnvironmentExtensions
{
    public static bool IsTesting(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Testing);
    }

    public static bool IsRunningInContainer => Environment.GetEnvironmentVariable(EnvironmentConstants.RunningInContainer) == "true";
}