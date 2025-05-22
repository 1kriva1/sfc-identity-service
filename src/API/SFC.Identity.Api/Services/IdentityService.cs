using AutoMapper;

using Grpc.Core;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using SFC.Identity.Api.Infrastructure.Extensions;
using SFC.Identity.Application.Features.User.Queries.Find;
using SFC.Identity.Application.Features.User.Queries.Get;
using SFC.Identity.Contracts.Headers;
using SFC.Identity.Contracts.Messages.User.Find;
using SFC.Identity.Contracts.Messages.User.Get;
using SFC.Identity.Infrastructure.Constants;

using static SFC.Identity.Contracts.Services.IdentityService;

namespace SFC.Identity.Api.Services;

// Need set AuthenticationSchemes, because not work and if set this in AuthExtensions Duende IdentityServer refuse to work 
[Authorize(Policy.General, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IdentityService(IMapper mapper, ISender mediator) : IdentityServiceBase
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _mediator = mediator;

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        GetUserQuery query = _mapper.Map<GetUserQuery>(request);

        GetUserViewModel model = await _mediator.Send(query).ConfigureAwait(true);

        context.AddAuditableHeaderIfRequested(_mapper.Map<AuditableHeader>(model.User));

        return _mapper.Map<GetUserResponse>(model);
    }

    public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
    {
        GetUsersQuery query = _mapper.Map<GetUsersQuery>(request);

        GetUsersViewModel result = await _mediator.Send(query).ConfigureAwait(true);

        context.AddPaginationHeader(_mapper.Map<PaginationHeader>(result.Metadata));

        return _mapper.Map<GetUsersResponse>(result);
    }
}