using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using AspNetCore.Controllers;

namespace AspNetCore;

public static class ControllerExtensions
{
    public static void Initialize(this Controller controller)
    {
        List<IFilterMetadata> filters = new();
        Dictionary<String, Object?> arguments = new();
        ViewContext context = HttpFactory.CreateViewContext();
        controller.ControllerContext.RouteData = context.RouteData;
        controller.ControllerContext.HttpContext = context.HttpContext;
        controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();

        controller.OnActionExecuting(new ActionExecutingContext(context, filters, arguments, controller));
    }

    public static T ImitateLogin<T>(this T controller, Int64 id) where T : Controller
    {
        Claim[] claims = { new(ClaimTypes.NameIdentifier, id.ToString(CultureInfo.CurrentCulture)) };
        ActionContext action = new(controller.HttpContext, controller.RouteData, new ActionDescriptor());
        ActionExecutingContext context = new(action, new List<IFilterMetadata>(), new Dictionary<String, Object?>(), controller);

        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Password"));

        controller.OnActionExecuting(context);

        return controller;
    }

    public static ViewResult StaticNotFoundView(this AController controller)
    {
        controller.When(sub => sub.NotFoundView()).DoNotCallBase();
        controller.NotFoundView().Returns(new ViewResult());

        return controller.NotFoundView();
    }
    public static RedirectResult StaticRedirect(this AController aController, String url)
    {
        RedirectResult result = new(url);
        aController.When(sub => sub.Redirect(url)).DoNotCallBase();
        aController.Redirect(url).Returns(result);

        return result;
    }
    public static RedirectToActionResult StaticRedirectTo(this AController aController, String action, String? controller = null, Object? routeValues = null)
    {
        RedirectToActionResult result = new(action, controller, routeValues);
        IDictionary<String, Object> values = HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues);
        aController.When(sub => sub.RedirectToAction(action, controller, Arg.Any<Object>())).DoNotCallBase();
        aController.RedirectToAction(action, controller, Arg.Any<Object?>()).Returns(sub =>
        {
            IDictionary<String, Object> args = HtmlHelper.AnonymousObjectToHtmlAttributes(sub.ArgAt<Object?>(2));

            if (values.Count != args.Count)
                return new RedirectToActionResult(action, controller, sub.ArgAt<Object?>(2));

            foreach ((String key, Object value) in values)
                if (!args.ContainsKey(key) || value != args[key])
                    return new RedirectToActionResult(action, controller, sub.ArgAt<Object?>(2));

            return result;
        });

        return result;
    }
}
