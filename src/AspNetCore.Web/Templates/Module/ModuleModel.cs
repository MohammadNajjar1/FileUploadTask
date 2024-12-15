using Humanizer;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Objects;

namespace AspNetCore.Web.Templates;

public class ModuleModel
{
    public String Model { get; }
    public String? Area { get; }
    public String Models { get; }
    public String VarName { get; }
    public String ShortName { get; }
    public String Controller { get; }
    public String ControllerNamespace { get; }

    public Type[] EnumTypes { get; set; }
    public PropertyInfo[] Properties { get; set; }
    public PropertyInfo[] AllProperties { get; set; }
    public Dictionary<String, String> Views { get; set; }
    public Dictionary<String, String?> Relations { get; set; }
    public Dictionary<String, PropertyInfo[]> Indexes { get; set; }
    public Dictionary<String, PropertyInfo[]> ViewProperties { get; set; }
    public Dictionary<String, PropertyInfo[]> AllViewProperties { get; set; }

    public ModuleModel(String model, String controller, String? area)
    {
        Views = new Dictionary<String, String>();
        Assembly assembly = typeof(AView).Assembly;
        Indexes = new Dictionary<String, PropertyInfo[]>();
        ViewProperties = new Dictionary<String, PropertyInfo[]>();
        AllViewProperties = new Dictionary<String, PropertyInfo[]>();
        Type modelType = assembly.GetType($"AspNetCore.Objects.{model}") ?? typeof(AModel);
        Type viewType = assembly.GetType($"AspNetCore.Objects.{model}View") ?? typeof(AView<>);
        BindingFlags declaredOnly = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        ControllerNamespace = $"AspNetCore.Controllers.{area}".Trim('.');
        ShortName = Regex.Split(model, "(?=[A-Z])").Last();
        VarName = ShortName.ToLower();
        Models = model.Pluralize();
        Controller = controller;
        Model = model;
        Area = area;

        PropertyInfo[] modelProperties = modelType.GetProperties();
        PropertyInfo[] indexes = modelType
            .GetCustomAttributes<IndexAttribute>()
            .Where(index => index.IsUnique)
            .Select(index => modelType.GetProperty(index.PropertyNames[0])!)
            .OrderByDescending(property => property.Name.Length)
            .ThenByDescending(property => property.Name)
            .ToArray();

        foreach (String view in new[] { "", "Index", "Create", "Details", "Edit", "Delete" })
        {
            Type type = assembly.GetType($"AspNetCore.Objects.{model}{view}View") ?? viewType;

            Views[view] = type.Name;
            AllViewProperties[view] = type.GetProperties();
            ViewProperties[view] = type.GetProperties(declaredOnly);
            Indexes[view] = indexes.Where(index => ViewProperties[view].Any(property => property.Name == index.Name)).ToArray();
        }

        AllProperties = modelProperties.Where(property =>
            (Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType).IsEnum ||
            property.PropertyType.Namespace == "System").ToArray();
        Properties = modelType.GetProperties(declaredOnly);
        EnumTypes = AllProperties
            .Select(property => Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType)
            .Where(type => type.IsEnum)
            .ToArray();
        Relations = modelProperties
            .ToDictionary(
                property => property.Name,
                property => modelProperties.SingleOrDefault(relation =>
                    property.Name.EndsWith("Id") &&
                    relation.PropertyType.Assembly == modelType.Assembly &&
                    relation.Name == property.Name[..^2])?.PropertyType.Name);
    }
}
