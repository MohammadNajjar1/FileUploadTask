using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

public class FormLabelTagHelperTests
{
    [Theory]
    [InlineData(typeof(Int64), null, "*")]
    [InlineData(typeof(Int64), true, "*")]
    [InlineData(typeof(Int64), false, "")]
    [InlineData(typeof(Int64?), null, "")]
    [InlineData(typeof(Int64?), true, "*")]
    [InlineData(typeof(Int64?), false, "")]
    [InlineData(typeof(Boolean), null, "")]
    [InlineData(typeof(Boolean), true, "*")]
    [InlineData(typeof(Boolean), false, "")]
    [InlineData(typeof(Boolean?), null, "")]
    [InlineData(typeof(Boolean?), true, "*")]
    [InlineData(typeof(Boolean?), false, "")]
    public void Process_Label(Type type, Boolean? required, String symbol)
    {
        FormLabelTagHelper helper = new();
        TagHelperContent content = new DefaultTagHelperContent();
        TagHelperAttribute[] attributes = { new("for", "Test") };
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
        TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        TagHelperOutput output = new("label", new TagHelperAttributeList(attributes), (_, _) => Task.FromResult(content));

        helper.For = new ModelExpression("Total.Sum", new ModelExplorer(metadata, metadata, null));
        helper.Required = required;

        helper.Process(context, output);

        Assert.Equal("Test", output.Attributes["for"].Value);
        Assert.Equal($"<span class=\"require\">{symbol}</span>", output.Content.GetContent());
    }
}
