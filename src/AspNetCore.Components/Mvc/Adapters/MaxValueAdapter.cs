using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class MaxValueAdapter : AttributeAdapterBase<MaxValueAttribute>
{
    public MaxValueAdapter(MaxValueAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-range"] = GetErrorMessage(context);
        context.Attributes["data-val-range-max"] = Attribute.Maximum.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
