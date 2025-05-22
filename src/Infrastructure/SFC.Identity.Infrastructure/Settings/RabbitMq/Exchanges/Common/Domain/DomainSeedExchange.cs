namespace SFC.Identity.Infrastructure.Settings.RabbitMq.Exchanges.Common.Domain;

public class DomainSeedExchange
{
    public Exchange Seeded { get; set; } = default!;

    public Exchange Seed { get; set; } = default!;

    public Exchange RequireSeed { get; set; } = default!;
}