using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class AcceptFilesAdapter : AttributeAdapterBase<AcceptFilesAttribute>
{
    public AcceptFilesAdapter(AcceptFilesAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-accept"] = GetErrorMessage(context);
        context.Attributes["data-val-accept-types"] = Attribute.Extensions;
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
