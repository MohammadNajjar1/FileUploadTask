using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Components.Mvc;

public class NotTrimmedAttributeTests
{
    [Fact]
    public void NotTrimmedAttribute_SetsBinderType()
    {
        Assert.Equal(typeof(NotTrimmedAttribute), new NotTrimmedAttribute().BinderType);
    }

    [Fact]
    public async Task BindModelAsync_DoesNotTrimValue()
    {
        DefaultModelBindingContext context = new()
        {
            ModelName = "Test",
            ModelState = new ModelStateDictionary(),
            ActionContext = HttpFactory.CreateViewContext(),
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String)),
            ValueProvider = new RouteValueProvider(BindingSource.Path, new RouteValueDictionary(new { Test = " Value  " }))
        };

        await new NotTrimmedAttribute().BindModelAsync(context);

        Assert.Equal(ModelBindingResult.Success(" Value  "), context.Result);
    }
}
