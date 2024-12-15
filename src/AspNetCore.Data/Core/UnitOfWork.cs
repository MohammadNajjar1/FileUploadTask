using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AspNetCore.Objects;

namespace AspNetCore.Data;

public class UnitOfWork : IUnitOfWork
{
    protected IMapper Mapper { get; }
    protected DbContext Context { get; }

    public UnitOfWork(DbContext context, IMapper mapper)
    {
        Mapper = mapper;
        Context = context;
    }

    public TDestination? GetAs<TModel, TDestination>(Int64? id) where TModel : AModel where TDestination : class
    {
        return id == null
            ? default
            : Context
                .Set<TModel>()
                .AsNoTracking()
                .Where(model => model.Id == id)
                .ProjectTo<TDestination>(Mapper.ConfigurationProvider)
                .FirstOrDefault();
    }
    public TModel? Get<TModel>(Int64? id) where TModel : AModel
    {
        return id == null ? null : Context.Find<TModel>(id);
    }
    public TDestination To<TDestination>(Object source)
    {
        return Mapper.Map<TDestination>(source);
    }

    public IQuery<TModel> Select<TModel>() where TModel : AModel
    {
        return new Query<TModel>(Context.Set<TModel>(), Mapper.ConfigurationProvider);
    }

    public void InsertRange<TModel>(IEnumerable<TModel> models) where TModel : AModel
    {
        foreach (TModel model in models)
        {
            model.Id = 0;

            Context.Add(model);
        }
    }
    public void Insert<TModel>(TModel model) where TModel : AModel
    {
        model.Id = 0;

        Context.Add(model);
    }
    public void Update<TModel>(TModel model) where TModel : AModel
    {
        EntityEntry<TModel> entry = Context.Entry(model);

        if (entry.State == EntityState.Detached)
            entry.State = EntityState.Modified;

        entry.Property(property => property.CreationDate).IsModified = false;
    }

    public void DeleteRange<TModel>(IEnumerable<TModel> models) where TModel : AModel
    {
        Context.RemoveRange(models);
    }
    public void Delete<TModel>(TModel model) where TModel : AModel
    {
        Context.Remove(model);
    }
    public void Delete<TModel>(Int64 id) where TModel : AModel
    {
        Delete(Context.Find<TModel>(id)!);
    }

    public virtual void Commit()
    {
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
