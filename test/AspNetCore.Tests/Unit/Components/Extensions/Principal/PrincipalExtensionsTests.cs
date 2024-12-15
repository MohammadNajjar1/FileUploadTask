using System.Security.Claims;

namespace AspNetCore.Components.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void Id_NoClaim_Zero()
    {
        Assert.Equal(0, new ClaimsPrincipal().Id());
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("1", 1)]
    public void Id_ReturnsNameIdentifierClaim(String identifier, Int64 id)
    {
        ClaimsIdentity identity = new(new[] { new Claim(ClaimTypes.NameIdentifier, identifier) });

        Assert.Equal(id, new ClaimsPrincipal(identity).Id());
    }

    [Fact]
    public void UpdateClaim_New()
    {
        ClaimsIdentity identity = new();
        ClaimsPrincipal principal = new(identity);

        principal.UpdateClaim(ClaimTypes.Name, "Testing name");

        Assert.Equal("Testing name", principal.FindFirstValue(ClaimTypes.Name));
    }

    [Fact]
    public void UpdateClaim_Existing()
    {
        ClaimsIdentity identity = new();
        ClaimsPrincipal principal = new(identity);
        identity.AddClaim(new Claim(ClaimTypes.Name, "ClaimTypeName"));

        principal.UpdateClaim(ClaimTypes.Name, "Testing name");

        Assert.Equal("Testing name", principal.FindFirstValue(ClaimTypes.Name));
    }
}
