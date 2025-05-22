using System.Linq.Expressions;

using SFC.Identity.Application.Common.Enums;

namespace SFC.Identity.Application.Features.Common.Models.Find.Sorting;

/// <summary>
/// Sorting list for quering to database.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class Sortings<TEntity>
{
    private readonly List<Sorting<TEntity, dynamic>> _sortList;

    public bool IsValid => _sortList.Any(s => s.Condition);

    public IEnumerable<Sorting<TEntity, dynamic>> Get() => _sortList.Where(s => s.Condition);

    public Sortings()
    {
        _sortList = [];
    }

    public Sortings(IEnumerable<Sorting<TEntity, dynamic>> sorting)
    {
        _sortList = new List<Sorting<TEntity, dynamic>>(sorting);
    }

    public void Add<TKey>(bool condition, Expression<Func<TEntity, dynamic>> expression, SortingDirection direction = SortingDirection.Ascending)
    {
        Append(condition, expression, direction);
    }

    public IQueryable<TEntity> ApplySort<TKey>(IQueryable<TEntity> query, IEnumerable<Sorting<TEntity, TKey>> sorting)
    {
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        Sorting<TEntity, TKey>? main = sorting.FirstOrDefault();
#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection

        if (main != null)
        {
            IOrderedQueryable<TEntity> orderedQuery = main.Direction == SortingDirection.Ascending
               ? query.OrderBy(main.Expression)
               : query.OrderByDescending(main.Expression);

#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
            IEnumerable<Sorting<TEntity, TKey>> secondary = sorting.Skip(1);
#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection

            foreach (Sorting<TEntity, TKey> sort in secondary)
            {
                orderedQuery = sort.Direction == SortingDirection.Ascending
                    ? orderedQuery.ThenBy(sort.Expression)
                    : orderedQuery.ThenByDescending(main.Expression);
            }

            return orderedQuery;
        }

        return query;
    }

    private void Append(bool condition, Expression<Func<TEntity, dynamic>> expression, SortingDirection direction)
    {
        _sortList.Add(new Sorting<TEntity, dynamic>
        {
            Condition = condition,
            Expression = expression,
            Direction = direction
        });
    }
}