using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.Components.Mvc;

public class LanguageFilter : IResourceFilter
{
    private ILanguages Languages { get; }

    public LanguageFilter(ILanguages languages)
    {
        Languages = languages;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (context.RouteData.Values["language"] is String abbrevation)
            Languages.Current = Languages[abbrevation];
        else
            Languages.Current = Languages.Default;
    }
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
