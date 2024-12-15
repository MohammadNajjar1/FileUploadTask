using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Components.Mvc;

public class ViewLocationExpanderTests
{
    [Fact]
    public void ExpandViewLocations_Area_ReturnsAreaLocations()
    {
        ActionContext context = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        ViewLocationExpanderContext expander = new(context, "Index", null, null, null, true);
        context.RouteData.Values["area"] = "Test";

        IEnumerable<String> expected = new[] { "/Views/{2}/Shared/{0}.cshtml", "/Views/{2}/{1}/{0}.cshtml", "/Views/Shared/{0}.cshtml" };
        IEnumerable<String> actual = new ViewLocationExpander().ExpandViewLocations(expander, Array.Empty<String>());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ExpandViewLocations_ReturnsViewLocations()
    {
        ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        ViewLocationExpanderContext context = new(action, "Index", null, null, null, true);
        ViewLocationExpander expander = new();

        IEnumerable<String> expected = new[] { "/Views/{1}/{0}.cshtml", "/Views/Shared/{0}.cshtml" };
        IEnumerable<String> actual = expander.ExpandViewLocations(context, Array.Empty<String>());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PopulateValues_DoesNothing()
    {
        ActionContext context = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        ViewLocationExpanderContext expander = new(context, "Index", null, null, null, true);

        new ViewLocationExpander().PopulateValues(expander);
    }
}
