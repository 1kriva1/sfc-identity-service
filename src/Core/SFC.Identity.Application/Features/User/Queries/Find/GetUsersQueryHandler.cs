using AutoMapper;

using MediatR;

using SFC.Identity.Application.Common.Dto.User;
using SFC.Identity.Application.Features.Common.Dto.Pagination;
using SFC.Identity.Application.Features.Common.Models.Find;
using SFC.Identity.Application.Features.Common.Models.Find.Filters;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Features.Common.Models.Find.Sorting;
using SFC.Identity.Application.Features.User.Queries.Find.Extensions;
using SFC.Identity.Application.Interfaces.Persistence.Repository.User;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Features.User.Queries.Find;
public class GetUsersQueryHandler(IMapper mapper, IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, GetUsersViewModel>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<GetUsersViewModel> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Filter<IUser>> filters = request.Filter.BuildSearchFilters();

        IEnumerable<Sorting<IUser, dynamic>>? sorting = request.Sorting.BuildGeneralTemplateSearchSorting();

        FindParameters<IUser> parameters = new()
        {
            Pagination = _mapper.Map<Pagination>(request.Pagination),
            Filters = new Filters<IUser>(filters),
            Sorting = new Sortings<IUser>(sorting)
        };

        PagedList<IUser> pageList = await _userRepository.FindAsync(parameters).ConfigureAwait(true);

        return new GetUsersViewModel
        {
            Items = _mapper.Map<IEnumerable<UserDto>>(pageList),
            Metadata = _mapper.Map<PageMetadataDto>(pageList)
        };
    }
}