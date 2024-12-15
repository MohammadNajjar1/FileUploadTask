using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using AspNetCore.Components.Security;
using System.Security.Claims;

namespace AspNetCore.Components.Mvc;

public class AuthorizationFilterTests
{
    private AuthorizationFilter filter;
    private IAuthorization authorization;
    private AuthorizationFilterContext context;

    public AuthorizationFilterTests()
    {
        ActionContext action = new(Substitute.For<HttpContext>(), new RouteData(), new ActionDescriptor());
        context = new AuthorizationFilterContext(action, Array.Empty<IFilterMetadata>());
        authorization = Substitute.For<IAuthorization>();
        filter = new AuthorizationFilter(authorization);
    }

    [Fact]
    public void OnAuthorization_NotAuthenticated_SetsNullResult()
    {
        context.HttpContext.User.Identity?.IsAuthenticated.Returns(false);

        filter.OnAuthorization(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnAuthorization_NotAuthorized_ReturnsNotFoundView()
    {
        context.HttpContext.User.Identity?.IsAuthenticated.Returns(true);

        filter.OnAuthorization(context);

        ViewResult actual = Assert.IsType<ViewResult>(context.Result);

        Assert.Equal("~/Views/Home/NotFound.cshtml", actual.ViewName);
        Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
    }

    [Fact]
    public void OnAuthorization_IsAuthorized_SetsNullResult()
    {
        context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Returns(new Claim(ClaimTypes.NameIdentifier, "11000"));
        context.HttpContext.User.Identity?.IsAuthenticated.Returns(true);
        context.RouteData.Values["controller"] = "Controller";
        context.RouteData.Values["action"] = "Action";
        context.RouteData.Values["area"] = "Area";

        authorization.IsGrantedFor(11000, "Area/Controller/Action").Returns(true);

        filter.OnAuthorization(context);

        Assert.Null(context.Result);
    }
}
