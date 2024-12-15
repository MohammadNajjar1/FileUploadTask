using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class DateValidatorTests
{
    [Fact]
    public void AddValidation_Date()
    {
        Dictionary<String, String> attributes = new();
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForType(typeof(DateTime));
        ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

        new DateValidator().AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-date", Validation.For("DateTime", "DateTime"));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }
}
