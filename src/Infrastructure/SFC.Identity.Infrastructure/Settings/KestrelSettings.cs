namespace SFC.Identity.Infrastructure.Settings;
public class KestrelSettings
{
    public const string SectionKey = "Kestrel";

    public Dictionary<string, KestrelEndpointSettings> Endpoints { get; init; } = [];
}

public class KestrelEndpointSettings
{
#pragma warning disable CA1056 // URI-like properties should not be strings
    public required string Url { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public required string Protocols { get; set; }
}