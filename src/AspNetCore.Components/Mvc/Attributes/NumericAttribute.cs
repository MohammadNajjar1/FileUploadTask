using AspNetCore.Resources;
using System.Data.SqlTypes;

namespace AspNetCore.Components.Mvc;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NumericAttribute : ValidationAttribute
{
    public Int32 Scale { get; }
    public Int32 Precision { get; }

    public NumericAttribute(Int32 precision, Int32 scale)
        : base(() => Validation.For("Numeric"))
    {
        Precision = precision;
        Scale = scale;
    }

    public override String FormatErrorMessage(String name)
    {
        return String.Format(ErrorMessageString, name, Precision - Scale, Scale);
    }
    public override Boolean IsValid(Object? value)
    {
        if (value == null)
            return true;

        try
        {
            SqlDecimal number = new(Trim(Convert.ToDecimal(value)));

            return number.Precision - number.Scale <= Precision - Scale && number.Scale <= Scale;
        }
        catch
        {
            return false;
        }
    }

    private Decimal Trim(Decimal value)
    {
        String trimmed = Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
        trimmed = trimmed.Contains('.') ? trimmed.TrimEnd('0') : trimmed;

        return Convert.ToDecimal(trimmed, CultureInfo.InvariantCulture);
    }
}
