namespace AspNetCore.Components.Security;

public class AuthorizeAsAttributeTests
{
    [Fact]
    public void AuthorizeAsAttribute_SetsAction()
    {
        Assert.Equal("Action", new AuthorizeAsAttribute("Action").Action);
    }
}
