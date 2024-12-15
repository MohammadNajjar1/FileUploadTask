using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;
using NonFactors.Mvc.Lookup;

namespace AspNetCore.Components.Lookups;

public class MvcLookup<TModel, TView> : ALookup<TView>
    where TModel : AModel
    where TView : AView
{
    protected IUnitOfWork UnitOfWork { get; }

    public MvcLookup(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public override String GetColumnHeader(PropertyInfo property)
    {
        return Resource.ForProperty(typeof(TView), property.Name);
    }
    public override String GetColumnCssClass(PropertyInfo property)
    {
        Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (type.IsEnum)
            return "text-start";

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                return "text-end";
            default:
                return "text-start";
        }
    }

    public override IQueryable<TView> GetModels()
    {
        return UnitOfWork.Select<TModel>().To<TView>();
    }
}
