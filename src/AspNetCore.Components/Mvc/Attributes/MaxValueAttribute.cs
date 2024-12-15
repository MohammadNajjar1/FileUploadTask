using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MaxValueAttribute : ValidationAttribute
{
    public Decimal Maximum { get; }

    public MaxValueAttribute(Double maximum)
        : base(() => Validation.For("MaxValue"))
    {
        Maximum = Convert.ToDecimal(maximum);
    }

    public override String FormatErrorMessage(String name)
    {
        return String.Format(ErrorMessageString, name, Maximum);
    }
    public override Boolean IsValid(Object? value)
    {
        try
        {
            return value == null || Convert.ToDecimal(value) <= Maximum;
        }
        catch
        {
            return false;
        }
    }
}
