using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Notifications;
using AspNetCore.Components.Security;

namespace AspNetCore.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public abstract class AController : Controller
{
    public Alerts Alerts
    {
        get;
    }
    public IAuthorization Authorization
    {
        get
        {
            return HttpContext.RequestServices.GetRequiredService<IAuthorization>();
        }
    }

    protected AController()
    {
        Alerts = new Alerts();
    }

    public virtual ViewResult NotFoundView()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;

        return View($"~/Views/{nameof(Home)}/{nameof(Home.NotFound)}.cshtml");
    }
    public virtual ViewResult NotEmptyView(Object? model)
    {
        return model == null ? NotFoundView() : View(model);
    }
    public virtual ActionResult RedirectToLocal(String? url)
    {
        if (!Url.IsLocalUrl(url))
            return RedirectToAction(nameof(Home.Index), nameof(Home), new { area = "" });

        return Redirect(url);
    }

    public virtual Boolean IsAuthorizedFor(String permission)
    {
        return Authorization.IsGrantedFor(User.Id(), permission);
    }

    public override RedirectToActionResult RedirectToAction(String? actionName, String? controllerName, Object? routeValues)
    {
        IDictionary<String, Object> values = HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues);
        controllerName ??= values.ContainsKey("controller") ? values["controller"] as String : null;
        actionName ??= values.ContainsKey("action") ? values["action"] as String : null;
        Object? area = values.ContainsKey("area") ? values["area"] : null;
        controllerName ??= RouteData.Values["controller"] as String;
        actionName ??= RouteData.Values["action"] as String;
        area ??= RouteData.Values["area"];

        if (!IsAuthorizedFor($"{area}/{controllerName}/{actionName}".Trim('/')))
            return base.RedirectToAction(nameof(Home.Index), nameof(Home), new { area = "" });

        return base.RedirectToAction(actionName, controllerName, values);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is JsonResult)
            return;

        Alerts alerts = Alerts;

        if (TempData["Alerts"] is String alertsJson)
        {
            alerts = JsonSerializer.Deserialize<Alerts>(alertsJson)!;
            alerts.Merge(Alerts);
        }

        if (alerts.Count > 0)
            TempData["Alerts"] = JsonSerializer.Serialize(alerts);
    }
}
