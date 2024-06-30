using AutoMapper;

namespace SFC.Identity.Application.Common.Mappings;
public interface IMapFromReverse<T> : IMapFrom<T>
{
    new void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).ReverseMap();
}
