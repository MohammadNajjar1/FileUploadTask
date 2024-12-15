using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Components.Mvc;

[HtmlTargetElement("input", Attributes = "asp-for")]
public class FormInputTagHelper : TagHelper
{
    [HtmlAttributeName("type")]
    public String? Type { get; set; }

    [HtmlAttributeName("asp-for")]
    public ModelExpression? For { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        String classes = For?.Metadata.ModelType == typeof(Boolean) ? "form-check-input" : "form-control";
        classes = $"{classes} {output.Attributes["class"]?.Value}".Trim();

        if (Type == null && For?.Metadata.ModelType != typeof(Boolean))
            output.Attributes.SetAttribute("type", "text");

        if (output.Attributes["autocomplete"] == null)
            output.Attributes.Add("autocomplete", "off");

        output.Attributes.SetAttribute("class", classes);

        if (classes.Contains("date-picker") || classes.Contains("date-time-picker"))
            WrapAsDatepicker(output);
    }

    private void WrapAsDatepicker(TagHelperOutput output)
    {
        TagBuilder group = new("div");
        TagBuilder browser = new("button");

        browser.AddCssClass("date-picker-browser input-group-text fas fa-calendar-alt");
        browser.Attributes["type"] = "button";
        group.AddCssClass("input-group");

        output.PreElement.AppendHtml(group.RenderStartTag());
        output.PostElement.AppendHtml(browser);
        output.PostElement.AppendHtml(group.RenderEndTag());
    }
}
