using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Security;

namespace AspNetCore.Components.Mvc;

public class SiteMapTests
{
    private IDictionary<String, Object?> route;
    private IAuthorization authorization;
    private ViewContext context;
    private SiteMap siteMap;

    public SiteMapTests()
    {
        context = HttpFactory.CreateViewContext();
        authorization = Substitute.For<IAuthorization>();
        siteMap = new SiteMap(CreateSiteMap(), authorization);
        route = context.RouteData.Values;
    }

    [Fact]
    public void For_NoAuthorization_ReturnsAllNodes()
    {
        authorization.IsGrantedFor(Arg.Any<Int64>(), Arg.Any<String>()).Returns(true);

        SiteMapNode[] actual = siteMap.For(context).ToArray();

        Assert.Single(actual);

        Assert.Null(actual[0].Action);
        Assert.Null(actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("Administration", actual[0].Path);
        Assert.Equal("fa fa-cogs", actual[0].IconClass);

        actual = actual[0].Children.ToArray();

        Assert.Equal(2, actual.Length);

        Assert.Empty(actual[0].Children);

        Assert.Equal("Index", actual[0].Action);
        Assert.Equal("Accounts", actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("fa fa-user", actual[0].IconClass);
        Assert.Equal("Administration/Accounts/Index", actual[0].Path);

        Assert.Null(actual[1].Action);
        Assert.Equal("Roles", actual[1].Controller);
        Assert.Equal("Administration", actual[1].Area);
        Assert.Equal("fa fa-users", actual[1].IconClass);
        Assert.Equal("Administration/Roles", actual[1].Path);

        actual = actual[1].Children.ToArray();

        Assert.Single(actual);
        Assert.Empty(actual[0].Children);

        Assert.Equal("Create", actual[0].Action);
        Assert.Equal("Roles", actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("far fa-file", actual[0].IconClass);
        Assert.Equal("Administration/Roles/Create", actual[0].Path);
    }

    [Fact]
    public void For_ReturnsAuthorizedNodes()
    {
        authorization.IsGrantedFor(context.HttpContext.User.Id(), "Administration").Returns(false);
        authorization.IsGrantedFor(context.HttpContext.User.Id(), "Administration/Accounts/Index").Returns(true);

        SiteMapNode[] actual = siteMap.For(context).ToArray();

        Assert.Single(actual);

        Assert.Null(actual[0].Action);
        Assert.Null(actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("Administration", actual[0].Path);
        Assert.Equal("fa fa-cogs", actual[0].IconClass);

        actual = actual[0].Children.ToArray();

        Assert.Single(actual);

        Assert.Empty(actual[0].Children);

        Assert.Equal("Index", actual[0].Action);
        Assert.Equal("Accounts", actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("fa fa-user", actual[0].IconClass);
        Assert.Equal("Administration/Accounts/Index", actual[0].Path);
    }

    [Fact]
    public void For_SetsActiveMenu()
    {
        route["action"] = "Edit";
        route["controller"] = "Roles";
        route["area"] = "Administration";

        authorization.IsGrantedFor(Arg.Any<Int64>(), Arg.Any<String>()).Returns(true);

        SiteMapNode[] actual = siteMap.For(context).ToArray();

        Assert.Single(actual);
        Assert.True(actual[0].IsActive);

        actual = actual[0].Children.ToArray();

        Assert.Equal(2, actual.Length);
        Assert.True(actual[1].IsActive);
        Assert.False(actual[0].IsActive);
        Assert.Empty(actual[0].Children);

        actual = actual[1].Children.ToArray();

        Assert.Single(actual);
        Assert.Empty(actual[0].Children);
        Assert.False(actual[0].IsActive);
    }

    [Fact]
    public void For_RemovesEmptyNodes()
    {
        authorization.IsGrantedFor(Arg.Any<Int64>(), Arg.Any<String>()).Returns(true);
        authorization.IsGrantedFor(context.HttpContext.User.Id(), "Administration/Roles/Create").Returns(false);

        SiteMapNode[] actual = siteMap.For(context).ToArray();

        Assert.Single(actual);

        Assert.Null(actual[0].Action);
        Assert.Null(actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("Administration", actual[0].Path);
        Assert.Equal("fa fa-cogs", actual[0].IconClass);

        actual = actual[0].Children.ToArray();

        Assert.Single(actual);

        Assert.Empty(actual[0].Children);

        Assert.Equal("Index", actual[0].Action);
        Assert.Equal("Accounts", actual[0].Controller);
        Assert.Equal("Administration", actual[0].Area);
        Assert.Equal("fa fa-user", actual[0].IconClass);
        Assert.Equal("Administration/Accounts/Index", actual[0].Path);
    }

    [Fact]
    public void BreadcrumbFor_IsCaseInsensitive()
    {
        route["controller"] = "profile";
        route["action"] = "edit";
        route["area"] = null;

        SiteMapNode[] actual = siteMap.BreadcrumbFor(context).ToArray();

        Assert.Equal(2, actual.Length);

        Assert.Equal("Home/Index", actual[0].Path);
        Assert.Equal("fa fa-home", actual[0].IconClass);

        Assert.Equal("Profile/Edit", actual[1].Path);
        Assert.Equal("fa fa-pencil-alt", actual[1].IconClass);
    }

    [Fact]
    public void BreadcrumbFor_NoAction_ReturnsEmpty()
    {
        route["controller"] = "profile";
        route["action"] = "edit";
        route["area"] = "area";

        Assert.Empty(siteMap.BreadcrumbFor(context));
    }

    private static String CreateSiteMap()
    {
        return @"<siteMap>
            <siteMapNode icon=""fa fa-home"" controller=""Home"" action=""Index"">
                <siteMapNode icon=""fa fa-user"" controller=""Profile"">
                    <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                </siteMapNode>
                <siteMapNode menu=""true"" icon=""fa fa-cogs"" area=""Administration"">
                    <siteMapNode menu=""true"" icon=""fa fa-user"" controller=""Accounts"" action=""Index"">
                        <siteMapNode icon=""fa fa-info"" action=""Details"" route-id=""id"">
                            <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                        </siteMapNode>
                    </siteMapNode>
                    <siteMapNode menu=""true"" icon=""fa fa-users"" controller=""Roles"">
                        <siteMapNode menu=""true"" icon=""far fa-file"" action=""Create"" />
                        <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                    </siteMapNode>
                </siteMapNode>
            </siteMapNode>
        </siteMap>";
    }
}
