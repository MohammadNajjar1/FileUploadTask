using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class StringLengthAdapter : AttributeAdapterBase<StringLengthAttribute>
{
    public StringLengthAdapter(StringLengthAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-length"] = GetErrorMessage(context);
        context.Attributes["data-val-length-max"] = Attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);

        if (Attribute.MinimumLength > 0)
            context.Attributes["data-val-length-min"] = Attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        if (Attribute.MinimumLength == 0)
            return Validation.For("StringLength", validationContext.ModelMetadata.GetDisplayName(), Attribute.MaximumLength);

        return Validation.For("StringLengthRange", validationContext.ModelMetadata.GetDisplayName(), Attribute.MaximumLength, Attribute.MinimumLength);
    }
}
