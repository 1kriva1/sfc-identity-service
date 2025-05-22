using System.Linq.Expressions;

using SFC.Identity.Application.Common.Enums;

namespace SFC.Identity.Application.Features.Common.Models.Find.Sorting;

/// <summary>
/// Sorting class.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class Sorting<TEntity, TKey>
{
    /// <summary>
    /// Condition to define if sorting expression will be used during query to database.
    /// </summary>
    public bool Condition { get; set; }

    /// <summary>
    /// Expression for sorting during quering to database.
    /// </summary>
    public Expression<Func<TEntity, TKey>> Expression { get; set; } = default!;

    /// <summary>
    /// Sorting direction.
    /// </summary>
    public SortingDirection Direction { get; set; }
}