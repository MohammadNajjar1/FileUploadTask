using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCore.Components.Mvc;

public class ModelStateDictionaryExtensionsTests
{
    [Fact]
    public void Errors_FromModelState()
    {
        ModelStateDictionary modelState = new();
        modelState.AddModelError("InnerParent.PropertyTitle", "Error");
        modelState.AddModelError("WhitespaceErrors", "           ");
        modelState.AddModelError("WhitespaceErrors", "Whitespace");
        modelState.AddModelError("TwoErrors", "Error1");
        modelState.AddModelError("TwoErrors", "Error2");
        modelState.SetModelValue("Normal", "1", "1");
        modelState.AddModelError("EmptyErrors", "E");
        modelState.AddModelError("EmptyErrors", "");
        modelState.AddModelError("Error", "Error");
        modelState.AddModelError("Empty", "");

        Dictionary<String, String?> actual = modelState.Errors();

        Assert.Equal("Error", actual["InnerParent.PropertyTitle"]);
        Assert.Equal("           ", actual["WhitespaceErrors"]);
        Assert.Equal("Error1", actual["TwoErrors"]);
        Assert.Equal("E", actual["EmptyErrors"]);
        Assert.Equal("Error", actual["Error"]);
        Assert.Equal(6, actual.Count);
        Assert.Null(actual["Empty"]);
    }
}
