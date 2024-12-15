using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MinLengthAdapter : AttributeAdapterBase<MinLengthAttribute>
{
    public MinLengthAdapter(MinLengthAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-minlength"] = GetErrorMessage(context);
        context.Attributes["data-val-minlength-min"] = Attribute.Length.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return Validation.For("MinLength", validationContext.ModelMetadata.GetDisplayName(), Attribute.Length);
    }
}
