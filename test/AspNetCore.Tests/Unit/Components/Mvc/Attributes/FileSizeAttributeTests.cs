using Microsoft.AspNetCore.Http;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class FileSizeAttributeTests
{
    private FileSizeAttribute attribute;

    public FileSizeAttributeTests()
    {
        attribute = new FileSizeAttribute(12.25);
    }

    [Fact]
    public void FileSizeAttribute_SetsMaximumMB()
    {
        Assert.Equal(12.25M, new FileSizeAttribute(12.25).MaximumMB);
    }

    [Fact]
    public void FormatErrorMessage_ForName()
    {
        attribute = new FileSizeAttribute(12.25);

        String expected = Validation.For("FileSize", "File", attribute.MaximumMB);
        String actual = attribute.FormatErrorMessage("File");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_Null()
    {
        Assert.True(attribute.IsValid(null));
    }

    [Fact]
    public void IsValid_NotIFormFileValueIsValid()
    {
        Assert.True(attribute.IsValid("100"));
    }

    [Theory]
    [InlineData(240546)]
    [InlineData(12845056)]
    public void IsValid_LowerOrEqualFileSize(Int64 length)
    {
        IFormFile file = new FormFile(new MemoryStream(), 0, length, "File", "file.txt");

        Assert.True(attribute.IsValid(file));
    }

    [Fact]
    public void IsValid_GreaterThanMaximumIsNotValid()
    {
        IFormFile file = new FormFile(new MemoryStream(), 0, 12845057, "File", "file.txt");

        Assert.False(attribute.IsValid(file));
    }

    [Theory]
    [InlineData(240546, 4574)]
    [InlineData(12840000, 5056)]
    public void IsValid_LowerOrEqualFileSizes(Int64 firstLength, Int64 secondLength)
    {
        IFormFile[] files =
        {
            new FormFile(new MemoryStream(), 0, firstLength, "FirstFile", "first.txt"),
            new FormFile(new MemoryStream(), 0, secondLength, "SecondFile", "second.txt")
        };

        Assert.True(attribute.IsValid(files));
    }

    [Fact]
    public void IsValid_GreaterThanMaximumSizesAreNotValid()
    {
        IFormFile[] files =
        {
            new FormFile(new MemoryStream(), 0, 5057, "FirstFile", "first.txt"),
            new FormFile(new MemoryStream(), 0, 12840000, "SecondFile", "second.txt")
        };

        Assert.False(attribute.IsValid(files));
    }
}
