namespace AspNetCore.Objects;

public class AModelTests
{
    [Fact]
    public void CreationDate_ReturnsSameValue()
    {
        AModel model = Substitute.For<AModel>();

        Assert.Equal(model.CreationDate, model.CreationDate);
    }
}
