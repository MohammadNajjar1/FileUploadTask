using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCore.Components.Mvc;

public interface ISiteMap
{
    SiteMapNode[] For(ViewContext context);
    SiteMapNode[] BreadcrumbFor(ViewContext context);
}
