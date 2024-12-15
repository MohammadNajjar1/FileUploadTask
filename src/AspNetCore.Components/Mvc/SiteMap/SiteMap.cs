using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Security;

namespace AspNetCore.Components.Mvc;

public class SiteMap : ISiteMap
{
    private SiteMapNode[] Tree { get; }
    private IAuthorization Authorization { get; }
    private Dictionary<String, SiteMapNode[]> Breadcrumb { get; }

    public SiteMap(String map, IAuthorization authorization)
    {
        Tree = Parse(XElement.Parse(map), null);
        Authorization = authorization;
        Breadcrumb = Flatten(Tree)
            .ToDictionary(
                node => node.Path,
                node => node.ToBreadcrumb(),
                StringComparer.OrdinalIgnoreCase);
    }

    public SiteMapNode[] For(ViewContext context)
    {
        return Authorize(context.HttpContext.User.Id(), SetState(null, Tree, GetPath(context)));
    }
    public SiteMapNode[] BreadcrumbFor(ViewContext context)
    {
        return Breadcrumb.TryGetValue(GetPath(context), out SiteMapNode[]? breadcrumb) ? breadcrumb : Array.Empty<SiteMapNode>();
    }

    private SiteMapNode[] SetState(SiteMapNode? parent, SiteMapNode[] nodes, String current)
    {
        List<SiteMapNode> copies = new();

        foreach (SiteMapNode node in nodes)
        {
            SiteMapNode copy = new();
            copy.IconClass = node.IconClass;
            copy.IsMenu = node.IsMenu;
            copy.Path = node.Path;
            copy.Parent = parent;

            copy.Controller = node.Controller;
            copy.Action = node.Action;
            copy.Area = node.Area;

            copy.IsActive = String.Equals(node.Path, current, StringComparison.OrdinalIgnoreCase);
            copy.Children = SetState(copy, node.Children, current);

            if (parent?.IsActive == false)
                parent.IsActive = copy.IsActive;

            copies.Add(copy);
        }

        return copies.ToArray();
    }
    private SiteMapNode[] Authorize(Int64 accountId, SiteMapNode[] menu)
    {
        List<SiteMapNode> authorized = new();

        foreach (SiteMapNode node in menu)
        {
            node.Children = Authorize(accountId, node.Children);

            if (!IsEmpty(node) && (node.Action == null || Authorization.IsGrantedFor(accountId, node.Path)))
                authorized.Add(node);
            else
                authorized.AddRange(node.Children);
        }

        return authorized.ToArray();
    }
    private SiteMapNode[] Parse(XContainer root, SiteMapNode? parent)
    {
        List<SiteMapNode> nodes = new();

        foreach (XElement element in root.Elements("siteMapNode"))
        {
            SiteMapNode node = new();
            node.IconClass = (String?)element.Attribute("icon");
            node.IsMenu = (Boolean?)element.Attribute("menu") == true;

            node.Route = ParseRoute(element);
            node.Action = (String?)element.Attribute("action");
            node.Area = (String?)element.Attribute("area") ?? parent?.Area;
            node.Controller = (String?)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null);

            node.Path = $"{node.Area}/{node.Controller}/{node.Action}".Replace("//", "/").Trim('/');
            node.Children = Parse(element, node);
            node.Parent = parent;

            nodes.Add(node);
        }

        return nodes.ToArray();
    }
    private Dictionary<String, String> ParseRoute(XElement element)
    {
        return element
            .Attributes()
            .Where(attribute => attribute.Name.LocalName.StartsWith("route-"))
            .ToDictionary(attribute => attribute.Name.LocalName[6..], attribute => attribute.Value);
    }
    private List<SiteMapNode> Flatten(SiteMapNode[] branches)
    {
        List<SiteMapNode> list = new();

        foreach (SiteMapNode branch in branches)
        {
            list.Add(branch);
            list.AddRange(Flatten(branch.Children));
        }

        return list;
    }
    private String GetPath(ViewContext context)
    {
        RouteValueDictionary route = context.RouteData.Values;
        String? controller = route["controller"] as String;
        String? action = route["action"] as String;
        String? area = route["area"] as String;

        return $"{area}/{controller}/{action}".Trim('/');
    }
    private Boolean IsEmpty(SiteMapNode node)
    {
        return !node.IsMenu || node.Action == null && node.Children.Length == 0;
    }
}
