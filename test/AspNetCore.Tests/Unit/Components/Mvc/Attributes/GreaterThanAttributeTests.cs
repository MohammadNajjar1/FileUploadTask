using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class GreaterThanAttributeTests
{
    private GreaterThanAttribute attribute;

    public GreaterThanAttributeTests()
    {
        attribute = new GreaterThanAttribute(12.56);
    }

    [Fact]
    public void GreaterThanAttribute_SetsMinimum()
    {
        Assert.Equal(12.56M, new GreaterThanAttribute(12.56).Minimum);
    }

    [Fact]
    public void FormatErrorMessage_ForName()
    {
        attribute = new GreaterThanAttribute(12.56);

        String expected = Validation.For("GreaterThan", "Sum", attribute.Minimum);
        String actual = attribute.FormatErrorMessage("Sum");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_Null()
    {
        Assert.True(attribute.IsValid(null));
    }

    [Theory]
    [InlineData("1")]
    [InlineData(12.56)]
    [InlineData(12.559)]
    public void IsValid_LowerOrEqualValue_ReturnsFalse(Object value)
    {
        Assert.False(attribute.IsValid(value));
    }

    [Theory]
    [InlineData(13)]
    [InlineData("100")]
    public void IsValid_GreaterValue(Object value)
    {
        Assert.True(attribute.IsValid(value));
    }

    [Fact]
    public void IsValid_NotDecimal_ReturnsFalse()
    {
        Assert.False(attribute.IsValid("12.60M"));
    }
}
