using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MinValueAttributeTests
{
    private MinValueAttribute attribute;

    public MinValueAttributeTests()
    {
        attribute = new MinValueAttribute(12.56);
    }

    [Fact]
    public void MinValueAttribute_SetsMinimum()
    {
        Assert.Equal(12.56M, new MinValueAttribute(12.56).Minimum);
    }

    [Fact]
    public void FormatErrorMessage_ForName()
    {
        attribute = new MinValueAttribute(12.56);

        String expected = Validation.For("MinValue", "Sum", attribute.Minimum);
        String actual = attribute.FormatErrorMessage("Sum");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_Null()
    {
        Assert.True(attribute.IsValid(null));
    }

    [Theory]
    [InlineData(12.56)]
    [InlineData("12.561")]
    public void IsValid_GreaterOrEqualValue(Object value)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        Assert.True(attribute.IsValid(value));
    }

    [Fact]
    public void IsValid_LowerValue_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(12.559));
    }

    [Fact]
    public void IsValid_NotDecimalValueIsNotValid()
    {
        Assert.False(attribute.IsValid("12.56M"));
    }
}
