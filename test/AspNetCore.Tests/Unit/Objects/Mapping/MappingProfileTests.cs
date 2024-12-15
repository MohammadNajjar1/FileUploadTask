using AspNetCore.Objects;

namespace AspNetCore.Data.Mapping;

public class MappingProfileTests
{
    [Fact]
    public void Map_Role_RoleView()
    {
        Role role = ObjectsFactory.CreateRole(0);
        role.Permissions[0].PermissionId = 111;
        role.Permissions[1].PermissionId = 22;
        role.Permissions[2].PermissionId = 33;
        role.Permissions[3].PermissionId = 4;
        role.Permissions[4].PermissionId = 5;

        RoleView actual = TestingContext.Mapper.Map<RoleView>(role);

        Assert.Equal(new[] { 111L, 22L, 33L, 4L, 5L }, actual.Permissions.SelectedIds);
        Assert.Equal(role.CreationDate, actual.CreationDate);
        Assert.Equal(role.Title, actual.Title);
        Assert.Empty(actual.Permissions.Nodes);
        Assert.Equal(role.Id, actual.Id);
    }

    [Fact]
    public void Map_RoleView_Role()
    {
        RoleView role = ObjectsFactory.CreateRoleView(0);
        role.Permissions.SelectedIds.Add(111);
        role.Permissions.SelectedIds.Add(22);

        Role actual = TestingContext.Mapper.Map<Role>(role);

        Assert.Equal(new[] { 111L, 22L }, actual.Permissions.Select(role => role.PermissionId));
        Assert.Equal(role.CreationDate, actual.CreationDate);
        Assert.Equal(role.Title, actual.Title);
        Assert.Equal(role.Id, actual.Id);
    }
}
