using AutoMapper;

using SFC.Identity.Application.Common.Dto.User;
using SFC.Identity.Application.Common.Mappings.Interfaces;
using SFC.Identity.Domain.Entities.User;

namespace SFC.Identity.Application.Features.User.Queries.Get;
public class GetUserViewModel : IMapFrom<IUser>
{
    public required UserDto User { get; set; }

    public void Mapping(Profile profile) => profile.CreateMap<IUser, GetUserViewModel>()
                                                   .ForMember(p => p.User, d => d.MapFrom(z => z));
}