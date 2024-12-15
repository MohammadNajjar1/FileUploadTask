using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Encodings.Web;

namespace AspNetCore.Data;

public class LoggableProperty
{
    public Boolean IsModified { get; }
    private Object? OldValue { get; }
    private Object? NewValue { get; }
    private String Property { get; }

    private static JsonSerializerOptions Options = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public LoggableProperty(PropertyEntry entry, Object? newValue)
    {
        NewValue = newValue;
        OldValue = entry.CurrentValue;
        Property = entry.Metadata.Name;
        IsModified = entry.IsModified && !Equals(NewValue, OldValue);
    }

    public override String ToString()
    {
        if (IsModified)
            return $"{Property}: {Format(NewValue)} => {Format(OldValue)}";

        return $"{Property}: {Format(NewValue)}";
    }

    private String Format(Object? value)
    {
        return value switch
        {
            null => "null",
            DateTime date => $"\"{date:yyyy-MM-dd HH:mm:ss}\"",
            _ => JsonSerializer.Serialize(value, Options)
        };
    }
}
