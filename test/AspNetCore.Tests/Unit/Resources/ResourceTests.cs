using AspNetCore.Controllers;
using AspNetCore.Controllers.Administration;
using AspNetCore.Objects;

namespace AspNetCore.Resources;

public class ResourceTests
{
    [Fact]
    public void Set_Same()
    {
        Assert.Same(Resource.Set("Test"), Resource.Set("Test"));
    }

    [Fact]
    public void ForArea_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/Shared", "Areas", nameof(Area.Administration));
        String actual = Resource.ForArea(nameof(Area.Administration).ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForArea_NotFound_Empty()
    {
        Assert.Empty(Resource.ForArea("Null"));
    }

    [Fact]
    public void ForAction_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/Shared", "Actions", nameof(Accounts.Create));
        String actual = Resource.ForAction(nameof(Accounts.Create).ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForAction_NotFound_Empty()
    {
        Assert.Empty(Resource.ForAction("Null"));
    }

    [Fact]
    public void ForController_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/Shared", "Controllers", $"{nameof(Area.Administration)}/{nameof(Roles)}");
        String actual = Resource.ForController($"{nameof(Area.Administration)}/{nameof(Roles)}".ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForController_NotFound_Empty()
    {
        Assert.Empty(Resource.ForController("Null"));
    }

    [Fact]
    public void ForLookup_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/Lookup", "Titles", nameof(Role));
        String actual = Resource.ForLookup(nameof(Role).ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForLookup_NotFound_Empty()
    {
        Assert.Empty(Resource.ForLookup("Test"));
    }

    [Fact]
    public void ForString_IsCaseInsensitive()
    {
        Assert.Equal(ResourceFor("Shared/Shared", "Strings", "All"), Resource.ForString("all"));
    }

    [Fact]
    public void ForString_Formats()
    {
        String expected = String.Format(ResourceFor("Shared/Shared", "Strings", "SystemError"), "test");
        String actual = Resource.ForString("SystemError", "test");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForString_NoArguments_Format()
    {
        String expected = ResourceFor("Shared/Shared", "Strings", "SystemError");
        String actual = Resource.ForString("SystemError");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForString_NotFound_Empty()
    {
        Assert.Empty(Resource.ForString("Null"));
    }

    [Fact]
    public void ForHeader_IsCaseInsensitive()
    {
        Assert.Equal(ResourceFor("Shared/Page", "Headers", "Account"), Resource.ForHeader("account"));
    }

    [Fact]
    public void ForHeader_NotFound_Empty()
    {
        Assert.Empty(Resource.ForHeader("Test"));
    }

    [Fact]
    public void ForPage_Path_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/Page", "Titles", $"{nameof(Area.Administration)}/{nameof(Roles)}/{nameof(Roles.Details)}");
        String actual = Resource.ForPage($"{nameof(Area.Administration)}/{nameof(Roles)}/{nameof(Roles.Details)}".ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForPage_PathNotFound_Empty()
    {
        Assert.Empty(Resource.ForPage("Test"));
    }

    [Fact]
    public void ForPage_IsCaseInsensitive()
    {
        IDictionary<String, Object?> values = new Dictionary<String, Object?>
        {
            ["controller"] = nameof(Roles).ToUpper(),
            ["action"] = nameof(Roles.Details).ToUpper(),
            ["area"] = nameof(Area.Administration).ToUpper()
        };

        String expected = ResourceFor("Shared/Page", "Titles", $"{nameof(Area.Administration)}/{nameof(Roles)}/{nameof(Roles.Details)}");
        String actual = Resource.ForPage(values);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ForPage_WithoutArea(String? area)
    {
        IDictionary<String, Object?> values = new Dictionary<String, Object?>
        {
            ["area"] = area,
            ["controller"] = nameof(Profile),
            ["action"] = nameof(Profile.Edit)
        };

        String expected = ResourceFor("Shared/Page", "Titles", $"{nameof(Profile)}/{nameof(Profile.Edit)}");
        String actual = Resource.ForPage(values);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForPage_NotFound_Empty()
    {
        IDictionary<String, Object?> values = new Dictionary<String, Object?>
        {
            ["controller"] = null,
            ["action"] = null,
            ["area"] = null
        };

        Assert.Empty(Resource.ForPage(values));
    }

    [Fact]
    public void ForSiteMap_IsCaseInsensitive()
    {
        String expected = ResourceFor("Shared/SiteMap", "Titles", $"{nameof(Area.Administration)}/{nameof(Roles)}/{nameof(Roles.Index)}");
        String actual = Resource.ForSiteMap($"{nameof(Area.Administration)}/{nameof(Roles)}/{nameof(Roles.Index)}".ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForSiteMap_WithoutControllerAndAction()
    {
        String expected = ResourceFor("Shared/SiteMap", "Titles", nameof(Area.Administration));
        String actual = Resource.ForSiteMap(nameof(Area.Administration));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForSiteMap_NotFound_Empty()
    {
        Assert.Empty(Resource.ForSiteMap("Test/Test/Test"));
    }

    [Fact]
    public void ForProperty_NotMemberLambdaExpression_ReturnNull()
    {
        Assert.Empty(Resource.ForProperty<RoleView, String?>(view => view.ToString()));
    }

    [Fact]
    public void ForProperty_FromLambdaExpression()
    {
        String expected = ResourceFor($"Views/{nameof(Area.Administration)}/{nameof(Accounts)}/{nameof(AccountView)}", "Titles", nameof(AccountView.Username));
        String actual = Resource.ForProperty<AccountView, String?>(account => account.Username);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForProperty_NotFoundLambdaExpression_Empty()
    {
        Assert.Empty(Resource.ForProperty<AccountView, Int64>(account => account.Id));
    }

    [Fact]
    public void ForProperty_NotFoundLambdaType_Empty()
    {
        Assert.Empty(Resource.ForProperty<Object, String?>(test => test.ToString()));
    }

    [Fact]
    public void ForProperty_View()
    {
        String expected = ResourceFor($"Views/{nameof(Area.Administration)}/{nameof(Accounts)}/{nameof(AccountView)}", "Titles", nameof(AccountView.Username));
        String actual = Resource.ForProperty(nameof(AccountView), nameof(AccountView.Username));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForProperty_IsCaseInsensitive()
    {
        String expected = ResourceFor($"Views/{nameof(Area.Administration)}/{nameof(Accounts)}/{nameof(AccountView)}", "Titles", nameof(AccountView.Username));
        String actual = Resource.ForProperty(typeof(AccountView), nameof(AccountView.Username).ToUpper());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForProperty_NotFoundProperty_Empty()
    {
        Assert.Empty(Resource.ForProperty(typeof(AccountView), nameof(AccountView.Id)));
    }

    [Fact]
    public void ForProperty_NotFoundTypeProperty_Empty()
    {
        Assert.Empty(Resource.ForProperty(typeof(Object), nameof(ToString)));
    }

    [Fact]
    public void ForProperty_NotMemberExpression_ReturnNull()
    {
        Expression<Func<RoleView, String?>> lambda = view => view.ToString();

        Assert.Empty(Resource.ForProperty(lambda.Body));
    }

    [Fact]
    public void ForProperty_FromExpression()
    {
        Expression<Func<AccountView, String?>> lambda = account => account.Username;

        String expected = ResourceFor($"Views/{nameof(Area.Administration)}/{nameof(Accounts)}/{nameof(AccountView)}", "Titles", nameof(AccountView.Username));
        String actual = Resource.ForProperty(lambda.Body);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForProperty_NotFoundExpression_Empty()
    {
        Expression<Func<AccountView, Int64>> lambda = account => account.Id;

        Assert.Empty(Resource.ForProperty(lambda.Body));
    }

    [Fact]
    public void ForProperty_NotFoundType_Empty()
    {
        Expression<Func<Object, String?>> lambda = test => test.ToString();

        Assert.Empty(Resource.ForProperty(lambda.Body));
    }

    private String ResourceFor(String path, String group, String key)
    {
        String resource = File.ReadAllText($"Resources/{path}.json");

        return JsonSerializer.Deserialize<Dictionary<String, Dictionary<String, String?>>>(resource)?[group][key] ?? "";
    }
}
