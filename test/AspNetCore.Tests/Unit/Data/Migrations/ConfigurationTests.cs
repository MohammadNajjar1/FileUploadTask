using Microsoft.EntityFrameworkCore;
using AspNetCore.Objects;

namespace AspNetCore.Data.Migrations;

[Collection("Database access")]
public class ConfigurationTests : IDisposable
{
    private Configuration configuration;
    private DbContext context;

    public ConfigurationTests()
    {
        context = TestingContext.Create();
        configuration = new Configuration(context, TestingContext.Mapper);

        context.Drop();
    }
    public void Dispose()
    {
        configuration.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Seed_Roles()
    {
        configuration.Seed();

        Assert.Single(context.Db<Role>(), role => role.Title == "Sys_Admin");
    }

    [Fact]
    public void Seed_Accounts()
    {
        configuration.Seed();

        Assert.NotNull(context.Db<Account>().Single(account => account.Username == "admin" && account.Role!.Title == "Sys_Admin"));
    }

    [Theory]
    [InlineData("Administration", "Accounts", "Create")]
    [InlineData("Administration", "Accounts", "Edit")]
    [InlineData("Administration", "Accounts", "Index")]
    [InlineData("Administration", "Roles", "Create")]
    [InlineData("Administration", "Roles", "Delete")]
    [InlineData("Administration", "Roles", "Edit")]
    [InlineData("Administration", "Roles", "Index")]
    public void Seed_Permissions(String area, String controller, String action)
    {
        configuration.Seed();

        Assert.Single(context.Db<Permission>(), permission =>
            permission.Controller == controller &&
            permission.Action == action &&
            permission.Area == area);
    }

    [Fact]
    public void Seed_RemovesUnusedPermissions()
    {
        Role role = ObjectsFactory.CreateRole(0);
        role.Permissions.Add(new RolePermission
        {
            Permission = new Permission { Area = "Test", Controller = "Test", Action = "Test" }
        });

        context.Add(role);
        context.SaveChanges();

        configuration.Seed();

        Assert.Empty(context.Set<Permission>().Where(permission =>
            permission.Controller == "Test" &&
            permission.Action == "Test" &&
            permission.Area == "Test"));
    }

    [Fact]
    public void Seed_PermissionCount()
    {
        configuration.Seed();

        Int32 actual = context.Set<Permission>().Count();
        Int32 expected = GetType()
            .GetMethod(nameof(Seed_Permissions))!
            .GetCustomAttributes<InlineDataAttribute>()
            .Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Seed_RolePermissions()
    {
        configuration.Seed();

        IEnumerable<Int64> expected = context
            .Set<Permission>()
            .Select(permission => permission.Id)
            .OrderBy(permissionId => permissionId);

        IEnumerable<Int64> actual = context
            .Set<RolePermission>()
            .Where(permission => permission.Role.Title == "Sys_Admin")
            .Select(rolePermission => rolePermission.PermissionId)
            .OrderBy(permissionId => permissionId);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Seed_MultipleTimes()
    {
        configuration.Seed();
        configuration.Seed();
    }
}
