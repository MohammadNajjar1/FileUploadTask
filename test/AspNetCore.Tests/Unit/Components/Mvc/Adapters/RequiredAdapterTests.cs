using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class RequiredAdapterTests
{
    private RequiredAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public RequiredAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new RequiredAdapter(new RequiredAttribute());
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void RequiredAdapter_Message()
    {
        String? actual = new RequiredAdapter(new RequiredAttribute()).Attribute.ErrorMessage;
        String expected = Validation.For("Required");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddValidation_Required()
    {
        adapter.AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-required", Validation.For("Required", context.ModelMetadata.PropertyName));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetErrorMessage_Required()
    {
        String expected = Validation.For("Required", context.ModelMetadata.PropertyName);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
