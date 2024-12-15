using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class CompareAdapterTests
{
    private CompareAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public CompareAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        adapter = new CompareAdapter(new CompareAttribute(nameof(AllTypesView.StringField)));
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_EqualTo()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal(adapter.Attribute.OtherProperty, attributes["data-val-equalto-other"]);
        Assert.Equal(Validation.For("Compare", context.ModelMetadata.PropertyName, adapter.Attribute.OtherProperty), attributes["data-val-equalto"]);
    }

    [Fact]
    public void GetErrorMessage_EqualTo()
    {
        String expected = Validation.For("Compare", context.ModelMetadata.PropertyName, adapter.Attribute.OtherProperty);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
