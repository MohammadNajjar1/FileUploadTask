using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Components.Mvc;

public class ErrorResponseMiddleware
{
    private ILanguages Languages { get; }
    private RequestDelegate Next { get; }
    private ILogger<ErrorResponseMiddleware> Logger { get; }

    public ErrorResponseMiddleware(RequestDelegate next, ILanguages languages, ILogger<ErrorResponseMiddleware> logger)
    {
        Next = next;
        Logger = logger;
        Languages = languages;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await Next(context);

            if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status404NotFound)
                await View(context, "/home/not-found");
        }
        catch (Exception exception)
        {
            try
            {
                Logger.LogError(exception, "An unhandled exception has occurred while executing the request.");
            }
            finally
            {
                await View(context, "/home/error");
            }
        }
    }

    private async Task View(HttpContext context, String path)
    {
        Match abbreviation = Regex.Match(context.Request.Path, "^/(?<abbreviation>\\w{2})(/|$)");

        if (abbreviation.Success)
            context.Request.Path = $"/{Languages[abbreviation.Groups["abbreviation"].Value].Abbreviation}{path}";
        else
            context.Request.Path = path;

        context.Request.Method = HttpMethods.Get;
        context.Request.RouteValues.Clear();
        context.SetEndpoint(null);

        using IServiceScope scope = context.RequestServices.CreateScope();
        context.RequestServices = scope.ServiceProvider;

        await Next(context);
    }
}
