using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class FileSizeAdapter : AttributeAdapterBase<FileSizeAttribute>
{
    public FileSizeAdapter(FileSizeAttribute attribute)
        : base(attribute, null)
    {
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val-filesize"] = GetErrorMessage(context);
        context.Attributes["data-val-filesize-max"] = (Attribute.MaximumMB * 1024 * 1024).ToString(CultureInfo.InvariantCulture);
    }
    public override String GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return GetErrorMessage(validationContext.ModelMetadata);
    }
}
