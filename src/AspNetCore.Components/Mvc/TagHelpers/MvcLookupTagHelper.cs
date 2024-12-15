using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AspNetCore.Resources;
using NonFactors.Mvc.Lookup;

namespace AspNetCore.Components.Mvc;

[HtmlTargetElement("div", Attributes = "mvc-lookup-for,handler")]
public class MvcLookupTagHelper : LookupTagHelper
{
    public String? Handler { get; set; }

    public MvcLookupTagHelper(IHtmlGenerator html, IUrlHelperFactory factory)
        : base(html, factory)
    {
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        Url ??= $"~/Lookup/{Handler}";
        Title ??= Resource.ForLookup(Handler!);

        base.Process(context, output);
    }
}
