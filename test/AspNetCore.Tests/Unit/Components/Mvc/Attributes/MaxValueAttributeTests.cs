using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class MaxValueAttributeTests
{
    private MaxValueAttribute attribute;

    public MaxValueAttributeTests()
    {
        attribute = new MaxValueAttribute(12.56);
    }

    [Fact]
    public void MaxValueAttribute_SetsMaximum()
    {
        Assert.Equal(12.56M, new MaxValueAttribute(12.56).Maximum);
    }

    [Fact]
    public void FormatErrorMessage_ForName()
    {
        attribute = new MaxValueAttribute(13.44);

        String expected = Validation.For("MaxValue", "Sum", attribute.Maximum);
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
    [InlineData("12.559")]
    public void IsValid_LowerOrEqualValue(Object value)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        Assert.True(attribute.IsValid(value));
    }

    [Fact]
    public void IsValid_GreaterValue_ReturnsFalse()
    {
        Assert.False(attribute.IsValid(12.5601));
    }

    [Fact]
    public void IsValid_NotDecimal_ReturnsFalse()
    {
        Assert.False(attribute.IsValid("12.56M"));
    }
}
