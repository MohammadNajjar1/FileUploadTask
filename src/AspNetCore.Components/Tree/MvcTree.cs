namespace AspNetCore.Components.Tree;

public class MvcTree
{
    public List<MvcTreeNode> Nodes { get; set; }
    public HashSet<Int64> SelectedIds { get; set; }

    public MvcTree()
    {
        Nodes = new List<MvcTreeNode>();
        SelectedIds = new HashSet<Int64>();
    }
}
