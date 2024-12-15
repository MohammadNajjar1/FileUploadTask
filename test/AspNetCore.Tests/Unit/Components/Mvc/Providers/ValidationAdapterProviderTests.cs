namespace AspNetCore.Components.Mvc;

public class ValidationAdapterProviderTests
{
    private ValidationAdapterProvider provider;

    public ValidationAdapterProviderTests()
    {
        provider = new ValidationAdapterProvider();
    }

    [Fact]
    public void GetAttributeAdapter_Required()
    {
        Assert.IsType<RequiredAdapter>(provider.GetAttributeAdapter(new RequiredAttribute(), null));
    }

    [Fact]
    public void GetAttributeAdapter_FileSize()
    {
        Assert.IsType<FileSizeAdapter>(provider.GetAttributeAdapter(new FileSizeAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_StringLength()
    {
        Assert.IsType<StringLengthAdapter>(provider.GetAttributeAdapter(new StringLengthAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_Email()
    {
        Assert.IsType<EmailAddressAdapter>(provider.GetAttributeAdapter(new EmailAddressAttribute(), null));
    }

    [Fact]
    public void GetAttributeAdapter_GreaterThan()
    {
        Assert.IsType<GreaterThanAdapter>(provider.GetAttributeAdapter(new GreaterThanAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_AcceptFiles()
    {
        Assert.IsType<AcceptFilesAdapter>(provider.GetAttributeAdapter(new AcceptFilesAttribute(".docx"), null));
    }

    [Fact]
    public void GetAttributeAdapter_MinLength()
    {
        Assert.IsType<MinLengthAdapter>(provider.GetAttributeAdapter(new MinLengthAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_MaxLength()
    {
        Assert.IsType<MaxLengthAdapter>(provider.GetAttributeAdapter(new MaxLengthAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_MaxValue()
    {
        Assert.IsType<MaxValueAdapter>(provider.GetAttributeAdapter(new MaxValueAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_MinValue()
    {
        Assert.IsType<MinValueAdapter>(provider.GetAttributeAdapter(new MinValueAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_LessThan()
    {
        Assert.IsType<LessThanAdapter>(provider.GetAttributeAdapter(new LessThanAttribute(45), null));
    }

    [Fact]
    public void GetAttributeAdapter_Numeric()
    {
        Assert.IsType<NumericAdapter>(provider.GetAttributeAdapter(new NumericAttribute(7, 3), null));
    }

    [Fact]
    public void GetAttributeAdapter_Compare()
    {
        Assert.IsType<CompareAdapter>(provider.GetAttributeAdapter(new CompareAttribute("Other"), null));
    }

    [Fact]
    public void GetAttributeAdapter_Integer()
    {
        Assert.IsType<IntegerAdapter>(provider.GetAttributeAdapter(new IntegerAttribute(), null));
    }

    [Fact]
    public void GetAttributeAdapter_Digits()
    {
        Assert.IsType<DigitsAdapter>(provider.GetAttributeAdapter(new DigitsAttribute(), null));
    }

    [Fact]
    public void GetAttributeAdapter_Range()
    {
        Assert.IsType<RangeAdapter>(provider.GetAttributeAdapter(new RangeAttribute(4, 77), null));
    }

    [Fact]
    public void GetAttributeAdapter_Unsupported()
    {
        Assert.Null(provider.GetAttributeAdapter(new DataTypeAttribute(DataType.Custom), null));
    }
}
