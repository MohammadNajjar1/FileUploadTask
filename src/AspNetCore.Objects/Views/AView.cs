using AutoMapper;

namespace AspNetCore.Objects;

public abstract class AView
{
    public virtual Int64 Id
    {
        get;
        set;
    }

    public virtual DateTime CreationDate
    {
        get
        {
            if (!IsCreationDateSet)
                CreationDate = DateTime.Now;

            return InternalCreationDate;
        }
        protected set
        {
            IsCreationDateSet = true;
            InternalCreationDate = value;
        }
    }
    private Boolean IsCreationDateSet
    {
        get;
        set;
    }
    private DateTime InternalCreationDate
    {
        get;
        set;
    }
}
public abstract class AView<TModel> : AView
{
    internal virtual void Map(Profile profile)
    {
        profile.CreateMap(typeof(TModel), GetType()).ReverseMap();
    }
}
