using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

public class FormInputTagHelperTests
{
    private FormInputTagHelper helper;
    private TagHelperContext context;
    private TagHelperOutput output;

    public FormInputTagHelperTests()
    {
        TagHelperContent content = new DefaultTagHelperContent();
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));

        context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        output = new TagHelperOutput("input", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
        helper = new FormInputTagHelper { For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null)) };
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("on", "on")]
    [InlineData(null, null)]
    [InlineData("off", "off")]
    public void Process_Autocomplete(String? value, String? autocomplete)
    {
        output.Attributes.Add("autocomplete", value);

        helper.Process(context, output);

        Assert.Equal(3, output.Attributes.Count);
        Assert.Empty(output.Content.GetContent());
        Assert.Empty(output.PreElement.GetContent());
        Assert.Empty(output.PostElement.GetContent());
        Assert.Equal("text", output.Attributes["type"].Value);
        Assert.Equal("form-control", output.Attributes["class"].Value);
        Assert.Equal(autocomplete, output.Attributes["autocomplete"].Value);
    }

    [Theory]
    [InlineData("", "form-control")]
    [InlineData(null, "form-control")]
    [InlineData("test", "form-control test")]
    public void Process_TextClass(String? value, String classes)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));
        helper.For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null));
        output.Attributes.Add("class", value);

        helper.Process(context, output);

        Assert.Equal(3, output.Attributes.Count);
        Assert.Empty(output.Content.GetContent());
        Assert.Empty(output.PreElement.GetContent());
        Assert.Empty(output.PostElement.GetContent());
        Assert.Equal("text", output.Attributes["type"].Value);
        Assert.Equal(classes, output.Attributes["class"].Value);
        Assert.Equal("off", output.Attributes["autocomplete"].Value);
    }

    [Theory]
    [InlineData("", "form-check-input")]
    [InlineData(null, "form-check-input")]
    [InlineData("test", "form-check-input test")]
    public void Process_BooleanClass(String? value, String classes)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(Boolean));
        helper.For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null));
        output.Attributes.Add("class", value);

        helper.Process(context, output);

        Assert.Equal(2, output.Attributes.Count);
        Assert.Empty(output.Content.GetContent());
        Assert.Empty(output.PreElement.GetContent());
        Assert.Empty(output.PostElement.GetContent());
        Assert.Equal(classes, output.Attributes["class"].Value);
        Assert.Equal("off", output.Attributes["autocomplete"].Value);
    }

    [Theory]
    [InlineData("date-picker")]
    [InlineData("date-time-picker")]
    public void Process_Datepicker(String classes)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(DateTime));
        helper.For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, null));
        output.Attributes.Add("class", classes);

        helper.Process(context, output);

        Assert.Equal(3, output.Attributes.Count);
        Assert.Empty(output.Content.GetContent());
        Assert.Equal("text", output.Attributes["type"].Value);
        Assert.Equal("off", output.Attributes["autocomplete"].Value);
        Assert.Equal($"form-control {classes}", output.Attributes["class"].Value);
        Assert.Equal("<div class=\"input-group\">", output.PreElement.GetContent());
        Assert.Equal("<button class=\"date-picker-browser input-group-text fas fa-calendar-alt\" type=\"button\"></button></div>", output.PostElement.GetContent());
    }

    [Fact]
    public void Process_Input()
    {
        helper.Process(context, output);

        Assert.Equal(3, output.Attributes.Count);
        Assert.Empty(output.Content.GetContent());
        Assert.Empty(output.PreElement.GetContent());
        Assert.Empty(output.PostElement.GetContent());
        Assert.Equal("text", output.Attributes["type"].Value);
        Assert.Equal("off", output.Attributes["autocomplete"].Value);
        Assert.Equal("form-control", output.Attributes["class"].Value);
    }
}
