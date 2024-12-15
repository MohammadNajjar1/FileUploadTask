using Microsoft.AspNetCore.Http;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class FileSizeAttribute : ValidationAttribute
{
    public Decimal MaximumMB { get; }

    public FileSizeAttribute(Double maximumMB)
        : base(() => Validation.For("FileSize"))
    {
        MaximumMB = Convert.ToDecimal(maximumMB);
    }

    public override String FormatErrorMessage(String name)
    {
        return String.Format(ErrorMessageString, name, MaximumMB);
    }
    public override Boolean IsValid(Object? value)
    {
        IEnumerable<IFormFile>? files = value is IFormFile formFile ? new[] { formFile } : value as IEnumerable<IFormFile>;

        return files == null || files.Sum(file => file.Length) <= MaximumMB * 1024 * 1024;
    }
}
