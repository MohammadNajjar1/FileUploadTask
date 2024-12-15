using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Security;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Services;
using AspNetCore.Validators;

namespace AspNetCore.Controllers.Administration;

[Collection("Database access")]
public class AccountsTests : IDisposable
{
    private Account model;
    private DbContext context;
    private Account otherModel;
    private Accounts controller;

    public AccountsTests()
    {
        context = TestingContext.Create();
        IHasher hasher = Substitute.For<IHasher>();
        UnitOfWork unitOfWork = new(TestingContext.Create(), TestingContext.Mapper);
        controller = Substitute.ForPartsOf<Accounts>(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));

        hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(info => $"{info.ArgAt<String>(0)}Hashed" == info.ArgAt<String>(1));
        hasher.HashPassword(Arg.Any<String>()).Returns(info => $"{info.Arg<String>()}Hashed");
        context.Drop().Add(model = ObjectsFactory.CreateAccount(-1));
        context.Add(otherModel = ObjectsFactory.CreateAccount(-2));
        context.SaveChanges();

        controller.Initialize();
    }
    public void Dispose()
    {
        controller.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Index_Accounts()
    {
        Account[] expected = context.Set<Account>().OrderByDescending(account => account.Id).ToArray();
        AccountView[] actual = controller.Index().Returns<IQueryable<AccountView>>().ToArray();

        for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
        {
            Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
            Assert.Equal(expected[i].Role?.Title, actual[i].RoleTitle);
            Assert.Equal(expected[i].IsLocked, actual[i].IsLocked);
            Assert.Equal(expected[i].Username, actual[i].Username);
            Assert.Equal(expected[i].Email, actual[i].Email);
            Assert.Equal(expected[i].Id, actual[i].Id);
        }

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_View()
    {
        Assert.Null(controller.Create().Model);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_InvalidModelState_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        controller.Returns(controller.Create, ObjectsFactory.CreateAccountCreateView(model.Id));

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_UsedUsername_Error()
    {
        AccountCreateView account = ObjectsFactory.CreateAccountCreateView(model.Id);
        account.Username = model.Username.ToUpper();

        controller.Returns(controller.Create, account);

        controller.ModelState.IsSingle<AccountView>(nameof(AccountCreateView.Username), "UniqueUsername");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_UsedEmail_Error()
    {
        AccountCreateView account = ObjectsFactory.CreateAccountCreateView(model.Id);
        account.Email = model.Email.ToUpper();

        controller.Returns(controller.Create, account);

        controller.ModelState.IsSingle<AccountView>(nameof(AccountCreateView.Email), "UniqueEmail");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_Account()
    {
        AccountCreateView account = ObjectsFactory.CreateAccountCreateView(model.Id);
        account.RoleId = model.RoleId;

        controller.Create(account);

        Account actual = context.Db<Account>().Single(account => account.Id > otherModel.Id);

        Assert.Equal($"{account.Password}Hashed", actual.Passhash);
        Assert.Equal(account.CreationDate, actual.CreationDate);
        Assert.Equal(account.Email.ToLower(), actual.Email);
        Assert.Equal(account.Username, actual.Username);
        Assert.Equal(account.RoleId, actual.RoleId);
        Assert.Null(actual.RecoveryTokenExpiration);
        Assert.Null(actual.RecoveryToken);
        Assert.False(actual.IsLocked);
    }

    [Fact]
    public void Create_Authorization_Refresh()
    {
        controller.Create(ObjectsFactory.CreateAccountCreateView(model.Id));

        controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
    }

    [Fact]
    public void Create_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Accounts.Index));
        Object actual = controller.Create(ObjectsFactory.CreateAccountCreateView(model.Id));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Details_NotFound()
    {
        Object expected = controller.StaticNotFoundView();
        Object actual = controller.Details(0);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Details_View()
    {
        Account expected = model;
        AccountView actual = controller.Details(model.Id).Returns<AccountView>();

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Role?.Title, actual.RoleTitle);
        Assert.Equal(expected.IsLocked, actual.IsLocked);
        Assert.Equal(expected.Username, actual.Username);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_NotFound()
    {
        Object expected = controller.StaticNotFoundView();
        Object actual = controller.Edit(0);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Edit_View()
    {
        Account expected = model;
        AccountEditView actual = controller.Edit(model.Id).Returns<AccountEditView>();

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.IsLocked, actual.IsLocked);
        Assert.Equal(expected.Username, actual.Username);
        Assert.Equal(expected.RoleId, actual.RoleId);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_InvalidModelState_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        controller.Returns(controller.Edit, ObjectsFactory.CreateAccountEditView(model.Id));

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_UsedUsername_Error()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.Username = otherModel.Username.ToUpper();

        controller.Returns(controller.Edit, account);

        controller.ModelState.IsSingle<AccountView>(nameof(AccountEditView.Username), "UniqueUsername");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_UsedEmail_Error()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.Email = otherModel.Email.ToUpper();

        controller.Returns(controller.Edit, account);

        controller.ModelState.IsSingle<AccountView>(nameof(AccountEditView.Email), "UniqueEmail");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_Account()
    {
        AccountEditView account = ObjectsFactory.CreateAccountEditView(model.Id);
        account.IsLocked = model.IsLocked = !model.IsLocked;
        account.Username = $"{model.Username}Test";
        account.RoleId = model.RoleId = null;
        account.Email = $"{model.Email}Test";

        controller.Edit(account);

        Account actual = context.Db<Account>().Single(account => account.Id == model.Id);

        Assert.Equal(model.RecoveryTokenExpiration, actual.RecoveryTokenExpiration);
        Assert.Equal(model.RecoveryToken, actual.RecoveryToken);
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.IsLocked, actual.IsLocked);
        Assert.Equal(model.Passhash, actual.Passhash);
        Assert.Equal(model.Username, actual.Username);
        Assert.Equal(model.RoleId, actual.RoleId);
        Assert.Equal(model.Email, actual.Email);
        Assert.Equal(model.Id, actual.Id);
    }

    [Fact]
    public void Edit_Authorization_Refresh()
    {
        controller.Edit(ObjectsFactory.CreateAccountEditView(model.Id));

        controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
    }

    [Fact]
    public void Edit_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Accounts.Index));
        Object actual = controller.Edit(ObjectsFactory.CreateAccountEditView(model.Id));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }
}
