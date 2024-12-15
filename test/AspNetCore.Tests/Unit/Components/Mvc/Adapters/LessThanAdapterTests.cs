using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class LessThanAdapterTests
{
    private LessThanAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public LessThanAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new LessThanAdapter(new LessThanAttribute(128));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.Int32Field));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_LessThan()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal("128", attributes["data-val-lower-than"]);
        Assert.Equal(Validation.For("LessThan", context.ModelMetadata.PropertyName, 128), attributes["data-val-lower"]);
    }

    [Fact]
    public void GetErrorMessage_LessThan()
    {
        String expected = Validation.For("LessThan", context.ModelMetadata.PropertyName, 128);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
