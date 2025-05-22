using FluentValidation;
using FluentValidation.Results;

using MediatR;

using SFC.Identity.Application.Common.Constants;
using SFC.Identity.Application.Common.Exceptions;
using SFC.Identity.Application.Features.Common.Base;

namespace SFC.Identity.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
 where TRequest : notnull, BaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            ValidationResult[] validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)))
                                                             .ConfigureAwait(false);

            List<ValidationFailure> failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                // if need to throw exception when validation has failed 
                failures.ForEach(failure =>
                {
                    if (failure.CustomState is Exception ex) throw ex;
                });

                throw new BadRequestException(Localization.ValidationError, failures);
            }
        }

        ArgumentNullException.ThrowIfNull(next);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        return await next();//.ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}