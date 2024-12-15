using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace AspNetCore.Data;

public class Query<TModel> : IQuery<TModel> where TModel : class
{
    public Type ElementType => Set.ElementType;
    public Expression Expression => Set.Expression;
    public IQueryProvider Provider => Set.Provider;

    private IQueryable<TModel> Set { get; }
    private IConfigurationProvider Mapper { get; }

    public Query(IQueryable<TModel> set, IConfigurationProvider mapper)
    {
        Set = set;
        Mapper = mapper;
    }

    public IQuery<TResult> Select<TResult>(Expression<Func<TModel, TResult>> selector) where TResult : class
    {
        return new Query<TResult>(Set.Select(selector), Mapper);
    }
    public IQuery<TModel> Where(Expression<Func<TModel, Boolean>> predicate)
    {
        return new Query<TModel>(Set.Where(predicate), Mapper);
    }

    public IQueryable<TView> To<TView>()
    {
        return Set.AsNoTracking().ProjectTo<TView>(Mapper);
    }

    public IEnumerator<TModel> GetEnumerator()
    {
        return Set.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
