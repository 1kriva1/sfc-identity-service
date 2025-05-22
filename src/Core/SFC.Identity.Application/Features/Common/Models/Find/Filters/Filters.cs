using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace SFC.Identity.Application.Features.Common.Models.Find.Filters;

/// <summary>
/// Filters list for quering to database.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class Filters<TEntity>
{
    private readonly List<Filter<TEntity>> _filterList;

    public bool IsValid => _filterList.Any(f => f.Condition);

    public Collection<Filter<TEntity>> FilterList => new(_filterList.Where(f => f.Condition).ToList());

    public Filters()
    {
        _filterList = [];
    }

    public Filters(IEnumerable<Filter<TEntity>> filters)
    {
        _filterList = new List<Filter<TEntity>>(filters);
    }

    public void Add(bool condition, Expression<Func<TEntity, bool>> expression)
    {
        _filterList.Add(new Filter<TEntity>
        {
            Condition = condition,
            Expression = expression
        });
    }
}