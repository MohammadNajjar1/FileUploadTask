using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class FileSizeAdapterTests
{
    private FileSizeAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public FileSizeAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        adapter = new FileSizeAdapter(new FileSizeAttribute(12.25));
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.FileField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_FileSize()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal("12845056.00", attributes["data-val-filesize-max"]);
        Assert.Equal(Validation.For("FileSize", context.ModelMetadata.PropertyName, 12.25), attributes["data-val-filesize"]);
    }

    [Fact]
    public void GetErrorMessage_FileSize()
    {
        String expected = Validation.For("FileSize", context.ModelMetadata.PropertyName, 12.25);
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
