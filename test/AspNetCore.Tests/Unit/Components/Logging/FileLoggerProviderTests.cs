namespace AspNetCore.Components.Logging;

public class FileLoggerProviderTests
{
    [Fact]
    public void Create_SameInstance()
    {
        using FileLoggerProvider provider = new("log.txt", 10);

        Assert.Same(provider.CreateLogger("1"), provider.CreateLogger("2"));
    }
}
