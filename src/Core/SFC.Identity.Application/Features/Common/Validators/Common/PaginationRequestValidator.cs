using FluentValidation;

using SFC.Identity.Application.Features.Common.Base;
using SFC.Identity.Application.Features.Common.Models.Find.Paging;
using SFC.Identity.Application.Features.Common.Validators.Common;

namespace SFC.Identity.Application.Features.Common.Validators.Common;

/// <summary>
/// Validation for pagination and sorting values.
/// </summary>
/// <typeparam name="TResponse">Pagination request response type.</typeparam>
/// <typeparam name="TFilter">Filter type.</typeparam>
public class PaginationRequestValidator<TResponse, TFilter> : AbstractValidator<BasePaginationRequest<TResponse, TFilter>>
{
    public PaginationRequestValidator()
    {
        // pagination
        RuleFor(command => command.Pagination)
            .NotNull()
            .SetValidator(new PaginationValidator())
            .WithName(nameof(Pagination));

        //sorting
        RuleForEach(command => command.Sorting)
            .SetValidator(new SortingValidator());
    }
}