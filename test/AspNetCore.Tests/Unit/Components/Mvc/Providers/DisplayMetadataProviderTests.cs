using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using AspNetCore.Objects;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class DisplayMetadataProviderTests
{
    [Fact]
    public void CreateDisplayMetadata_NullContainerType_DoesNotSetDisplayName()
    {
        DisplayMetadataProvider provider = new();
        DisplayMetadataProviderContext context = new(
            ModelMetadataIdentity.ForType(typeof(RoleView)),
            ModelAttributes.GetAttributesForType(typeof(RoleView)));

        provider.CreateDisplayMetadata(context);

        Assert.Null(context.DisplayMetadata.DisplayName);
    }

    [Fact]
    public void CreateDisplayMetadata_DisplayName_EmptyIsNull()
    {
        String language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        PropertyInfo property = typeof(AllTypesView).GetProperty(nameof(AllTypesView.StringField))!;
        Resource.Set(nameof(AllTypesView))[language, "Titles", nameof(AllTypesView.StringField)] = "";
        DisplayMetadataProviderContext context = new(
            ModelMetadataIdentity.ForProperty(property, typeof(String), typeof(AllTypesView)),
            ModelAttributes.GetAttributesForType(typeof(AllTypesView)));

        new DisplayMetadataProvider().CreateDisplayMetadata(context);

        Assert.Null(context.DisplayMetadata.DisplayName!.Invoke());
    }

    [Fact]
    public void CreateDisplayMetadata_DisplayName_NoCache()
    {
        String language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        PropertyInfo property = typeof(AllTypesView).GetProperty(nameof(AllTypesView.StringField))!;
        Resource.Set(nameof(AllTypesView))[language, "Titles", nameof(AllTypesView.StringField)] = "Test";
        DisplayMetadataProviderContext context = new(
            ModelMetadataIdentity.ForProperty(property, typeof(String), typeof(AllTypesView)),
            ModelAttributes.GetAttributesForType(typeof(AllTypesView)));

        new DisplayMetadataProvider().CreateDisplayMetadata(context);

        Assert.Equal("Test", context.DisplayMetadata.DisplayName?.Invoke());

        Resource.Set(nameof(AllTypesView))[language, "Titles", nameof(AllTypesView.StringField)] = "Testing";

        Assert.Equal("Testing", context.DisplayMetadata.DisplayName?.Invoke());
    }
}
