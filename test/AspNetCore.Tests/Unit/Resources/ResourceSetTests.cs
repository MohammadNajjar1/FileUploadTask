namespace AspNetCore.Resources;

public class ResourceSetTests
{
    private ResourceSet resource;

    public ResourceSetTests()
    {
        resource = new ResourceSet();
    }

    [Theory]
    [InlineData("Test", "Test")]
    [InlineData("Test", "Group")]
    [InlineData("Language", "Test")]
    public void Indexer_Group_NotFound_ReturnsNull(String language, String group)
    {
        resource["Language", "Group", "Key"] = "test";

        Assert.Null(resource[language, group]);
    }

    [Theory]
    [InlineData("Test", "Group", "Key")]
    [InlineData("Language", "Test", "Key")]
    [InlineData("Language", "Group", "Test")]
    public void Indexer_NotFound_ReturnsNull(String language, String group, String key)
    {
        resource["Language", "Group", "Key"] = "test";

        Assert.Null(resource[language, group, key]);
    }

    [Fact]
    public void Indexer_Key_IsCaseInsensitive()
    {
        resource["A", "B", "C"] = "test";

        Assert.Equal("test", resource["A", "B", "c"]);
    }

    [Theory]
    [InlineData("Test", "Group", "Key")]
    [InlineData("Language", "Test", "Key")]
    [InlineData("Language", "Group", "Test")]
    public void Indexer_SetsResource(String language, String group, String key)
    {
        resource["Language", "Group", "Key"] = "existing resource";
        resource[language, group, key] = "new testing resource for override";

        Assert.Equal("new testing resource for override", resource[language, group, key]);
    }

    [Fact]
    public void Override_Resources()
    {
        resource["Language", "Group", "Key"] = "existing resource";

        resource.Override("Language", @"{ ""Group"": { ""Test"": ""test"", ""Key"": ""new resource"" } }");

        Assert.Equal("new resource", resource["Language", "Group", "Key"]);
        Assert.Equal("test", resource["Language", "Group", "Test"]);
    }

    [Fact]
    public void Inherit_Resources()
    {
        ResourceSet fallback = new();

        resource["Language", "Group", "Key"] = "existing resource";
        fallback.Override("Language", @"{ ""Group"": { ""Test"": ""test"", ""Key"": ""fallback resource"" } }");

        resource.Inherit(fallback);

        Assert.Equal("existing resource", resource["Language", "Group", "Key"]);
        Assert.Equal("test", resource["Language", "Group", "Test"]);
    }
}
