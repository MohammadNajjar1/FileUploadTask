using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class NumberValidatorTests
{
    [Fact]
    public void AddValidation_Number()
    {
        Dictionary<String, String> attributes = new();
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
        ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

        new NumberValidator().AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-number", Validation.For("Number", "Int64"));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddValidation_ExistingNumber()
    {
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
        Dictionary<String, String> attributes = new() { ["data-val-number"] = "Test" };
        ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

        new NumberValidator().AddValidation(context);

        Assert.Equal(KeyValuePair.Create("data-val-number", "Test"), attributes.Single());
    }
}
