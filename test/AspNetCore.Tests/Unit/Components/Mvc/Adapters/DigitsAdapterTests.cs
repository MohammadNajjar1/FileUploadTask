using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class DigitsAdapterTests
{
    private DigitsAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public DigitsAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new DigitsAdapter(new DigitsAttribute());
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_Digits()
    {
        adapter.AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-digits", Validation.For("Digits", context.ModelMetadata.PropertyName));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetErrorMessage_Digits()
    {
        String expected = Validation.For("Digits", context.ModelMetadata.PropertyName);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
