using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

[HtmlTargetElement("select", Attributes = "asp-for")]
public class FormSelectTagHelper : TagHelper
{
    [HtmlAttributeName("asp-for")]
    public ModelExpression? For { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output.Attributes["class"] == null)
            output.Attributes.Insert(0, new TagHelperAttribute("class", "form-select"));
        else
            output.Attributes.SetAttribute("class", $"form-select {output.Attributes["class"].Value}");

        if (!For!.Metadata.IsRequired)
            output.PreContent.AppendHtml(new TagBuilder("option"));
    }
}
