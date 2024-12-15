using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AspNetCore.Components.Tree;

namespace AspNetCore.Components.Mvc;

public class MvcTreeTagHelperTests
{
    private TagHelperContext context;
    private MvcTreeTagHelper helper;
    private TagHelperOutput output;

    public MvcTreeTagHelperTests()
    {
        MvcTree tree = new();
        TagHelperContent content = new DefaultTagHelperContent();
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(MvcTree));

        tree.SelectedIds.Add(123456);
        tree.Nodes.Add(new MvcTreeNode("Test"));
        tree.Nodes[0].Children.Add(new MvcTreeNode(4567, "Test2"));
        tree.Nodes[0].Children.Add(new MvcTreeNode(123456, "Test1"));

        output = new TagHelperOutput("div", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
        context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        helper = new MvcTreeTagHelper { For = new ModelExpression("Test", new ModelExplorer(metadata, metadata, tree)) };
    }

    [Fact]
    public void Process_AddsDataForAttribute()
    {
        helper.Process(context, output);

        Assert.Equal("Test.SelectedIds", output.Attributes["data-for"].Value);
    }

    [Theory]
    [InlineData(false, "mvc-tree")]
    [InlineData(true, "mvc-tree mvc-tree-readonly")]
    public void Process_AddsClasses(Boolean isReadonly, String classes)
    {
        helper.Readonly = isReadonly;

        helper.Process(context, output);

        Assert.Equal(classes, output.Attributes["class"].Value);
    }

    [Theory]
    [InlineData(false, "mvc-tree test")]
    [InlineData(true, "mvc-tree mvc-tree-readonly test")]
    public void Process_AppendsClasses(Boolean isReadonly, String classes)
    {
        output.Attributes.Add("class", "test");

        helper.Readonly = isReadonly;

        helper.Process(context, output);

        Assert.Equal(classes, output.Attributes["class"].Value);
    }

    [Fact]
    public void Process_BuildsTree()
    {
        helper.Process(context, output);

        String expected =
            "<div class=\"mvc-tree-ids\">" +
                "<input name=\"Test.SelectedIds\" type=\"hidden\" value=\"123456\" />" +
            "</div>" +
            "<ul class=\"mvc-tree-view\">" +
                "<li class=\"mvc-tree-branch\">" +
                    "<i></i> <a href=\"#\">Test</a>" +
                    "<ul>" +
                        "<li data-id=\"4567\">" +
                            "<i></i> <a href=\"#\">Test2</a>" +
                        "</li>" +
                        "<li class=\"mvc-tree-checked\" data-id=\"123456\">" +
                            "<i></i> <a href=\"#\">Test1</a>" +
                        "</li>" +
                    "</ul>" +
                "</li>" +
            "</ul>";
        String actual = output.Content.GetContent();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Process_BuildsCollapsedTree()
    {
        helper.HideDepth = 1;

        helper.Process(context, output);

        String expected =
            "<div class=\"mvc-tree-ids\">" +
                "<input name=\"Test.SelectedIds\" type=\"hidden\" value=\"123456\" />" +
            "</div>" +
            "<ul class=\"mvc-tree-view\">" +
                "<li class=\"mvc-tree-branch mvc-tree-collapsed\">" +
                    "<i></i> <a href=\"#\">Test</a>" +
                    "<ul>" +
                        "<li data-id=\"4567\">" +
                            "<i></i> <a href=\"#\">Test2</a>" +
                        "</li>" +
                        "<li class=\"mvc-tree-checked\" data-id=\"123456\">" +
                            "<i></i> <a href=\"#\">Test1</a>" +
                        "</li>" +
                    "</ul>" +
                "</li>" +
            "</ul>";
        String actual = output.Content.GetContent();

        Assert.Equal(expected, actual);
    }
}
