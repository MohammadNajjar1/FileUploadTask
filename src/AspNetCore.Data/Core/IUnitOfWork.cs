using AspNetCore.Objects;

namespace AspNetCore.Data;

public interface IUnitOfWork : IDisposable
{
    TDestination? GetAs<TModel, TDestination>(Int64? id) where TModel : AModel where TDestination : class;
    TModel? Get<TModel>(Int64? id) where TModel : AModel;
    TDestination To<TDestination>(Object source);

    IQuery<TModel> Select<TModel>() where TModel : AModel;

    void InsertRange<TModel>(IEnumerable<TModel> models) where TModel : AModel;
    void Insert<TModel>(TModel model) where TModel : AModel;

    void Update<TModel>(TModel model) where TModel : AModel;

    void DeleteRange<TModel>(IEnumerable<TModel> models) where TModel : AModel;
    void Delete<TModel>(TModel model) where TModel : AModel;
    void Delete<TModel>(Int64 id) where TModel : AModel;

    void Commit();
}
