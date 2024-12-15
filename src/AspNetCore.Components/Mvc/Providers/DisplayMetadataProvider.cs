using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using AspNetCore.Resources;

namespace AspNetCore.Components.Mvc;

public class DisplayMetadataProvider : IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.MetadataKind == ModelMetadataKind.Property && context.Key.ContainerType is Type view)
        {
            context.DisplayMetadata.DisplayName = () => Resource.ForProperty(view, context.Key.Name!) is { Length: > 0 } title ? title : null;
        }
        else if (context.Key.MetadataKind == ModelMetadataKind.Type)
        {
            Type type = Nullable.GetUnderlyingType(context.Key.ModelType) ?? context.Key.ModelType;

            if (type.IsEnum)
            {
                context.DisplayMetadata.EnumGroupedDisplayNamesAndValues = Enum
                    .GetNames(type)
                    .Select(name =>
                    {
                        String group = Resource.Localized(type.Name, "Groups", name);
                        String title = Resource.Localized(type.Name, "Titles", name);
                        String value = (type.GetField(name)!.GetValue(null) as Enum)!.ToString("d");

                        return new KeyValuePair<EnumGroupAndName, String>(new EnumGroupAndName(group, title), value);
                    });
            }
        }
    }
}
