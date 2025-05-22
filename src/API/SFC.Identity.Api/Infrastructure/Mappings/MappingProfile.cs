using System.Reflection;

using Google.Protobuf.WellKnownTypes;

using SFC.Identity.Application.Common.Dto.User;
using SFC.Identity.Application.Common.Extensions;
using SFC.Identity.Application.Common.Mappings.Base;
using SFC.Identity.Application.Features.Common.Dto.Common;
using SFC.Identity.Application.Features.Common.Dto.Pagination;
using SFC.Identity.Application.Features.User.Queries.Find;
using SFC.Identity.Application.Features.User.Queries.Find.Dto.Filters;
using SFC.Identity.Application.Features.User.Queries.Get;

namespace SFC.Identity.Api.Infrastructure.Mappings;
public class MappingProfile : BaseMappingProfile
{
    protected override Assembly Assembly => Assembly.GetExecutingAssembly();

    public MappingProfile() : base()
    {
        ApplyCustomMappings();
    }

    private void ApplyCustomMappings()
    {
        #region Simple types

        CreateMap<DateTime, Timestamp>()
            .ConvertUsing(value => DateTime.SpecifyKind(value, DateTimeKind.Utc).ToTimestamp());

        #endregion Simple types

        #region Complex types

        // identity contracts
        CreateMapIdentityContracts();

        #endregion Complex types
    }

    private void CreateMapIdentityContracts()
    {
        // get user
        CreateMap<SFC.Identity.Contracts.Messages.User.Get.GetUserRequest, GetUserQuery>();
        CreateMap<UserDto, SFC.Identity.Contracts.Models.User.User>();
        CreateMap<GetUserViewModel, SFC.Identity.Contracts.Messages.User.Get.GetUserResponse>();
        CreateMap<UserDto, SFC.Identity.Contracts.Headers.AuditableHeader>();

        // get users
        // (filters)
        CreateMap<SFC.Identity.Contracts.Messages.User.Find.GetUsersRequest, GetUsersQuery>();
        CreateMap<SFC.Identity.Contracts.Models.Common.Pagination, PaginationDto>();
        CreateMap<SFC.Identity.Contracts.Models.Common.Sorting, SortingDto>();
        CreateMap<SFC.Identity.Contracts.Messages.User.Find.Filters.GetUsersFilter, GetUsersFilterDto>();
        // (result)
        CreateMap<GetUsersViewModel, SFC.Identity.Contracts.Messages.User.Find.GetUsersResponse>();
        // (headers)
        CreateMap<PageMetadataDto, SFC.Identity.Contracts.Headers.PaginationHeader>();
    }
}