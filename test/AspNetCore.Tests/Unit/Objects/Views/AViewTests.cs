namespace AspNetCore.Objects;

public class AViewTests
{
    [Fact]
    public void CreationDate_ReturnsSameValue()
    {
        AView view = Substitute.For<AView>();

        Assert.Equal(view.CreationDate, view.CreationDate);
    }
}
