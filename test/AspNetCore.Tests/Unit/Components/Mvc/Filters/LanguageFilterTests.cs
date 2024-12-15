using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Components.Mvc;

public class LanguageFilterTests
{
    [Fact]
    public void OnResourceExecuting_SetsCurrentLanguage()
    {
        ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        ResourceExecutingContext context = new(action, Array.Empty<IFilterMetadata>(), Array.Empty<IValueProviderFactory>());
        Languages languages = new("en", new[] { new Language { Abbreviation = "en", Culture = new CultureInfo("en-gb") }, new Language { Abbreviation = "us", Culture = new CultureInfo("en-us") } });

        context.RouteData.Values["language"] = "us";
        new LanguageFilter(languages).OnResourceExecuting(context);

        Assert.Equal(languages["us"], languages.Current);
    }

    [Fact]
    public void OnResourceExecuting_SetsDefaultLanguage()
    {
        ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        Languages languages = new("en", new[] { new Language { Abbreviation = "en", Culture = new CultureInfo("en-gb") } });
        ResourceExecutingContext context = new(action, Array.Empty<IFilterMetadata>(), Array.Empty<IValueProviderFactory>());

        new LanguageFilter(languages).OnResourceExecuting(context);

        Assert.Equal(languages.Default, languages.Current);
    }

    [Fact]
    public void OnResourceExecuted_DoesNothing()
    {
        ActionContext action = new(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        Languages languages = new("en", new[] { new Language { Abbreviation = "en", Culture = new CultureInfo("en-gb") } });

        new LanguageFilter(languages).OnResourceExecuted(new ResourceExecutedContext(action, Array.Empty<IFilterMetadata>()));
    }
}
