using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public static class ModelMessagesProvider
{
    public static void Set(DefaultModelBindingMessageProvider messages)
    {
        messages.SetAttemptedValueIsInvalidAccessor((_, field) => Validation.For("InvalidField", field));
        messages.SetUnknownValueIsInvalidAccessor(field => Validation.For("InvalidField", field));
        messages.SetMissingBindRequiredValueAccessor(field => Validation.For("Required", field));
        messages.SetValueMustNotBeNullAccessor(field => Validation.For("Required", field));
        messages.SetValueIsInvalidAccessor(value => Validation.For("InvalidValue", value));
        messages.SetValueMustBeANumberAccessor(field => Validation.For("Number", field));
        messages.SetMissingKeyOrValueAccessor(() => Validation.For("RequiredValue"));
    }
}
