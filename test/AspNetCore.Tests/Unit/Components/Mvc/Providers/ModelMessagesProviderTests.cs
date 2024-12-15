using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class ModelMessagesProviderTests
{
    private DefaultModelBindingMessageProvider messages;

    public ModelMessagesProviderTests()
    {
        messages = new DefaultModelBindingMessageProvider();
        ModelMessagesProvider.Set(messages);
    }

    [Fact]
    public void ModelMessagesProvider_SetsAttemptedValueIsInvalidAccessor()
    {
        String actual = messages.AttemptedValueIsInvalidAccessor("Test", "Property");
        String expected = Validation.For("InvalidField", "Property");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ModelMessagesProvider_SetsUnknownValueIsInvalidAccessor()
    {
        Assert.Equal(Validation.For("InvalidField", "Property"), messages.UnknownValueIsInvalidAccessor("Property"));
    }

    [Fact]
    public void ModelMessagesProvider_SetsMissingBindRequiredValueAccessor()
    {
        String actual = messages.MissingBindRequiredValueAccessor("Property");
        String expected = Validation.For("Required", "Property");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ModelMessagesProvider_SetsValueMustNotBeNullAccessor()
    {
        Assert.Equal(Validation.For("Required", "Property"), messages.ValueMustNotBeNullAccessor("Property"));
    }

    [Fact]
    public void ModelMessagesProvider_ValueIsInvalidAccessor()
    {
        Assert.Equal(Validation.For("InvalidValue", "Value"), messages.ValueIsInvalidAccessor("Value"));
    }

    [Fact]
    public void ModelMessagesProvider_SetsValueMustBeANumberAccessor()
    {
        Assert.Equal(Validation.For("Number", "Property"), messages.ValueMustBeANumberAccessor("Property"));
    }

    [Fact]
    public void ModelMessagesProvider_SetsMissingKeyOrValueAccessor()
    {
        Assert.Equal(Validation.For("RequiredValue"), messages.MissingKeyOrValueAccessor());
    }
}
