using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class NumericAdapter : AttributeAdapterBase<NumericAttribute>
{
    public NumericAdapter(NumericAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-number"] = GetErrorMessage(context);
        context.Attributes["data-val-number-scale"] = Attribute.Scale.ToString(CultureInfo.InvariantCulture);
        context.Attributes["data-val-number-precision"] = Attribute.Precision.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
