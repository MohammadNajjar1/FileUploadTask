namespace AspNetCore.Components.Tree;

public class MvcTreeTests
{
    [Fact]
    public void MvcTree_CreatesEmpty()
    {
        MvcTree actual = new();

        Assert.Empty(actual.Nodes);
        Assert.Empty(actual.SelectedIds);
    }
}
