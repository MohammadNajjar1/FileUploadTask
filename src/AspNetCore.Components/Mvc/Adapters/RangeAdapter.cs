using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class RangeAdapter : AttributeAdapterBase<RangeAttribute>
{
    public RangeAdapter(RangeAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-range"] = GetErrorMessage(context);
        context.Attributes["data-val-range-min"] = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture)!;
        context.Attributes["data-val-range-max"] = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture)!;
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return Validation.For("Range", validationContext.ModelMetadata.GetDisplayName(), Attribute.Minimum, Attribute.Maximum);
    }
}
