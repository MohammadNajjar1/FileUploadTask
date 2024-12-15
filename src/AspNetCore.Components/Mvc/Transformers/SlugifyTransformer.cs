using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Components.Mvc;

public class SlugifyTransformer : IOutboundParameterTransformer
{
    private static ConcurrentDictionary<Object, String?> Values { get; }

    static SlugifyTransformer()
    {
        Values = new ConcurrentDictionary<Object, String?>();
    }

    public String? TransformOutbound(Object? value)
    {
        if (value == null)
            return null;

        if (Values.TryGetValue(value, out String? slug))
            return slug;

        return Values[value] = value is String key ? Regex.Replace(key, "(?<!^)((?=[A-Z][a-z]+)|(?<![A-Z])(?=[A-Z][A-Z]+))", "-") : null;
    }
}
