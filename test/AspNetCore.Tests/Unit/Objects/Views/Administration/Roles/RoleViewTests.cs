using AspNetCore.Components.Tree;

namespace AspNetCore.Objects;

public class RoleViewTests
{
    [Fact]
    public void RoleView_CreatesEmpty()
    {
        MvcTree actual = new RoleView().Permissions;

        Assert.Empty(actual.SelectedIds);
        Assert.Empty(actual.Nodes);
    }
}
