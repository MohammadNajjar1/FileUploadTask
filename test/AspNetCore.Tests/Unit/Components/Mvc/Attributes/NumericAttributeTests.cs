using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class NumericAttributeTests
{
    private NumericAttribute attribute;

    public NumericAttributeTests()
    {
        attribute = new NumericAttribute(5, 2);
    }

    [Fact]
    public void PrecisionAttribute_SetsErrorMessage()
    {
        String actual = new NumericAttribute(5, 2).FormatErrorMessage("Test");
        String expected = Validation.For("Numeric", "Test", 3, 2);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_Null()
    {
        Assert.True(attribute.IsValid(null));
    }

    [Fact]
    public void IsValid_HigherPrecisionInteger_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(1000));
    }

    [Fact]
    public void IsValid_HigherPrecisionDecimal_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(1234.45M));
    }

    [Fact]
    public void IsValid_HigherScaleFloat_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(1.234F));
    }

    [Fact]
    public void IsValid_HigherScale_ReturnsFalse()
    {
        Assert.False(attribute.IsValid("1.234"));
    }

    [Fact]
    public void IsValid_InvalidNumber_ReturnsFalse()
    {
        Assert.False(attribute.IsValid("test"));
    }

    [Fact]
    public void IsValid_DecimalPrecision()
    {
        Assert.True(new NumericAttribute(3, 3).IsValid(0.345M));
    }

    [Fact]
    public void IsValid_DecimalValue()
    {
        Assert.True(attribute.IsValid(123.45M));
    }

    [Fact]
    public void IsValid_IntegerValue()
    {
        Assert.True(attribute.IsValid("123"));
    }
}
