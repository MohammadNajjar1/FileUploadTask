using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class DigitsAdapter : AttributeAdapterBase<DigitsAttribute>
{
    public DigitsAdapter(DigitsAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-digits"] = GetErrorMessage(context);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
