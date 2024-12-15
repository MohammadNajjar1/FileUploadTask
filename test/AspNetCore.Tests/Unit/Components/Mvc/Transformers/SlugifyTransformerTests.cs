namespace AspNetCore.Components.Mvc;

public class SlugifyTransformerTests
{
    [Fact]
    public void TransformOutbound_SameValues()
    {
        String? expected = new SlugifyTransformer().TransformOutbound("TestOne");
        String? actual = new SlugifyTransformer().TransformOutbound("TestOne");

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(1, null)]
    [InlineData(null, null)]
    [InlineData("test", "test")]
    [InlineData("Test", "Test")]
    [InlineData("VIPWeek", "VIP-Week")]
    [InlineData("TestWeek", "Test-Week")]
    [InlineData("TestIWeek", "Test-I-Week")]
    [InlineData("TestInWeek", "Test-In-Week")]
    [InlineData("TestWeekVIP", "Test-Week-VIP")]
    [InlineData("VIPWeekMulti", "VIP-Week-Multi")]
    [InlineData("TestWeekMulti", "Test-Week-Multi")]
    [InlineData("TestWEEKMulti", "Test-WEEK-Multi")]
    public void TransformOutbound_Value(Object? value, String? slug)
    {
        Assert.Equal(slug, new SlugifyTransformer().TransformOutbound(value));
    }
}
