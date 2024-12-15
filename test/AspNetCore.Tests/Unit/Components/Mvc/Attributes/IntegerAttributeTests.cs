using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class IntegerAttributeTests
{
    private IntegerAttribute attribute;

    public IntegerAttributeTests()
    {
        attribute = new IntegerAttribute();
    }

    [Fact]
    public void IntegerAttribute_SetsErrorMessage()
    {
        attribute = new IntegerAttribute();

        Assert.Equal(Validation.For("Integer", "Test"), attribute.FormatErrorMessage("Test"));
    }

    [Fact]
    public void IsValid_Null()
    {
        Assert.True(attribute.IsValid(null));
    }

    [Fact]
    public void IsValid_RealValue_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(12.549));
    }

    [Fact]
    public void IsValid_IntegerValue()
    {
        Assert.True(attribute.IsValid(-254679849874447));
    }

    [Fact]
    public void IsValid_LongIntegerValue()
    {
        Assert.True(attribute.IsValid("+92233720368547758074878484887777"));
    }
}
