using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.Components.Mvc;

public class ClientValidatorProviderTests
{
    [Theory]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTime?))]
    public void CreateValidators_ForDate(Type type)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
        ClientValidatorProviderContext context = new(metadata, new List<ClientValidatorItem>());

        new ClientValidatorProvider().CreateValidators(context);

        ClientValidatorItem actual = context.Results.Single();

        Assert.IsType<DateValidator>(actual.Validator);
        Assert.Null(actual.ValidatorMetadata);
        Assert.True(actual.IsReusable);
    }

    [Theory]
    [InlineData(typeof(Single))]
    [InlineData(typeof(Single?))]
    [InlineData(typeof(Double))]
    [InlineData(typeof(Double?))]
    [InlineData(typeof(Decimal))]
    [InlineData(typeof(Decimal?))]
    public void CreateValidators_ForDecimal(Type type)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
        ClientValidatorProviderContext context = new(metadata, new List<ClientValidatorItem>());

        new ClientValidatorProvider().CreateValidators(context);

        ClientValidatorItem actual = context.Results.Single();

        Assert.IsType<NumberValidator>(actual.Validator);
        Assert.Null(actual.ValidatorMetadata);
        Assert.True(actual.IsReusable);
    }

    [Theory]
    [InlineData(typeof(Byte))]
    [InlineData(typeof(Byte?))]
    [InlineData(typeof(SByte))]
    [InlineData(typeof(SByte?))]
    [InlineData(typeof(Int16))]
    [InlineData(typeof(Int16?))]
    [InlineData(typeof(UInt16))]
    [InlineData(typeof(UInt16?))]
    [InlineData(typeof(Int32))]
    [InlineData(typeof(Int32?))]
    [InlineData(typeof(UInt32))]
    [InlineData(typeof(UInt32?))]
    [InlineData(typeof(Int64))]
    [InlineData(typeof(Int64?))]
    [InlineData(typeof(UInt64))]
    [InlineData(typeof(UInt64?))]
    public void CreateValidators_ForInteger(Type type)
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
        ClientValidatorProviderContext context = new(metadata, new List<ClientValidatorItem>());

        new ClientValidatorProvider().CreateValidators(context);

        ClientValidatorItem actual = context.Results.Single();

        Assert.IsType<IntegerValidator>(actual.Validator);
        Assert.Null(actual.ValidatorMetadata);
        Assert.True(actual.IsReusable);
    }

    [Fact]
    public void CreateValidators_DoesNotCreate()
    {
        ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));
        ClientValidatorProviderContext context = new(metadata, new List<ClientValidatorItem>());

        new ClientValidatorProvider().CreateValidators(context);

        Assert.Empty(context.Results);
    }
}
