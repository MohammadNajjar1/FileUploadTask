using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MaxLengthAdapterTests
{
    private MaxLengthAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public MaxLengthAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new MaxLengthAdapter(new MaxLengthAttribute(128));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_MaxLength()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal("128", attributes["data-val-length-max"]);
        Assert.Equal(Validation.For("MaxLength", context.ModelMetadata.PropertyName, 128), attributes["data-val-length"]);
    }

    [Fact]
    public void GetErrorMessage_MaxLength()
    {
        String expected = Validation.For("MaxLength", context.ModelMetadata.PropertyName, 128);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(Validation.For("MaxLength"), adapter.Attribute.ErrorMessage);
        Assert.Equal(expected, actual);
    }
}
