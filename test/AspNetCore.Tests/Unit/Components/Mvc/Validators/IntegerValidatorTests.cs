using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class IntegerValidatorTests
{
    [Fact]
    public void AddValidation_Integer()
    {
        Dictionary<String, String> attributes = new();
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
        ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

        new IntegerValidator().AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-integer", Validation.For("Integer", "Int64"));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddValidation_ExistingInteger()
    {
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
        Dictionary<String, String> attributes = new() { ["data-val-integer"] = "Test" };
        ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

        new IntegerValidator().AddValidation(context);

        Assert.Equal(KeyValuePair.Create("data-val-integer", "Test"), attributes.Single());
    }
}
