using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace AspNetCore.Components.Mvc;

public class ValidationAdapterProvider : IValidationAttributeAdapterProvider
{
    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        return attribute switch
        {
            StringLengthAttribute stringLength => new StringLengthAdapter(stringLength),
            GreaterThanAttribute greaterThan => new GreaterThanAdapter(greaterThan),
            AcceptFilesAttribute acceptFiles => new AcceptFilesAdapter(acceptFiles),
            MinLengthAttribute minLength => new MinLengthAdapter(minLength),
            MaxLengthAttribute maxLength => new MaxLengthAdapter(maxLength),
            EmailAddressAttribute email => new EmailAddressAdapter(email),
            RequiredAttribute required => new RequiredAdapter(required),
            MaxValueAttribute maxValue => new MaxValueAdapter(maxValue),
            MinValueAttribute minValue => new MinValueAdapter(minValue),
            LessThanAttribute lessThan => new LessThanAdapter(lessThan),
            FileSizeAttribute fileSize => new FileSizeAdapter(fileSize),
            CompareAttribute compare => new CompareAdapter(compare),
            IntegerAttribute integer => new IntegerAdapter(integer),
            NumericAttribute numeric => new NumericAdapter(numeric),
            DigitsAttribute digits => new DigitsAdapter(digits),
            RangeAttribute range => new RangeAdapter(range),
            _ => null
        };
    }
}
