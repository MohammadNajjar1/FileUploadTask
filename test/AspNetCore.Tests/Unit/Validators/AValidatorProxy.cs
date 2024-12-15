using AspNetCore.Data;
using AspNetCore.Objects;

namespace AspNetCore.Validators;

public class AValidatorProxy : AValidator
{
    public AValidatorProxy(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public Boolean BaseIsSpecified<TView>(TView view, Expression<Func<TView, Object?>> property) where TView : AView
    {
        return IsSpecified(view, property);
    }
}
