using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using SFC.Identity.Domain.Common;
using SFC.Identity.Domain.Common.Interfaces;

namespace SFC.Identity.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor for dispatching domain events when saving changes in the database.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DispatchDomainEventsSaveChangesInterceptor"/> class.
/// </remarks>
/// <param name="mediator">The mediator instance used for publishing domain events.</param>
public class DispatchDomainEventsSaveChangesInterceptor(
    ILogger<DispatchDomainEventsSaveChangesInterceptor> logger,
    IMediator mediator) : SaveChangesInterceptor
{
    private readonly ILogger<DispatchDomainEventsSaveChangesInterceptor> _logger = logger;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc/>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;

        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);

        Tuple<List<IEntity>, List<BaseEvent>> domain = GetDomainEvents(context, state => state == EntityState.Deleted);

        List<BaseEvent> domainEvents = domain.Item2;

        if (domainEvents.Count != 0)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            try
            {
                InterceptionResult<int> saveResult = await base.SavingChangesAsync(eventData, result, cancellationToken)
                                                               .ConfigureAwait(true);

                await PublishEventsAsync(domain, cancellationToken).ConfigureAwait(false);

                await transaction.CommitAsync(cancellationToken)
                                 .ConfigureAwait(false);

                return saveResult;
            }
            catch (Exception exception)
            {
                Action<ILogger, Exception> logRequest = LoggerMessage.Define(
                    LogLevel.Error,
                    new EventId(),
                    $"Error during saving domain entity on delete."
                );
                logRequest(_logger, exception);

                await transaction.RollbackAsync(cancellationToken)
                                 .ConfigureAwait(false);
                throw;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken)
                         .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;

        if (context == null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);

        Tuple<List<IEntity>, List<BaseEvent>> domain = GetDomainEvents(context, state => state != EntityState.Deleted);

        List<BaseEvent> domainEvents = domain.Item2;

        if (domainEvents.Count != 0)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            try
            {
                int saveResult = await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);

                await PublishEventsAsync(domain, cancellationToken).ConfigureAwait(false);

                await transaction.CommitAsync(cancellationToken)
                                 .ConfigureAwait(false);

                return saveResult;
            }
            catch (Exception exception)
            {
                Action<ILogger, Exception> logRequest = LoggerMessage.Define(
                    LogLevel.Error,
                    new EventId(),
                    $"Error during saving domain entity on add or update."
                );
                logRequest(_logger, exception);

                await transaction.RollbackAsync(cancellationToken)
                                 .ConfigureAwait(false);
                throw;
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    private static Tuple<List<IEntity>, List<BaseEvent>> GetDomainEvents(DbContext context, Predicate<EntityState> predicate)
    {
        List<IEntity> domainEventEntities = context.ChangeTracker
            .Entries<IEntity>()
            .Where(e => e.Entity.DomainEvents.Count != 0 && predicate(e.State))
            .Select(e => e.Entity)
            .ToList();

        List<BaseEvent> domainEvents = domainEventEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        return Tuple.Create(domainEventEntities, domainEvents);
    }

    private async Task PublishEventsAsync(Tuple<List<IEntity>, List<BaseEvent>> domain, CancellationToken cancellationToken = default)
    {
        List<IEntity> domainEventEntities = domain.Item1;
        List<BaseEvent> domainEvents = domain.Item2;

        domainEventEntities.ForEach(e => e.ClearDomainEvents());

        foreach (BaseEvent domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken)
                           .ConfigureAwait(false);
        }
    }
}