using AspNetCore.Objects;

namespace AspNetCore.Resources;

public class ValidationTests
{
    [Fact]
    public void For_IsCaseInsensitive()
    {
        String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Validations", "ExpiredToken");
        String actual = Validation.For<AccountView>("expiredtoken");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void For_Format()
    {
        String expected = String.Format(ResourceFor("Views/Administration/Accounts/AccountView", "Validations", "ExpiredToken"), "test");
        String actual = Validation.For<AccountView>("ExpiredToken", "test");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void For_NoArguments_Format()
    {
        String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Validations", "ExpiredToken");
        String actual = Validation.For<AccountView>("ExpiredToken");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForString_NotFound_Empty()
    {
        Assert.Empty(Validation.For<AccountView>("Null"));
    }

    private String ResourceFor(String path, String group, String key)
    {
        String resource = File.ReadAllText($"Resources/{path}.json");

        return JsonSerializer.Deserialize<Dictionary<String, Dictionary<String, String?>>>(resource)?[group][key] ?? "";
    }
}
