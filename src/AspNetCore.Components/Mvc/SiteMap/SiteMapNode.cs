using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Components.Mvc;

public class SiteMapNode
{
    public String Path { get; set; }
    public Boolean IsMenu { get; set; }
    public Boolean IsActive { get; set; }
    public String? IconClass { get; set; }

    public String? Area { get; set; }
    public String? Action { get; set; }
    public String? Controller { get; set; }
    public Dictionary<String, String> Route { get; set; }

    public SiteMapNode? Parent { get; set; }
    public SiteMapNode[] Children { get; set; }

    public SiteMapNode()
    {
        Path = "";
        Children = Array.Empty<SiteMapNode>();
        Route = new Dictionary<String, String>();
    }

    public String Form(IUrlHelper url)
    {
        if (Action == null)
            return "#";

        Dictionary<String, Object?> route = new();
        ActionContext context = url.ActionContext;
        route["area"] = Area;

        foreach ((String key, String newKey) in Route)
            route[key] = context.RouteData.Values[newKey] ?? context.HttpContext.Request.Query[newKey];

        return url.Action(Action, Controller, route) ?? "#";
    }
    public SiteMapNode[] ToBreadcrumb()
    {
        SiteMapNode? node = this;
        List<SiteMapNode> breadcrumb = new();

        while (node != null)
        {
            if (node.Action != null)
                breadcrumb.Insert(0, node);

            node = node.Parent;
        }

        return breadcrumb.ToArray();
    }
}
