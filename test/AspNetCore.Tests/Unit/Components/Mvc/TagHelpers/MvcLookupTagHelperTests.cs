using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

public class MvcLookupTagHelperTests
{
    [Theory]
    [InlineData("Test", "Test")]
    [InlineData(null, "~/Lookup/Handling")]
    public void Process_Url(String? url, String fullUrl)
    {
        ViewContext view = new();
        TagHelperContent content = new DefaultTagHelperContent();
        IUrlHelperFactory factory = HttpFactory.CreateUrlHelperFactory(view);
        MvcLookupTagHelper helper = new(Substitute.For<IHtmlGenerator>(), factory) { ViewContext = view };
        TagHelperOutput output = new("div", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
        TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");

        helper.Url = url;
        helper.Handler = "Handling";

        helper.Process(context, output);

        Assert.Equal(fullUrl, helper.Url);
    }

    [Theory]
    [InlineData(null, "Roles")]
    [InlineData("Testing", "Testing")]
    public void Process_Title(String? title, String newTitle)
    {
        TagHelperContent content = new DefaultTagHelperContent();
        TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        TagHelperOutput output = new("div", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
        MvcLookupTagHelper helper = new(Substitute.For<IHtmlGenerator>(), Substitute.For<IUrlHelperFactory>());

        helper.Title = title;
        helper.Handler = "Role";

        helper.Process(context, output);

        Assert.Equal(newTitle, helper.Title);
    }
}
