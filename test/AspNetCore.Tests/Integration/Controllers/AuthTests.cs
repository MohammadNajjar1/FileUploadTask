using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Components.Mail;
using AspNetCore.Components.Notifications;
using AspNetCore.Components.Security;
using AspNetCore.Controllers.Administration;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;
using AspNetCore.Services;
using AspNetCore.Validators;
using System.Security.Claims;

namespace AspNetCore.Controllers;

[Collection("Database access")]
public class AuthTests : IDisposable
{
    private Account model;
    private Auth controller;
    private IMailClient mail;
    private Accounts accounts;
    private DbContext context;

    public AuthTests()
    {
        context = TestingContext.Create();
        mail = Substitute.For<IMailClient>();
        IHasher hasher = Substitute.For<IHasher>();
        UnitOfWork unitOfWork = new(TestingContext.Create(), TestingContext.Mapper);
        accounts = new Accounts(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));
        controller = Substitute.ForPartsOf<Auth>(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher), mail);

        hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(info => $"{info.ArgAt<String>(0)}Hashed" == info.ArgAt<String>(1));
        hasher.HashPassword(Arg.Any<String>()).Returns(info => $"{info.Arg<String>()}Hashed");
        context.Drop().Add(model = ObjectsFactory.CreateAccount(-1));
        context.Add(ObjectsFactory.CreateAccount(-2));
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
    public void Recover_LoggedIn_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home));
        Object actual = controller.ImitateLogin(1).Recover();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Recover_View()
    {
        Assert.Null(Assert.IsType<ViewResult>(controller.Recover()).Model);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public async Task Recover_Post_LoggedIn_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home));
        Object actual = await controller.ImitateLogin(1).Recover(new AccountRecoveryView());

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Recover_InvalidModelstate_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        await controller.Returns(controller.Recover, new AccountRecoveryView());

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public async Task Recover_NotFound_Account()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email + "test" });

        Assert.All(context.Db<Account>(), account => Assert.True(account.RecoveryTokenExpiration < DateTime.Now.AddMinutes(-5)));
    }

    [Fact]
    public async Task Recover_Account()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        Account actual = context.Db<Account>().Single(account => account.Email == model.Email);

        Assert.True(actual.RecoveryTokenExpiration < DateTime.Now.AddMinutes(31));
        Assert.True(DateTime.Now.AddMinutes(29) < actual.RecoveryTokenExpiration);
        Assert.IsType<Guid>(Guid.Parse(actual.RecoveryToken!));
        Assert.NotEqual(model.Passhash, actual.RecoveryToken);
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.IsLocked, actual.IsLocked);
        Assert.Equal(model.Username, actual.Username);
        Assert.Equal(model.Passhash, actual.Passhash);
        Assert.Equal(model.RoleId, actual.RoleId);
        Assert.Equal(model.Email, actual.Email);
        Assert.Equal(model.Id, actual.Id);
    }

    [Fact]
    public async Task Recover_NotFound_Email()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email + "test" });

        await mail.DidNotReceive().SendAsync(Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>());
    }

    [Fact]
    public async Task Recover_Mail()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        String? url = controller.Url.Action(nameof(Auth.Reset), nameof(Auth), new { token = "TestToken" }, controller.Request.Scheme);
        String subject = Message.For<AccountView>("RecoveryEmailSubject");
        String body = Message.For<AccountView>("RecoveryEmailBody", url);

        await mail.Received().SendAsync(model.Email, subject, body);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public async Task Recover_Message(String prefix)
    {
        await controller.Recover(new AccountRecoveryView { Email = prefix + model.Email });

        controller.Alerts.IsSingle<AccountView>(AlertType.Info, "RecoveryInformation");
        Assert.Empty(controller.ModelState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public async Task Recover_Success_Redirect(String prefix)
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Login));
        Object actual = await controller.Recover(new AccountRecoveryView { Email = prefix + model.Email });

        controller.Alerts.IsSingle<AccountView>(AlertType.Info, "RecoveryInformation");
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Reset_LoggedIn_Redirect()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home));
        Object actual = controller.ImitateLogin(model.Id).Reset(token);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Reset_InvalidModelstate_Redirect()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();
        controller.ModelState.AddModelError("Test", "Error");

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset(token);

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Reset_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset("Test");

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "ExpiredToken");
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Reset_Expired_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset(model.RecoveryToken);

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "ExpiredToken");
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Reset_View()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Assert.Null(Assert.IsType<ViewResult>(controller.Reset(token)).Model);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public async Task Reset_Post_LoggedIn_Redirect()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home));
        Object actual = controller.ImitateLogin(model.Id).Reset(new AccountResetView { Token = token, NewPassword = "Test" });

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Reset_Post_InvalidModelstate_Redirect()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();
        controller.ModelState.AddModelError("Test", "Error");

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset(new AccountResetView { Token = token, NewPassword = "Test" });

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Reset_Post_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset(new AccountResetView { Token = "Test", NewPassword = "Test" });

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "ExpiredToken");
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Reset_Post_Expired_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Recover));
        Object actual = controller.Reset(new AccountResetView { Token = model.RecoveryToken, NewPassword = "Test" });

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "ExpiredToken");
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Reset_Account()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        controller.Reset(new AccountResetView { Token = token, NewPassword = "Test" });

        Account actual = context.Db<Account>().Single(account => account.Email == model.Email);

        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.Email.ToLower(), actual.Email);
        Assert.Equal(model.IsLocked, actual.IsLocked);
        Assert.Equal(model.Username, actual.Username);
        Assert.Equal("TestHashed", actual.Passhash);
        Assert.Null(actual.RecoveryTokenExpiration);
        Assert.Equal(model.RoleId, actual.RoleId);
        Assert.Equal(model.Id, actual.Id);
        Assert.Null(actual.RecoveryToken);
    }

    [Fact]
    public async Task Reset_Success_Redirect()
    {
        await controller.Recover(new AccountRecoveryView { Email = model.Email });

        controller.Alerts.Clear();

        String? token = context.Db<Account>().Single(account => account.Email == model.Email).RecoveryToken;

        Object expected = controller.StaticRedirectTo(nameof(Auth.Login));
        Object actual = controller.Reset(new AccountResetView { Token = token, NewPassword = "Test" });

        controller.Alerts.IsSingle<AccountView>(AlertType.Success, "SuccessfulReset", 4000);
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Login_LoggedIn_Redirect()
    {
        controller.Url.IsLocalUrl("/test").Returns(false);

        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home), new { area = "" });
        Object actual = controller.ImitateLogin(1).Login("/test");

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Login_LoggedIn_RedirectLocal()
    {
        controller.Url.IsLocalUrl("/test").Returns(true);

        Object expected = controller.StaticRedirect("/test");
        Object actual = controller.ImitateLogin(1).Login("/test");

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Login_View()
    {
        Assert.Null(Assert.IsType<ViewResult>(controller.Login("")).Model);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public async Task Login_Post_LoggedIn_Redirect()
    {
        controller.Url.IsLocalUrl("/test").Returns(false);

        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home), new { area = "" });
        Object actual = await controller.ImitateLogin(1).Login(ObjectsFactory.CreateAccountLoginView(1));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Login_Post_LoggedIn_RedirectLocal()
    {
        AccountLoginView account = ObjectsFactory.CreateAccountLoginView(1);
        controller.Url.IsLocalUrl(account.ReturnUrl).Returns(true);

        Object expected = controller.StaticRedirect(account.ReturnUrl!);
        Object actual = await controller.ImitateLogin(1).Login(account);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Login_InvalidModelstate_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        await controller.Returns(controller.Login, ObjectsFactory.CreateAccountLoginView(-1));

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public async Task Login_NotFound_Error()
    {
        AccountLoginView account = ObjectsFactory.CreateAccountLoginView(-1);
        account.Username += "Test";

        await controller.Returns(controller.Login, account);

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "IncorrectAuthentication");
        Assert.Empty(controller.ModelState);
    }

    [Fact]
    public async Task Login_IncorectPassword_Error()
    {
        AccountLoginView account = ObjectsFactory.CreateAccountLoginView(-1);
        account.Password += "Test";

        await controller.Returns(controller.Login, account);

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "IncorrectAuthentication");
        Assert.Empty(controller.ModelState);
    }

    [Fact]
    public async Task Login_Locked_Error()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = true;

        accounts.Edit(account);

        await controller.Returns(controller.Login, ObjectsFactory.CreateAccountLoginView(-1));

        controller.Alerts.IsSingle<AccountView>(AlertType.Danger, "LockedAccount");
        Assert.Empty(controller.ModelState);
    }

    [Fact]
    public async Task Login_Account()
    {
        IAuthenticationService auth = controller.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        await controller.Login(ObjectsFactory.CreateAccountLoginView(-1));

        await auth.Received().SignInAsync(controller.HttpContext, "Cookies", Arg.Is<ClaimsPrincipal>(principal =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier) == model.Id.ToString() &&
            principal.FindFirstValue(ClaimTypes.Name) == model.Username &&
            principal.FindFirstValue(ClaimTypes.Email) == model.Email &&
            principal.Identity!.AuthenticationType == "Password"), null);
    }

    [Fact]
    public async Task Login_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Home.Index), nameof(Home), new { area = "" });
        Object actual = await controller.Login(ObjectsFactory.CreateAccountLoginView(-1));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Login_Success_RedirectLocal()
    {
        AccountLoginView account = ObjectsFactory.CreateAccountLoginView(-1);
        controller.Url.IsLocalUrl(account.ReturnUrl).Returns(true);

        Object expected = controller.StaticRedirect(account.ReturnUrl!);
        Object actual = await controller.Login(account);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task Logout_Account()
    {
        IAuthenticationService auth = controller.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        await controller.Logout();

        await auth.Received().SignOutAsync(controller.HttpContext, "Cookies", null);
    }

    [Fact]
    public async Task Logout_Headers()
    {
        await controller.Logout();

        Assert.Equal(@"""cookies"", ""storage"", ""executionContexts""", controller.Response.Headers["Clear-Site-Data"]);
    }

    [Fact]
    public async Task Logout_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Login));
        Object actual = await controller.Logout();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }
}
