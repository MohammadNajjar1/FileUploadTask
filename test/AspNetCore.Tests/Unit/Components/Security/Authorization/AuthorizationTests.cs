using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Security.Area;
using AspNetCore.Data;
using AspNetCore.Objects;

namespace AspNetCore.Components.Security;

[Collection("Database access")]
public class AuthorizationTests : IDisposable
{
    private DbContext context;
    private IServiceProvider services;
    private Authorization authorization;

    public AuthorizationTests()
    {
        context = TestingContext.Create();
        services = Substitute.For<IServiceProvider>();

        services.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());
        services.GetService(typeof(IUnitOfWork)).Returns(_ => new UnitOfWork(TestingContext.Create(), TestingContext.Mapper));

        authorization = new Authorization(Assembly.GetExecutingAssembly(), services);
    }
    public void Dispose()
    {
        context.Dispose();
    }

    [Fact]
    public void IsGrantedFor_AuthorizesControllerByIgnoringCase()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}".ToUpper()));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeControllerByIgnoringCase()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}".ToUpper()));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesControllerWithoutArea()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), nameof(AuthorizeController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeControllerWithoutArea()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesControllerWithArea()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeControllerWithArea()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.AuthorizedGetAction));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedGetAction)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedGetAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesNamedGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), "AuthorizedNamedGetAction");

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/AuthorizedNamedGetAction"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNamedGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/AuthorizedNamedGetAction"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesNotExistingAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/Test"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNotExistingAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), "Other");

        Assert.False(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/Test"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesNonGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.AuthorizedPostAction));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedPostAction)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNonGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedPostAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesNamedNonGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), "AuthorizedNamedPostAction");

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/AuthorizedNamedPostAction"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNamedNonGetAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/AuthorizedNamedPostAction"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesActionAsAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsAction)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeActionAsAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.AuthorizedAsAction));

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesActionAsSelf()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.AuthorizedAsSelf));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsSelf)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeActionAsSelf()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsSelf)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesActionAsOtherAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(InheritedAuthorizedController), nameof(InheritedAuthorizedController.InheritanceAction));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsOtherAction)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeActionAsOtherAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.AuthorizedAsOtherAction));

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.AuthorizedAsOtherAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesEmptyAreaAsNull()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), nameof(AuthorizeController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeEmptyAreaAsNull()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAuthorizedAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "AllowAnonymous", "AuthorizedAction");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AllowAnonymousController)}/{nameof(AllowAnonymousController.AuthorizedAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAllowAnonymousAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.AllowAnonymousAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAllowUnauthorizedAction()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.AllowUnauthorizedAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAuthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), nameof(AuthorizeController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeAuthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAllowAnonymousController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AllowAnonymousController)}/{nameof(AllowAnonymousController.SimpleAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesAllowUnauthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AllowUnauthorizedController)}/{nameof(AllowUnauthorizedController.AuthorizedAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesInheritedAuthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(InheritedAuthorizedController), nameof(InheritedAuthorizedController.InheritanceAction));

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(InheritedAuthorizedController)}/{nameof(InheritedAuthorizedController.InheritanceAction)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeInheritedAuthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"{nameof(InheritedAuthorizedController)}/{nameof(InheritedAuthorizedController.InheritanceAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesInheritedAllowAnonymousController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(InheritedAllowAnonymousController)}/{nameof(InheritedAllowAnonymousController.InheritanceAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesInheritedAllowUnauthorizedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(InheritedAllowUnauthorizedController)}/{nameof(InheritedAllowUnauthorizedController.InheritanceAction)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesNotAttributedController()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", "Test", "Test");

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(NotAttributedController)}/{nameof(NotAttributedController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNotExistingAccount()
    {
        CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));

        Assert.False(authorization.IsGrantedFor(0, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeLockedAccount()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action), isLocked: true);

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeNotFoundAccount()
    {
        CreateAccountWithPermissionFor("", nameof(AuthorizeController), nameof(AuthorizeController.Action));

        Assert.False(authorization.IsGrantedFor(0, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void IsGrantedFor_AuthorizesByIgnoringCase()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));

        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}".ToUpper()));
    }

    [Fact]
    public void IsGrantedFor_DoesNotAuthorizeByIgnoringCase()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}".ToUpper()));
    }

    [Fact]
    public void IsGrantedFor_CachesAccountPermissions()
    {
        Int64 accountId = CreateAccountWithPermissionFor("", nameof(AuthorizeController), nameof(AuthorizeController.Action));

        context.Drop();

        Assert.True(authorization.IsGrantedFor(accountId, $"{nameof(AuthorizeController)}/{nameof(AuthorizeController.Action)}"));
    }

    [Fact]
    public void Refresh_Permissions()
    {
        Int64 accountId = CreateAccountWithPermissionFor("Area", nameof(AuthorizedController), nameof(AuthorizedController.Action));
        Assert.True(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));

        context.Drop();

        authorization.Refresh(services);

        Assert.False(authorization.IsGrantedFor(accountId, $"Area/{nameof(AuthorizedController)}/{nameof(AuthorizedController.Action)}"));
    }

    private Int64 CreateAccountWithPermissionFor(String area, String controller, String action, Boolean isLocked = false)
    {
        RolePermission rolePermission = ObjectsFactory.CreateRolePermission(0);
        rolePermission.Permission.Controller = controller;
        rolePermission.Permission.Action = action;
        rolePermission.Permission.Area = area;

        Account account = ObjectsFactory.CreateAccount(0);
        account.Role = rolePermission.Role;
        account.IsLocked = isLocked;

        context.Drop().Add(rolePermission);
        context.Add(account);

        context.SaveChanges();

        authorization.Refresh(services);

        return account.Id;
    }
}
