namespace AspNetCore.Components.Mvc;

public class Languages : ILanguages
{
    public Language Default
    {
        get;
    }
    public Language Current
    {
        get
        {
            return Supported.Single(language => CultureInfo.CurrentUICulture.Equals(language.Culture));
        }
        set
        {
            CultureInfo.CurrentCulture = value.Culture ?? CultureInfo.CurrentCulture;
            CultureInfo.CurrentUICulture = value.Culture ?? CultureInfo.CurrentUICulture;
        }
    }
    public Language[] Supported
    {
        get;
    }

    private Dictionary<String, Language> Dictionary
    {
        get;
    }

    public Languages(String defaultAbbreviation, Language[] supported)
    {
        Dictionary = supported.ToDictionary(language => language.Abbreviation!);
        Default = Dictionary[defaultAbbreviation];
        Supported = supported;
    }

    public Language this[String abbreviation]
    {
        get
        {
            return Dictionary.TryGetValue(abbreviation, out Language? language) ? language : Default;
        }
    }
}
