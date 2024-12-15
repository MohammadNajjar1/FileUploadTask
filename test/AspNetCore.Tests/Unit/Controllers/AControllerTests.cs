using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Notifications;

namespace AspNetCore.Controllers;

public class AControllerTests : IDisposable
{
    private AController controller;

    public AControllerTests()
    {
        controller = Substitute.ForPartsOf<AController>();
        controller.TempData = Substitute.For<ITempDataDictionary>();

        controller.Initialize();
    }
    public void Dispose()
    {
        controller.Dispose();
    }

    [Fact]
    public void AController_EmptyAlerts()
    {
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void NotFoundView_Response()
    {
        ViewResult actual = controller.NotFoundView();

        Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
        Assert.Equal($"~/Views/{nameof(Home)}/{nameof(Home.NotFound)}.cshtml", actual.ViewName);
    }

    [Fact]
    public void NotEmptyView_Null_NotFound()
    {
        Assert.Equal(controller.StaticNotFoundView(), controller.NotEmptyView(null));
    }

    [Fact]
    public void NotEmptyView_Model()
    {
        controller.Returns(controller.NotEmptyView, new Object());
    }

    [Fact]
    public void RedirectToLocal_External_Redirect()
    {
        controller.Url.IsLocalUrl("test").Returns(false);

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToLocal("test"));

        Assert.Equal(nameof(Home.Index), actual.ActionName);
        Assert.Equal(nameof(Home), actual.ControllerName);
        Assert.Equal("", actual.RouteValues!["area"]);
        Assert.Single(actual.RouteValues);
    }

    [Fact]
    public void RedirectToLocal_Url_Redirect()
    {
        controller.Url.IsLocalUrl("/test/action").Returns(true);

        Assert.Equal("/test/action", Assert.IsType<RedirectResult>(controller.RedirectToLocal("/test/action")).Url);
    }

    [Fact]
    public void IsAuthorizedFor_CurrentUser()
    {
        controller.Authorization.IsGrantedFor(controller.User.Id(), "Area/Controller/Action").Returns(true);

        Assert.True(controller.IsAuthorizedFor("Area/Controller/Action"));
    }

    [Fact]
    public void RedirectToAction_Unauthorized_RouteData()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor(Arg.Any<String>()).Returns(true);
        controller.IsAuthorizedFor("administration/roles/index").Returns(false);

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction(null, null, new { }));

        Assert.Equal(nameof(Home.Index), actual.ActionName);
        Assert.Equal(nameof(Home), actual.ControllerName);
        Assert.Equal("", actual.RouteValues!["area"]);
        Assert.Single(actual.RouteValues);
    }

    [Fact]
    public void RedirectToAction_Authorized_RouteData()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor("administration/roles/index").Returns(true);

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction(null, null, new { }));

        Assert.Equal("roles", actual.ControllerName);
        Assert.Equal("index", actual.ActionName);
        Assert.Null(actual.RouteValues!["area"]);
        Assert.Empty(actual.RouteValues);
    }

    [Fact]
    public void RedirectToAction_Unauthorized_RouteValues()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor(Arg.Any<String>()).Returns(true);
        controller.IsAuthorizedFor("reports/daily/create").Returns(false);

        Object values = new { area = "reports", controller = "daily", action = "create" };

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction(null, null, values));

        Assert.Equal(nameof(Home.Index), actual.ActionName);
        Assert.Equal(nameof(Home), actual.ControllerName);
        Assert.Equal("", actual.RouteValues!["area"]);
        Assert.Single(actual.RouteValues);
    }

    [Fact]
    public void RedirectToAction_Authorized_RouteValues()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor("reports/daily/create").Returns(true);

        Object values = new { area = "reports", controller = "daily", action = "create" };

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction(null, null, values));

        Assert.Equal("reports", actual.RouteValues!["area"]);
        Assert.Equal("daily", actual.ControllerName);
        Assert.Equal("create", actual.ActionName);
        Assert.Equal(3, actual.RouteValues.Count);
    }

    [Fact]
    public void RedirectToAction_Unauthorized_Route()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor(Arg.Any<String>()).Returns(true);
        controller.IsAuthorizedFor("administration/daily/create").Returns(false);

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction("create", "daily", new { }));

        Assert.Equal(nameof(Home.Index), actual.ActionName);
        Assert.Equal(nameof(Home), actual.ControllerName);
        Assert.Equal("", actual.RouteValues!["area"]);
        Assert.Single(actual.RouteValues);
    }

    [Fact]
    public void RedirectToAction_Authorized_Route()
    {
        controller.RouteData.Values["action"] = "index";
        controller.RouteData.Values["controller"] = "roles";
        controller.RouteData.Values["area"] = "administration";
        controller.IsAuthorizedFor("administration/daily/create").Returns(true);

        RedirectToActionResult actual = Assert.IsType<RedirectToActionResult>(controller.RedirectToAction("create", "daily", new { }));

        Assert.Equal("daily", actual.ControllerName);
        Assert.Equal("create", actual.ActionName);
        Assert.Null(actual.RouteValues!["area"]);
        Assert.Empty(actual.RouteValues);
    }

    [Fact]
    public void OnActionExecuted_Json_NoTempData()
    {
        JsonResult result = new("Value");
        controller.Alerts.AddError("Test");

        controller.OnActionExecuted(new ActionExecutedContext(controller.ControllerContext, new List<IFilterMetadata>(), controller) { Result = result });

        Assert.Empty(controller.TempData);
    }

    [Fact]
    public void OnActionExecuted_TempDataAlerts()
    {
        controller.Alerts.AddError("Test");
        controller.TempData["Alerts"] = null;

        controller.OnActionExecuted(new ActionExecutedContext(controller.ControllerContext, new List<IFilterMetadata>(), controller));

        Assert.Equal(JsonSerializer.Serialize(controller.Alerts), controller.TempData["Alerts"]);
    }

    [Fact]
    public void OnActionExecuted_Alerts_Merge()
    {
        Alerts alerts = new();
        alerts.AddError("Test1");

        controller.TempData["Alerts"] = JsonSerializer.Serialize(alerts);

        controller.Alerts.AddError("Test2");
        alerts.AddError("Test2");

        controller.OnActionExecuted(new ActionExecutedContext(controller.ControllerContext, new List<IFilterMetadata>(), controller));

        Assert.Equal(JsonSerializer.Serialize(alerts), controller.TempData["Alerts"]);
    }

    [Fact]
    public void OnActionExecuted_NoAlerts()
    {
        controller.Alerts.Clear();

        controller.OnActionExecuted(new ActionExecutedContext(controller.ControllerContext, new List<IFilterMetadata>(), controller));

        Assert.Null(controller.TempData["Alerts"]);
    }
}
