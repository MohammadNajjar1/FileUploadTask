using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class IntegerAdapterTests
{
    private IntegerAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public IntegerAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new IntegerAdapter(new IntegerAttribute());
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_Integer()
    {
        adapter.AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-integer", Validation.For("Integer", context.ModelMetadata.PropertyName));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetErrorMessage_Integer()
    {
        String expected = Validation.For("Integer", context.ModelMetadata.PropertyName);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
