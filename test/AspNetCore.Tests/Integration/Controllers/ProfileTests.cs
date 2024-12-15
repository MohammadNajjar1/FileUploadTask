using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Extensions;
using AspNetCore.Components.Notifications;
using AspNetCore.Components.Security;
using AspNetCore.Controllers.Administration;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Services;
using AspNetCore.Validators;
using System.Security.Claims;

namespace AspNetCore.Controllers;

[Collection("Database access")]
public class ProfileTests : IDisposable
{
    private Account model;
    private Accounts accounts;
    private DbContext context;
    private Profile controller;
    private Account otherModel;

    public ProfileTests()
    {
        context = TestingContext.Create();
        IHasher hasher = Substitute.For<IHasher>();
        UnitOfWork unitOfWork = new(TestingContext.Create(), TestingContext.Mapper);
        accounts = new Accounts(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));
        controller = Substitute.ForPartsOf<Profile>(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));

        hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(info => $"{info.ArgAt<String>(0)}Hashed" == info.ArgAt<String>(1));
        hasher.HashPassword(Arg.Any<String>()).Returns(info => $"{info.Arg<String>()}Hashed");
        context.Drop().Add(model = ObjectsFactory.CreateAccount(-1));
        context.Add(otherModel = ObjectsFactory.CreateAccount(-2));
        context.SaveChanges();

        accounts.Initialize();
        controller.Initialize();
        controller.ImitateLogin(model.Id);
    }
    public void Dispose()
    {
        controller.Dispose();
        accounts.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Edit_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(0).Edit();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Edit_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.Edit();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Edit_View()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = false;

        accounts.Edit(account);

        Account expected = model;
        ProfileEditView actual = controller.Edit().Returns<ProfileEditView>();

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Username, actual.Username);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Null(actual.NewPassword);
        Assert.Null(actual.Password);
    }

    [Fact]
    public void Edit_Post_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(0).Edit(ObjectsFactory.CreateProfileEditView(-1));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Edit_Post_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.Edit(ObjectsFactory.CreateProfileEditView(-1));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Edit_InvalidModelState_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        controller.Returns(controller.Edit, ObjectsFactory.CreateProfileEditView(-1));

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_IncorrectPassword_Error()
    {
        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Password += "Test";

        controller.Returns(controller.Edit, profile);

        controller.ModelState.IsSingle<AccountView>(nameof(ProfileEditView.Password), "IncorrectPassword");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_UsedUsername_Error()
    {
        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Username = otherModel.Username.ToUpper();

        controller.Returns(controller.Edit, profile);

        controller.ModelState.IsSingle<AccountView>(nameof(ProfileEditView.Username), "UniqueUsername");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_UsedEmail_Error()
    {
        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Email = otherModel.Email.ToUpper();

        controller.Returns(controller.Edit, profile);

        controller.ModelState.IsSingle<AccountView>(nameof(ProfileEditView.Email), "UniqueEmail");
        Assert.Empty(controller.Alerts);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Edit_Profile(String? newPassword)
    {
        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Username = model.Username += "Test";
        profile.Email = model.Email += "Test";
        profile.NewPassword = newPassword;

        controller.Edit(profile);

        Account actual = context.Db<Account>().Single(account => account.Id == controller.User.Id());

        Assert.Equal(model.RecoveryTokenExpiration, actual.RecoveryTokenExpiration);
        Assert.Equal(model.RecoveryToken, actual.RecoveryToken);
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.Email.ToLower(), actual.Email);
        Assert.Equal(model.IsLocked, actual.IsLocked);
        Assert.Equal(model.Username, actual.Username);
        Assert.Equal(model.Passhash, actual.Passhash);
        Assert.Equal(model.RoleId, actual.RoleId);
        Assert.Equal(model.Id, actual.Id);
    }

    [Fact]
    public void Edit_NewPassword()
    {
        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Username = model.Username += "Test";
        profile.Email = model.Email += "Test";
        model.Passhash = "TestHashed";
        profile.NewPassword = "Test";

        controller.Edit(profile);

        Account actual = context.Db<Account>().Single(account => account.Id == controller.User.Id());

        Assert.Equal(model.RecoveryTokenExpiration, actual.RecoveryTokenExpiration);
        Assert.Equal(model.RecoveryToken, actual.RecoveryToken);
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.Email.ToLower(), actual.Email);
        Assert.Equal(model.IsLocked, actual.IsLocked);
        Assert.Equal(model.Username, actual.Username);
        Assert.Equal(model.Passhash, actual.Passhash);
        Assert.Equal(model.RoleId, actual.RoleId);
        Assert.Equal(model.Id, actual.Id);
    }

    [Fact]
    public void Edit_Claims()
    {
        controller.User.AddIdentity(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@email.com"),
            new Claim(ClaimTypes.Name, "TestName")
        }));

        ProfileEditView profile = ObjectsFactory.CreateProfileEditView(-1);
        profile.Username = model.Username += "Test";
        profile.Email = model.Email += "Test";

        controller.Edit(profile);

        Assert.Empty(controller.ModelState);
        Assert.Equal(profile.Username, controller.User.FindFirstValue(ClaimTypes.Name));
        controller.Alerts.IsSingle<AccountView>(AlertType.Success, "ProfileUpdated", 4000);
        Assert.Equal(profile.Email.ToLower(), controller.User.FindFirstValue(ClaimTypes.Email));
    }

    [Fact]
    public void Edit_Message()
    {
        controller.Edit(ObjectsFactory.CreateProfileEditView(-1));

        controller.Alerts.IsSingle<AccountView>(AlertType.Success, "ProfileUpdated", 4000);
        Assert.Empty(controller.ModelState);
    }

    [Fact]
    public void Edit_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Profile.Edit));
        Object actual = controller.Edit(ObjectsFactory.CreateProfileEditView(-1));

        controller.Alerts.IsSingle<AccountView>(AlertType.Success, "ProfileUpdated", 4000);
        Assert.Empty(controller.ModelState);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Delete_NotFound_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.ImitateLogin(0).Delete();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Delete_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.RoleId = model.RoleId;
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.Delete();

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Delete_Disclaimer()
    {
        controller.Delete();

        Assert.Empty(controller.ModelState);
        controller.Alerts.IsSingle<AccountView>(AlertType.Warning, "ProfileDeleteDisclaimer");
    }

    [Fact]
    public void Delete_View()
    {
        ViewResult actual = Assert.IsType<ViewResult>(controller.Delete());

        controller.Alerts.IsSingle<AccountView>(AlertType.Warning, "ProfileDeleteDisclaimer");
        Assert.Empty(controller.ModelState);
        Assert.Null(actual.Model);
    }

    [Fact]
    public void DeleteConfirmed_Locked_Redirect()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        ProfileDeleteView profile = ObjectsFactory.CreateProfileDeleteView(-1);
        account.IsLocked = true;

        accounts.Edit(account);

        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.DeleteConfirmed(profile);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void DeleteConfirmed_IncorrectPassword_Error()
    {
        controller.Returns(controller.DeleteConfirmed, ObjectsFactory.CreateProfileDeleteView(1));

        controller.ModelState.IsSingle<AccountView>(nameof(ProfileEditView.Password), "IncorrectPassword");
        controller.Alerts.IsSingle<AccountView>(AlertType.Warning, "ProfileDeleteDisclaimer");
        Assert.Equal(2, context.Db<Account>().Count());
    }

    [Fact]
    public void DeleteConfirmed_Profile()
    {
        controller.DeleteConfirmed(ObjectsFactory.CreateProfileDeleteView(-1));

        Assert.Single(context.Db<Account>(), account => account.Id != model.Id);
    }

    [Fact]
    public void DeleteConfirmed_Authorization_Refresh()
    {
        controller.DeleteConfirmed(ObjectsFactory.CreateProfileDeleteView(-1));

        controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
    }

    [Fact]
    public void DeleteConfirmed_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Auth.Logout), nameof(Auth));
        Object actual = controller.DeleteConfirmed(ObjectsFactory.CreateProfileDeleteView(-1));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }
}
