using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MaxLengthAdapter : AttributeAdapterBase<MaxLengthAttribute>
{
    public MaxLengthAdapter(MaxLengthAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-length"] = GetErrorMessage(context);
        context.Attributes["data-val-length-max"] = Attribute.Length.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return Validation.For("MaxLength", validationContext.ModelMetadata.GetDisplayName(), Attribute.Length);
    }
}
