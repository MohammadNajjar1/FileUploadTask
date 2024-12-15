using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MinValueAdapterTests
{
    private MinValueAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public MinValueAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new MinValueAdapter(new MinValueAttribute(128));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.Int32Field));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_MinValue()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal("128", attributes["data-val-range-min"]);
        Assert.Equal(Validation.For("MinValue", context.ModelMetadata.PropertyName, 128), attributes["data-val-range"]);
    }

    [Fact]
    public void GetErrorMessage_MinValue()
    {
        String expected = Validation.For("MinValue", context.ModelMetadata.PropertyName, 128);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
