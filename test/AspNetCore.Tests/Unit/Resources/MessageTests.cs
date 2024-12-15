using AspNetCore.Objects;

namespace AspNetCore.Resources;

public class MessageTests
{
    [Fact]
    public void For_IsCaseInsensitive()
    {
        String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Messages", "ProfileUpdated");
        String actual = Message.For<AccountView>("profileupdated");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void For_Format()
    {
        String expected = String.Format(ResourceFor("Views/Administration/Accounts/AccountView", "Messages", "RecoveryEmailBody"), "test");
        String actual = Message.For<AccountView>("RecoveryEmailBody", "test");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void For_NoArguments_Format()
    {
        String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Messages", "RecoveryEmailBody");
        String actual = Message.For<AccountView>("RecoveryEmailBody");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForString_NotFound_Empty()
    {
        Assert.Empty(Message.For<AccountView>("Null"));
    }

    private String ResourceFor(String path, String group, String key)
    {
        String resource = File.ReadAllText($"Resources/{path}.json");

        return JsonSerializer.Deserialize<Dictionary<String, Dictionary<String, String?>>>(resource)?[group][key] ?? "";
    }
}
