using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MinLengthAdapterTests
{
    private MinLengthAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public MinLengthAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new MinLengthAdapter(new MinLengthAttribute(128));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_MinLength()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal("128", attributes["data-val-minlength-min"]);
        Assert.Equal(Validation.For("MinLength", context.ModelMetadata.PropertyName, 128), attributes["data-val-minlength"]);
    }

    [Fact]
    public void GetErrorMessage_MinLength()
    {
        String expected = Validation.For("MinLength", context.ModelMetadata.PropertyName, 128);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(Validation.For("MinLength"), adapter.Attribute.ErrorMessage);
        Assert.Equal(expected, actual);
    }
}
