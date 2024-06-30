using Grpc.Core;

using SFC.Identity.Grpc.Messages.CreatePlayer;

using static SFC.Identity.Grpc.Services.IdentityService;

namespace SFC.Identity.Infrastructure.Services.Internal;
public class IdentityService : IdentityServiceBase
{
    public override Task<CreatePlayerResponse> CreatePlayer(CreatePlayerRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CreatePlayerResponse { });
    }
}
