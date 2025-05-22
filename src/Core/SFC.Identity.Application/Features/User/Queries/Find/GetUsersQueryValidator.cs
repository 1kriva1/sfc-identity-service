using FluentValidation;

using SFC.Identity.Application.Features.Common.Validators.Common;
using SFC.Identity.Application.Features.User.Queries.Find.Dto.Filters;

namespace SFC.Identity.Application.Features.User.Queries.Find;
public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        // pagination request validation
        RuleFor(command => command)
            .SetValidator(new PaginationRequestValidator<GetUsersViewModel, GetUsersFilterDto>());
    }
}