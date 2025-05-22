using AutoMapper;

using MassTransit;

using Microsoft.Extensions.Configuration;

using SFC.Identity.Application.Interfaces.Identity;
using SFC.Identity.Domain.Entities.User;
using SFC.Identity.Infrastructure.Extensions;
using SFC.Identity.Infrastructure.Settings.RabbitMq;
using SFC.Identity.Messages.Commands.User;

using Exchange = SFC.Identity.Infrastructure.Settings.RabbitMq.Exchange;

namespace SFC.Identity.Infrastructure.Consumers.User;
public class RequireUsersSeedConsumer(
    IMapper mapper,
    IUserSeedService usersSeedService)
    : IConsumer<RequireUsersSeed>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserSeedService _usersSeedService = usersSeedService;

    public async Task Consume(ConsumeContext<RequireUsersSeed> context)
    {
        RequireUsersSeed message = context.Message;

        IEnumerable<IUser> users = await _usersSeedService.GetSeedUsersAsync().ConfigureAwait(true);

        SeedUsers command = _mapper.Map<SeedUsers>(users)
                                   .SetCommandInitiator(message.Initiator);

        await context.Publish(command).ConfigureAwait(false);
    }
}

public class RequireUsersSeedConsumerDefinition : ConsumerDefinition<RequireUsersSeedConsumer>
{
    private readonly RabbitMqSettings _settings;

    private Exchange Exchange { get { return _settings.Exchanges.Identity.Value.Domain.User.Seed.RequireSeed; } }

    public RequireUsersSeedConsumerDefinition(IConfiguration configuration)
    {
        _settings = configuration.GetRabbitMqSettings();
        EndpointName = "sfc.identity.users.seed.require.queue";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RequireUsersSeedConsumer> consumerConfigurator,
            IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
        {
            rmq.AutoDelete = true;
            rmq.DiscardFaultedMessages();

            rmq.Bind(Exchange.Name, x => x.AutoDelete = true);
        }
    }
}