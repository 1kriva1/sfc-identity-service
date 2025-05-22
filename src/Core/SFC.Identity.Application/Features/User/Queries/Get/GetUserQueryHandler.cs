using AutoMapper;

using MediatR;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Interfaces.Persistence.Repository.User;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Features.User.Queries.Get;
public class GetUserQueryHandler(IMapper mapper, IUserRepository userRepository) : IRequestHandler<GetUserQuery, GetUserViewModel>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<GetUserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        IUser user = await _userRepository.GetByIdAsync(request.Id).ConfigureAwait(true)
            ?? throw new NotFoundException(Localization.UserNotFound);

        return _mapper.Map<GetUserViewModel>(user);
    }
}