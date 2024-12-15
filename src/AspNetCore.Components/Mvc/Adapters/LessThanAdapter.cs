using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class LessThanAdapter : AttributeAdapterBase<LessThanAttribute>
{
    public LessThanAdapter(LessThanAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-lower"] = GetErrorMessage(context);
        context.Attributes["data-val-lower-than"] = Attribute.Maximum.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
