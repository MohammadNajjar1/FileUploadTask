using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class RangeAdapterTests
{
    private RangeAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public RangeAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        CultureInfo.CurrentCulture = new CultureInfo("lt");
        adapter = new RangeAdapter(new RangeAttribute(4.3, 128.4));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.DoubleField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_Range()
    {
        adapter.AddValidation(context);

        Assert.Equal(3, attributes.Count);
        Assert.Equal("4.3", attributes["data-val-range-min"]);
        Assert.Equal("128.4", attributes["data-val-range-max"]);
        Assert.Equal(Validation.For("Range", context.ModelMetadata.PropertyName, 4.3, 128.4), attributes["data-val-range"]);
    }

    [Fact]
    public void GetErrorMessage_Range()
    {
        String expected = Validation.For("Range", context.ModelMetadata.PropertyName, 4.3, 128.4);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(Validation.For("Range"), adapter.Attribute.ErrorMessage);
        Assert.Equal(expected, actual);
    }
}
