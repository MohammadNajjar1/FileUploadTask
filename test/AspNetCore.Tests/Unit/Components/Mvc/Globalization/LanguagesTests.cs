namespace AspNetCore.Components.Mvc;

public class LanguagesTests
{
    private Languages languages;

    public LanguagesTests()
    {
        languages = new Languages("en", new[]
        {
            new Language
            {
                Name = "English GB",
                Abbreviation = "en",
                Culture = new CultureInfo("en-GB")
            },
            new Language
            {
                Name = "English US",
                Abbreviation = "us",
                Culture = new CultureInfo("en-US")
            }
        });
    }

    [Fact]
    public void Default_Language()
    {
        CultureInfo.CurrentUICulture = languages["us"].Culture!;

        Assert.Same(languages["en"], languages.Default);
    }

    [Fact]
    public void Current_GetsLanguage()
    {
        CultureInfo.CurrentUICulture = languages["en"].Culture!;

        Assert.Same(languages["en"], languages.Current);
    }

    [Fact]
    public void Current_SetsLanguage()
    {
        languages.Current = languages.Supported.Last();

        Assert.Same(languages.Supported.Last().Culture, CultureInfo.CurrentCulture);
        Assert.Same(languages.Supported.Last().Culture, CultureInfo.CurrentUICulture);
    }

    [Fact]
    public void Supported_Languages()
    {
        Language[] actual = languages.Supported;

        Assert.Equal(new CultureInfo("en-US"), actual[1].Culture);
        Assert.Equal(new CultureInfo("en-GB"), actual[0].Culture);
        Assert.Equal("us", actual[1].Abbreviation);
        Assert.Equal("en", actual[0].Abbreviation);
        Assert.Equal("English US", actual[1].Name);
        Assert.Equal("English GB", actual[0].Name);
    }

    [Fact]
    public void Indexer_ReturnsLanguage()
    {
        Language actual = languages["en"];

        Assert.Equal(new CultureInfo("en-GB"), actual.Culture);
        Assert.Equal("en", actual.Abbreviation);
        Assert.Equal("English GB", actual.Name);
    }
}
