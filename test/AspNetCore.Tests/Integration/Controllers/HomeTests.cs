using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Notifications;
using AspNetCore.Components.Security;
using AspNetCore.Controllers.Administration;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;
using AspNetCore.Services;
using AspNetCore.Validators;

namespace AspNetCore.Controllers;

[Collection("Database access")]
public class HomeTests : IDisposable
{
    private Account model;
    private Home controller;
    private Accounts accounts;
    private DbContext context;

    public HomeTests()
    {
        context = TestingContext.Create();
        IHasher hasher = Substitute.For<IHasher>();
        UnitOfWork unitOfWork = new(TestingContext.Create(), TestingContext.Mapper);
        controller = Substitute.ForPartsOf<Home>(new AccountService(unitOfWork, hasher));
        accounts = new Accounts(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));

        context.Drop().Add(model = ObjectsFactory.CreateAccount(-1));
        context.SaveChanges();

        controller.Initialize();
        accounts.Initialize();
    }
    public void Dispose()
    {
        controller.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Index_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(0).Index();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Index_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(account.Id).Index();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Index_View()
    {
        Assert.Null(Assert.IsType<ViewResult>(controller.ImitateLogin(model.Id).Index()).Model);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Error_Json()
    {
        controller.HttpContext.TraceIdentifier = "TestTrace";
        controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        Object? actual = Assert.IsType<JsonResult>(controller.Error()).Value;

        controller.Alerts.IsSingle(AlertType.Danger, Resource.ForString("SystemError", "TestTrace"), 0, "SystemError");
        Assert.Same(controller.Alerts, actual?.GetType().GetProperty("alerts")?.GetValue(actual));
        Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
        Assert.Empty(controller.ModelState);
    }

    [Fact]
    public void Error_View()
    {
        Object? actual = Assert.IsType<ViewResult>(controller.Error()).Model;

        Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Null(actual);
    }

    [Fact]
    public void NotFound_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(account.Id).NotFound();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void NotFound_View()
    {
        Object? actual = Assert.IsType<ViewResult>(controller.NotFound()).Model;

        Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Null(actual);
    }
}
