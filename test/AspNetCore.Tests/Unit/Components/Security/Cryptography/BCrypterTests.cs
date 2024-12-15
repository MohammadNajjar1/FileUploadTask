namespace AspNetCore.Components.Security;

public class BCrypterTests
{
    private BCrypter crypter;

    public BCrypterTests()
    {
        crypter = new BCrypter();
    }

    [Fact]
    public void Hash_Value()
    {
        String value = "Test";
        String hash = crypter.Hash(value);

        Assert.True(BCrypt.Net.BCrypt.Verify(value, hash));
    }

    [Fact]
    public void HashPassword_Value()
    {
        String value = "Test";
        String hash = crypter.HashPassword(value);

        Assert.True(BCrypt.Net.BCrypt.Verify(value, hash));
    }

    [Fact]
    public void Verify_NullValue_ReturnsFalse()
    {
        Assert.False(crypter.Verify(null, ""));
    }

    [Fact]
    public void Verify_NullHash_ReturnsFalse()
    {
        Assert.False(crypter.Verify("", null));
    }

    [Fact]
    public void Verify_Hash()
    {
        Assert.True(crypter.Verify("Test", "$2a$04$tXfDH9cZGOqFbCV8CF1ik.kW8R7.UKpEi5G7P4K842As1DI1bwDxm"));
    }

    [Fact]
    public void VerifyPassword_NullValue_ReturnsFalse()
    {
        Assert.False(crypter.VerifyPassword(null, ""));
    }

    [Fact]
    public void VerifyPassword_NullHash_ReturnsFalse()
    {
        Assert.False(crypter.VerifyPassword("", null));
    }

    [Fact]
    public void VerifyPassword_Passhash()
    {
        Assert.True(crypter.VerifyPassword("Test", "$2a$13$g7QgmyFicKkyI4kiHM8XQ.LfBdpdcLUbw1tkr9.owCN5qY9eCj8Lq"));
    }
}
