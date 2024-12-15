using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

public class FormSelectTagHelperTests
{
    private FormSelectTagHelper helper;
    private TagHelperContext context;
    private TagHelperOutput output;

    public FormSelectTagHelperTests()
    {
        TagHelperContent content = new DefaultTagHelperContent();
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));

        context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        output = new TagHelperOutput("select", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
        helper = new FormSelectTagHelper { For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null)) };
    }

    [Theory]
    [InlineData("", "form-select ")]
    [InlineData(null, "form-select ")]
    [InlineData("test", "form-select test")]
    public void Process_Class(String? value, String classes)
    {
        output.Attributes.Add("class", value);

        helper.Process(context, output);

        Assert.Single(output.Attributes);
        Assert.Empty(output.Content.GetContent());
        Assert.Equal(classes, output.Attributes["class"].Value);
    }

    [Theory]
    [InlineData(typeof(Boolean), "")]
    [InlineData(typeof(Boolean?), "<option></option>")]
    public void Process_Required(Type type, String content)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
        helper = new FormSelectTagHelper { For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null)) };

        helper.Process(context, output);

        Assert.Equal(content, output.PreContent.GetContent());
    }

    [Fact]
    public void Process_Input()
    {
        helper.Process(context, output);

        Assert.Single(output.Attributes);
        Assert.Empty(output.Content.GetContent());
        Assert.Equal("form-select", output.Attributes["class"].Value);
    }
}
