using MediatR;

using Microsoft.Extensions.Logging;

using SFC.Identity.Application.Common.Enums;

namespace SFC.Identity.Application.Features.Common.Base;
public abstract class BaseRequest
{
    public abstract RequestId RequestId { get; }

    public EventId EventId => new((int)RequestId, Enum.GetName(RequestId));
}

public abstract class Request : BaseRequest, IRequest { }

public abstract class Request<TResponse> : BaseRequest, IRequest<TResponse> { }