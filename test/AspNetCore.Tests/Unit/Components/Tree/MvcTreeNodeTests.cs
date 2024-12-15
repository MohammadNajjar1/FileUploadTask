namespace AspNetCore.Components.Tree;

public class MvcTreeNodeTests
{
    [Fact]
    public void MvcTreeNode_SetsTitle()
    {
        MvcTreeNode actual = new("Nodey");

        Assert.Equal("Nodey", actual.Title);
        Assert.Empty(actual.Children);
        Assert.Null(actual.Id);
    }

    [Fact]
    public void MvcTreeNode_SetsIdAndTitle()
    {
        MvcTreeNode actual = new(1, "Nodey");

        Assert.Equal("Nodey", actual.Title);
        Assert.Empty(actual.Children);
        Assert.Equal(1, actual.Id);
    }
}
