using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCore.Components.Security;

namespace AspNetCore;

public static class HttpFactory
{
    public static IHtmlHelper CreateHtmlHelper()
    {
        IHtmlHelper<Object> html = Substitute.For<IHtmlHelper<Object>>();
        ViewContext context = CreateViewContext();
        html.ViewContext.Returns(context);

        return html;
    }
    public static HttpContext CreateHttpContext()
    {
        HttpContext context = new DefaultHttpContext();

        context.Request.Path = "/en/home/index";
        context.RequestServices = Substitute.For<IServiceProvider>();
        context.RequestServices.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());
        context.RequestServices.GetService(typeof(ILoggerFactory)).Returns(Substitute.For<ILoggerFactory>());
        context.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());
        context.RequestServices.GetService(typeof(IServiceScopeFactory)).Returns(Substitute.For<IServiceScopeFactory>());
        context.RequestServices.GetService(typeof(IAuthenticationService)).Returns(Substitute.For<IAuthenticationService>());
        context.RequestServices.GetService(typeof(ITempDataDictionaryFactory)).Returns(Substitute.For<ITempDataDictionaryFactory>());

        return context;
    }
    public static ViewContext CreateViewContext()
    {
        ViewContext context = new();
        context.RouteData = new RouteData();
        context.HttpContext = CreateHttpContext();
        context.ActionDescriptor = new ActionDescriptor();
        IUrlHelperFactory factory = CreateUrlHelperFactory(context);

        context.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);

        return context;
    }
    public static IUrlHelperFactory CreateUrlHelperFactory(ActionContext context)
    {
        IUrlHelper url = Substitute.For<IUrlHelper>();
        IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();

        url.ActionContext.Returns(context);
        factory.GetUrlHelper(context).Returns(url);
        url.Content(Arg.Any<String>()).Returns(info => info.Arg<String>());

        return factory;
    }
}
