namespace AspNetCore.Components.System;

public static class Constant
{
    private static ConcurrentDictionary<String, Object> Values { get; }

    static Constant()
    {
        Values = new ConcurrentDictionary<String, Object>();
    }

    public static void Set(String key, Object value)
    {
        Values[key] = value;
    }

    private static T Get<T>(String key)
    {
        return (T)Values[key];
    }
}
