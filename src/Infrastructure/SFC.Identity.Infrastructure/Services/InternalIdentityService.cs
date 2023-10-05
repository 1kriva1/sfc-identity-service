using Grpc.Core;

using GrpcIdentityService;

using static GrpcIdentityService.IdentityService;

namespace SFC.Identity.Infrastructure.Services;
public class InternalIdentityService : IdentityServiceBase
{
    public override Task<CreatePlayerResponse> CreatePlayer(CreatePlayerRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CreatePlayerResponse { });
    }
}
