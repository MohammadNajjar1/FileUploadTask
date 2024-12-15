using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Security;
using AspNetCore.Components.Tree;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;
using AspNetCore.Services;
using AspNetCore.Validators;

namespace AspNetCore.Controllers.Administration;

[Collection("Database access")]
public class RolesTests : IDisposable
{
    private Role model;
    private Role otherModel;
    private Roles controller;
    private Accounts accounts;
    private DbContext context;

    public RolesTests()
    {
        context = TestingContext.Create();
        model = ObjectsFactory.CreateRole(-1);
        otherModel = ObjectsFactory.CreateRole(-2);
        IHasher hasher = Substitute.For<IHasher>();
        UnitOfWork unitOfWork = new(TestingContext.Create(), TestingContext.Mapper);
        controller = Substitute.ForPartsOf<Roles>(new RoleValidator(unitOfWork), new RoleService(unitOfWork));
        accounts = new Accounts(new AccountValidator(unitOfWork, hasher), new AccountService(unitOfWork, hasher));

        otherModel.Permissions.Clear();
        context.Drop().Add(model);
        context.Add(otherModel);
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
    public void Index_Roles()
    {
        Role[] expected = context.Set<Role>().OrderByDescending(role => role.Id).ToArray();
        RoleView[] actual = controller.Index().Returns<IQueryable<RoleView>>().ToArray();

        for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
        {
            Assert.Equal(expected[i].Permissions.Select(role => role.PermissionId), actual[i].Permissions.SelectedIds);
            Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
            Assert.Equal(expected[i].Title, actual[i].Title);
            Assert.Equal(expected[i].Id, actual[i].Id);
            Assert.Empty(actual[i].Permissions.Nodes);
        }

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_View()
    {
        RoleView actual = controller.Create().Returns<RoleView>();

        Assert.Empty(actual.Permissions.SelectedIds);
        AssertPermissionTree(actual.Permissions);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Equal(0, actual.Id);
        Assert.Null(actual.Title);
    }

    [Fact]
    public void Create_InvalidModelState_Error()
    {
        RoleView role = ObjectsFactory.CreateRoleView(model.Id);
        controller.ModelState.AddModelError("Test", "Error");

        controller.Returns(controller.Create, role);

        controller.ModelState.IsSingle("Test", "Error");
        AssertPermissionTree(role.Permissions);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_UsedTitle_Error()
    {
        RoleView role = ObjectsFactory.CreateRoleView(model.Id);
        role.Title = model.Title.ToUpper();

        controller.Returns(controller.Create, role);

        controller.ModelState.IsSingle<RoleView>(nameof(RoleView.Title), "UniqueTitle");
        AssertPermissionTree(role.Permissions);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Create_Role()
    {
        RoleView role = ObjectsFactory.CreateRoleView(model.Id);

        controller.Create(role);

        Role actual = context.Db<Role>().Single(role => role.Id > otherModel.Id);

        Assert.Equal(role.CreationDate, actual.CreationDate);
        Assert.Equal(role.Title, actual.Title);
    }

    [Fact]
    public void Create_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Roles.Index));
        Object actual = controller.Create(ObjectsFactory.CreateRoleView(model.Id));

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
        Role expected = model;
        RoleView actual = controller.Details(model.Id).Returns<RoleView>();

        Assert.Equal(expected.Permissions.Select(role => role.PermissionId), actual.Permissions.SelectedIds);
        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Title, actual.Title);
        AssertPermissionTree(actual.Permissions);
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
        Role expected = model;
        RoleView actual = controller.Edit(model.Id).Returns<RoleView>();

        Assert.Equal(expected.Permissions.Select(role => role.PermissionId), actual.Permissions.SelectedIds);
        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Title, actual.Title);
        AssertPermissionTree(actual.Permissions);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_InvalidModelState_Error()
    {
        controller.ModelState.AddModelError("Test", "Error");

        controller.Returns(controller.Edit, ObjectsFactory.CreateRoleView(model.Id));

        controller.ModelState.IsSingle("Test", "Error");
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_UsedTitle_Error()
    {
        RoleView role = ObjectsFactory.CreateRoleView(model.Id);
        role.Title = otherModel.Title.ToUpper();

        controller.Returns(controller.Edit, role);

        controller.ModelState.IsSingle<RoleView>(nameof(RoleView.Title), "UniqueTitle");
        AssertPermissionTree(role.Permissions);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Edit_Role()
    {
        RoleView role = ObjectsFactory.CreateRoleView(model.Id);
        role.Permissions.SelectedIds.Add(model.Permissions[0].PermissionId);
        role.Permissions.SelectedIds.Add(model.Permissions[1].PermissionId);
        role.Permissions.SelectedIds.Add(model.Permissions[2].PermissionId);
        role.Title = model.Title += "Test";
        model.Permissions.RemoveAt(4);
        model.Permissions.RemoveAt(3);

        controller.Edit(role);

        Role actual = context.Db<Role>().Single(role => role.Id == model.Id);
        RolePermission[] actualPermissions = context.Db<RolePermission>().Where(permission => permission.RoleId == model.Id).ToArray();

        Assert.Equal(model.Permissions.Select(role => role.PermissionId), actualPermissions.Select(role => role.PermissionId));
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(model.Title, actual.Title);
        Assert.Equal(model.Id, actual.Id);
    }

    [Fact]
    public void Edit_Authorization_Refresh()
    {
        controller.Edit(ObjectsFactory.CreateRoleView(model.Id));

        controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
    }

    [Fact]
    public void Edit_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Roles.Index));
        Object actual = controller.Edit(ObjectsFactory.CreateRoleView(model.Id));

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Delete_NotFound()
    {
        Object expected = controller.StaticNotFoundView();
        Object actual = controller.Delete(0);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    [Fact]
    public void Delete_View()
    {
        Role expected = model;
        RoleView actual = controller.Delete(model.Id).Returns<RoleView>();

        Assert.Equal(expected.Permissions.Select(role => role.PermissionId), actual.Permissions.SelectedIds);
        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Title, actual.Title);
        AssertPermissionTree(actual.Permissions);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
    }

    [Fact]
    public void Delete_UpdateAccounts()
    {
        AccountCreateView otherAccount = ObjectsFactory.CreateAccountCreateView(2);
        AccountCreateView account = ObjectsFactory.CreateAccountCreateView(1);
        otherAccount.RoleId = otherModel.Id;
        account.RoleId = model.Id;

        accounts.Create(account);
        accounts.Create(otherAccount);

        controller.DeleteConfirmed(model.Id);

        Assert.Single(context.Db<Account>(), account => account.RoleId == null);
        Assert.Single(context.Db<Account>(), account => account.RoleId == otherModel.Id);
    }

    [Fact]
    public void DeleteConfirmed_Role()
    {
        controller.DeleteConfirmed(model.Id);

        Assert.Single(context.Db<Role>(), role => role.Id != model.Id);
    }

    [Fact]
    public void DeleteConfirmed_Authorization_Refresh()
    {
        controller.DeleteConfirmed(model.Id);

        controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
    }

    [Fact]
    public void DeleteConfirmed_Success_Redirect()
    {
        Object expected = controller.StaticRedirectTo(nameof(Roles.Index));
        Object actual = controller.DeleteConfirmed(model.Id);

        Assert.Empty(controller.ModelState);
        Assert.Empty(controller.Alerts);
        Assert.Same(expected, actual);
    }

    private void AssertPermissionTree(MvcTree permissions)
    {
        Assert.Single(permissions.Nodes);
        Assert.Null(permissions.Nodes[0].Id);
        Assert.Equal(3, permissions.Nodes[0].Children.Count);
        Assert.Equal(Resource.ForString("All"), permissions.Nodes[0].Title);

        Assert.Null(permissions.Nodes[0].Children[0].Id);
        Assert.Equal(2, permissions.Nodes[0].Children[0].Children.Count);
        Assert.Equal(Resource.ForArea(nameof(Area.Administration)), permissions.Nodes[0].Children[0].Title);

        Assert.Null(permissions.Nodes[0].Children[0].Children[0].Id);
        Assert.Single(permissions.Nodes[0].Children[0].Children[0].Children);
        Assert.Equal(Resource.ForController($"{nameof(Area.Administration)}/{nameof(Accounts)}"), permissions.Nodes[0].Children[0].Children[0].Title);

        Assert.NotNull(permissions.Nodes[0].Children[0].Children[0].Children[0].Id);
        Assert.Empty(permissions.Nodes[0].Children[0].Children[0].Children[0].Children);
        Assert.Equal(Resource.ForAction(nameof(Accounts.Edit)), permissions.Nodes[0].Children[0].Children[0].Children[0].Title);

        Assert.Null(permissions.Nodes[0].Children[0].Children[1].Id);
        Assert.Equal(2, permissions.Nodes[0].Children[0].Children[1].Children.Count);
        Assert.Equal(Resource.ForController($"{nameof(Area.Administration)}/{nameof(Roles)}"), permissions.Nodes[0].Children[0].Children[1].Title);

        Assert.NotNull(permissions.Nodes[0].Children[0].Children[1].Children[0].Id);
        Assert.Empty(permissions.Nodes[0].Children[0].Children[1].Children[0].Children);
        Assert.Equal(Resource.ForAction(nameof(Roles.Create)), permissions.Nodes[0].Children[0].Children[1].Children[0].Title);

        Assert.NotNull(permissions.Nodes[0].Children[0].Children[1].Children[1].Id);
        Assert.Empty(permissions.Nodes[0].Children[0].Children[1].Children[1].Children);
        Assert.Equal(Resource.ForAction(nameof(Roles.Delete)), permissions.Nodes[0].Children[0].Children[1].Children[1].Title);

        Assert.Null(permissions.Nodes[0].Children[1].Id);
        Assert.Single(permissions.Nodes[0].Children[1].Children);
        Assert.Equal(Resource.ForController(nameof(Auth)), permissions.Nodes[0].Children[1].Title);

        Assert.NotNull(permissions.Nodes[0].Children[1].Children[0].Id);
        Assert.Empty(permissions.Nodes[0].Children[1].Children[0].Children);
        Assert.Equal(Resource.ForAction(nameof(Auth.Recover)), permissions.Nodes[0].Children[1].Children[0].Title);

        Assert.Null(permissions.Nodes[0].Children[2].Id);
        Assert.Single(permissions.Nodes[0].Children[2].Children);
        Assert.Equal(Resource.ForController(nameof(Profile)), permissions.Nodes[0].Children[2].Title);

        Assert.NotNull(permissions.Nodes[0].Children[2].Children[0].Id);
        Assert.Empty(permissions.Nodes[0].Children[2].Children[0].Children);
        Assert.Equal(Resource.ForAction(nameof(Profile.Delete)), permissions.Nodes[0].Children[2].Children[0].Title);
    }
}
