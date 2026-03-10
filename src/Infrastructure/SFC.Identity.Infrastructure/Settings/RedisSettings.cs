namespace SFC.Identity.Infrastructure.Settings;
public class RedisSettings
{
    public const string SectionKey = "Redis";

    public required string User { get; set; }

    public required string Password { get; set; }

    public required string InstanceName { get; set; }
}