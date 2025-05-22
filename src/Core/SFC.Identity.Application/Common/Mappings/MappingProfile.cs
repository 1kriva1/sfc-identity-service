using System.Reflection;

using SFC.GeneralTemplate.Application.Features.Common.Dto.Pagination;
using SFC.Identity.Application.Common.Mappings.Base;
using SFC.Identity.Application.Features.Common.Dto.Pagination;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;

namespace SFC.Identity.Application.Common.Mappings;
public class MappingProfile : BaseMappingProfile
{
    protected override Assembly Assembly => Assembly.GetExecutingAssembly();

    public MappingProfile() : base()
    {
        ApplyCustomMappings();
    }

    private void ApplyCustomMappings()
    {
        #region Generic types

        CreateMap(typeof(PagedList<>), typeof(PageDto<>))
            .ForMember(nameof(PageDto<object>.Items), d => d.Ignore())
            .ForMember(nameof(PageDto<object>.Metadata), d => d.Ignore());

        CreateMap(typeof(PagedList<>), typeof(PageMetadataDto));

        #endregion Generic types        
    }
}