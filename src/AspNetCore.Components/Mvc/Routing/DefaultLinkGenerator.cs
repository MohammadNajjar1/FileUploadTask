using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Components.Mvc;

public class DefaultLinkGenerator : LinkGenerator
{
    private LinkGenerator Generator { get; }

    public DefaultLinkGenerator(LinkGenerator generator)
    {
        Generator = generator;
    }

    public override String? GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
    {
        if (!values.ContainsKey("language") && ambientValues?.ContainsKey("language") == true)
            values["language"] = ambientValues["language"];

        return Generator.GetPathByAddress(httpContext, address, values, ambientValues, pathBase, fragment, options);
    }

    public override String? GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
    {
        return Generator.GetPathByAddress(address, values, pathBase, fragment, options);
    }

    public override String? GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, String? scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
    {
        if (!values.ContainsKey("language") && ambientValues?.ContainsKey("language") == true)
            values["language"] = ambientValues["language"];

        return Generator.GetUriByAddress(httpContext, address, values, ambientValues, scheme, host, pathBase, fragment, options);
    }

    public override String? GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, String? scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
    {
        return Generator.GetUriByAddress(address, values, scheme, host, pathBase, fragment, options);
    }
}
