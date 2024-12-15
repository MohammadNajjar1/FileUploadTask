using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Components.Mvc;

public class ErrorResponseMiddlewareTests
{
    [Theory]
    [InlineData("/sales/order", "/home/not-found")]
    [InlineData("/en/sales/order", "/en-GB/home/not-found")]
    public async Task Invoke_NotFound(String path, String requestPath)
    {
        ILanguages languages = new Languages("en-GB", new[] { new Language { Abbreviation = "en-GB" } });
        ILogger<ErrorResponseMiddleware> logger = Substitute.For<ILogger<ErrorResponseMiddleware>>();
        HttpContext actual = HttpFactory.CreateHttpContext();
        actual.Request.RouteValues.Add("test", "test");
        actual.Request.Path = path;
        Boolean asserted = false;

        await new ErrorResponseMiddleware(next =>
        {
            if (next.Response.StatusCode != StatusCodes.Status404NotFound)
                next.Response.StatusCode = StatusCodes.Status404NotFound;
            else
                asserted = true;

            return Task.CompletedTask;
        }, languages, logger).Invoke(actual);

        Assert.True(asserted);
        Assert.Empty(actual.Request.RouteValues);
        Assert.Equal(requestPath, actual.Request.Path);
        Assert.Equal(HttpMethods.Get, actual.Request.Method);
    }

    [Theory]
    [InlineData("/sales/order", "/home/error")]
    [InlineData("/en/sales/order", "/en-GB/home/error")]
    public async Task Invoke_Error(String path, String requestPath)
    {
        ILanguages languages = new Languages("en-GB", new[] { new Language { Abbreviation = "en-GB" } });
        ILogger<ErrorResponseMiddleware> logger = Substitute.For<ILogger<ErrorResponseMiddleware>>();
        HttpContext actual = HttpFactory.CreateHttpContext();
        actual.Request.RouteValues.Add("test", "test");
        actual.Request.Path = path;
        Boolean asserted = false;

        await new ErrorResponseMiddleware(next =>
        {
            Assert.Equal(requestPath, next.Request.Path);

            asserted = true;

            return Task.CompletedTask;
        }, languages, logger).Invoke(actual);

        Assert.True(asserted);
        Assert.Empty(actual.Request.RouteValues);
        Assert.Equal(requestPath, actual.Request.Path);
        Assert.Equal(HttpMethods.Get, actual.Request.Method);
    }
}
