using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class EmailAddressAdapterTests
{
    private EmailAddressAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public EmailAddressAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new EmailAddressAdapter(new EmailAddressAttribute());
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.StringField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_Email()
    {
        adapter.AddValidation(context);

        KeyValuePair<String, String> expected = KeyValuePair.Create("data-val-email", Validation.For("EmailAddress", context.ModelMetadata.PropertyName));
        KeyValuePair<String, String> actual = attributes.Single();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetErrorMessage_Email()
    {
        String expected = Validation.For("EmailAddress", context.ModelMetadata.PropertyName);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
