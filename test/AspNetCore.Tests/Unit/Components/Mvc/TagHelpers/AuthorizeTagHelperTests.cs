using Microsoft.AspNetCore.Razor.TagHelpers;
using AspNetCore.Components.Security;
using System.Security.Claims;

namespace AspNetCore.Components.Mvc;

public class AuthorizeTagHelperTests
{
    private IAuthorization authorization;
    private AuthorizeTagHelper helper;
    private TagHelperContext context;
    private TagHelperOutput output;

    public AuthorizeTagHelperTests()
    {
        authorization = Substitute.For<IAuthorization>();
        TagHelperContent content = new DefaultTagHelperContent();
        helper = new AuthorizeTagHelper(authorization) { ViewContext = HttpFactory.CreateViewContext() };
        context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
        output = new TagHelperOutput("authorize", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
    }

    [Theory]
    [InlineData("A/B/C", "A", "B", "C", "D", "E", "F")]
    [InlineData("A/B/C", null, null, null, "A", "B", "C")]
    public void Process_NotAuthorized_SuppressesOutput(
        String permission,
        String? area, String? controller, String? action,
        String routeArea, String routeController, String routeAction)
    {
        authorization.IsGrantedFor(Arg.Any<Int64>(), Arg.Any<String>()).Returns(true);
        authorization.IsGrantedFor(1, permission).Returns(false);

        (helper.ViewContext!.HttpContext.User.Identity as ClaimsIdentity)!.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
        helper.ViewContext.RouteData.Values["controller"] = routeController;
        helper.ViewContext.RouteData.Values["action"] = routeAction;
        helper.ViewContext.RouteData.Values["area"] = routeArea;

        output.PostContent.SetContent("PostContent");
        output.PostElement.SetContent("PostElement");
        output.PreContent.SetContent("PreContent");
        output.PreElement.SetContent("PreElement");
        output.Content.SetContent("Content");
        output.TagName = "TagName";

        helper.Controller = controller;
        helper.Action = action;
        helper.Area = area;

        helper.Process(context, output);

        Assert.Empty(output.PostContent.GetContent());
        Assert.Empty(output.PostElement.GetContent());
        Assert.Empty(output.PreContent.GetContent());
        Assert.Empty(output.PreElement.GetContent());
        Assert.Empty(output.Content.GetContent());
        Assert.Null(output.TagName);
    }

    [Theory]
    [InlineData("A/B/C", "A", "B", "C", "D", "E", "F")]
    [InlineData("A/B/C", null, null, null, "A", "B", "C")]
    public void Process_RemovesWrappingTag(
        String permission,
        String? area, String? controller, String? action,
        String? routeArea, String? routeController, String? routeAction)
    {
        authorization.IsGrantedFor(1, Arg.Any<String>()).Returns(false);
        authorization.IsGrantedFor(1, permission).Returns(true);

        (helper.ViewContext!.HttpContext.User.Identity as ClaimsIdentity)!.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
        helper.ViewContext.RouteData.Values["controller"] = routeController;
        helper.ViewContext.RouteData.Values["action"] = routeAction;
        helper.ViewContext.RouteData.Values["area"] = routeArea;

        output.PostContent.SetContent("PostContent");
        output.PostElement.SetContent("PostElement");
        output.PreContent.SetContent("PreContent");
        output.PreElement.SetContent("PreElement");
        output.Content.SetContent("Content");
        output.TagName = "TagName";

        helper.Controller = controller;
        helper.Action = action;
        helper.Area = area;

        helper.Process(context, output);

        Assert.Equal("PostContent", output.PostContent.GetContent());
        Assert.Equal("PostElement", output.PostElement.GetContent());
        Assert.Equal("PreContent", output.PreContent.GetContent());
        Assert.Equal("PreElement", output.PreElement.GetContent());
        Assert.Equal("Content", output.Content.GetContent());
        Assert.Null(output.TagName);
    }
}
