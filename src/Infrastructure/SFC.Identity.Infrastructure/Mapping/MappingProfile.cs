using System.Reflection;

using SFC.Identity.Application.Common.Dto.User;
using SFC.Identity.Application.Common.Mappings.Base;
using SFC.Identity.Domain.Entities.User;
using SFC.Identity.Infrastructure.Persistence.Entities;
using SFC.Identity.Messages.Commands.User;
using SFC.Identity.Messages.Events.User;
using SFC.Identity.Messages.Models.User;

namespace SFC.Identity.Infrastructure.Mapping;
public class MappingProfile : BaseMappingProfile
{
    protected override Assembly Assembly => Assembly.GetExecutingAssembly();

    public MappingProfile()
    {
        ApplyCustomMappings();
    }

    protected void ApplyCustomMappings()
    {
        #region Complex types

        CreateMap<ApplicationUser, UserDto>();

        #endregion Complex types

        #region Messages

        // identity messages
        CreateMapIdentityMessages();

        #endregion Messages
    }

    private void CreateMapIdentityMessages()
    {
        CreateMap<IEnumerable<IUser>, SeedUsers>()
            .ForMember(p => p.Users, d => d.MapFrom(z => z));
        CreateMap<IEnumerable<ApplicationUser>, SeedUsers>()
            .ForMember(p => p.Users, d => d.MapFrom(z => z));

        CreateMap<IEnumerable<IUser>, UsersSeeded>()
            .ForMember(p => p.Users, d => d.MapFrom(z => z));
        CreateMap<IEnumerable<ApplicationUser>, UsersSeeded>()
            .ForMember(p => p.Users, d => d.MapFrom(z => z));

        CreateMap<IUser, UserCreated>()
            .ForMember(p => p.User, d => d.MapFrom(z => z));
        CreateMap<ApplicationUser, UserCreated>()
            .ForMember(p => p.User, d => d.MapFrom(z => z));

        CreateMap<ApplicationUser, User>();
        CreateMap<IUser, User>();
    }
}