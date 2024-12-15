using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class AcceptFilesAdapterTests
{
    private AcceptFilesAdapter adapter;
    private ClientModelValidationContext context;
    private Dictionary<String, String> attributes;

    public AcceptFilesAdapterTests()
    {
        attributes = new Dictionary<String, String>();
        IModelMetadataProvider provider = new EmptyModelMetadataProvider();
        adapter = new AcceptFilesAdapter(new AcceptFilesAttribute(".docx,.rtf"));
        ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), nameof(AllTypesView.FileField));

        context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
    }

    [Fact]
    public void AddValidation_AcceptFiles()
    {
        adapter.AddValidation(context);

        Assert.Equal(2, attributes.Count);
        Assert.Equal(".docx,.rtf", attributes["data-val-accept-types"]);
        Assert.Equal(Validation.For("AcceptFiles", context.ModelMetadata.PropertyName, ".docx,.rtf"), attributes["data-val-accept"]);
    }

    [Fact]
    public void GetErrorMessage_AcceptFiles()
    {
        String expected = Validation.For("AcceptFiles", context.ModelMetadata.PropertyName, ".docx,.rtf");
        String actual = adapter.GetErrorMessage(context);

        Assert.Equal(expected, actual);
    }
}
