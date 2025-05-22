using SFC.Identity.Application.Common.Enums;
using SFC.Identity.Application.Features.Common.Base;

#pragma warning disable CA1716
namespace SFC.Identity.Application.Features.User.Queries.Get;
#pragma warning restore CA1716
public class GetUserQuery : Request<GetUserViewModel>
{
    public override RequestId RequestId { get => RequestId.GetUser; }

    public Guid Id { get; set; }
}