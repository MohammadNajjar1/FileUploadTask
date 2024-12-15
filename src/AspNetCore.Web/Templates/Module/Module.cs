using Genny;
using Humanizer;
using AspNetCore.Objects;
using System.Text.Encodings.Web;

namespace AspNetCore.Web.Templates;

[GennyModuleDescriptor("Default system module template")]
public class Module : GennyModule
{
    [GennyParameter(0, Required = true)]
    public String? Model { get; set; }

    [GennyParameter(1, Required = true)]
    public String? Controller { get; set; }

    [GennyParameter(2, Required = false)]
    public String? Area { get; set; }

    [GennySwitch("force", "f")]
    public Boolean Force { get; set; }

    public Module(IServiceProvider services)
        : base(services)
    {
    }

    public override void Run()
    {
        String path = $"{Area}/{Controller}".Trim('/');
        String shortPath = Area == null ? "" : $"{Area}/";
        ModuleModel model = new(Model!, Controller!, Area);

        Dictionary<String, GennyScaffoldingResult> results = new()
        {
            { $"../AspNetCore.Services/{shortPath}{Model}Service.cs", Scaffold("Services/Service") },
            { $"../AspNetCore.Validators/{shortPath}{Model}Validator.cs", Scaffold("Validators/Validator") },
            { $"../AspNetCore.Controllers/{shortPath}{Controller}.cs", Scaffold("Controllers/Controller") },
            { $"../../test/AspNetCore.Tests/Integration/Controllers/{shortPath}{Controller}Tests.cs", Scaffold("Tests/ControllerTests") },

            { $"../AspNetCore.Web/Views/{path}/Index.cshtml", Scaffold("Web/Index") },
            { $"../AspNetCore.Web/Views/{path}/Create.cshtml", Scaffold("Web/Create") },
            { $"../AspNetCore.Web/Views/{path}/Details.cshtml", Scaffold("Web/Details") },
            { $"../AspNetCore.Web/Views/{path}/Edit.cshtml", Scaffold("Web/Edit") },
            { $"../AspNetCore.Web/Views/{path}/Delete.cshtml", Scaffold("Web/Delete") }
        };

        foreach (String view in model.ViewProperties.Keys.Where(key => key.Length == 0 || model.Views[key] != model.Views[""]))
            results[$"../AspNetCore.Web/Resources/Views/{path}/{Model}{view}View.json"] = Scaffold($"Resources/{view}View");

        foreach (Type type in model.EnumTypes)
            results[$"../AspNetCore.Web/Resources/Enums/{path}/{type.Name}.json"] = Scaffold("Resources/Enum", Enum.GetNames(type));

        if (!File.Exists($"../AspNetCore.Objects/Models/{path}/{Model}.cs") ||
            !File.Exists($"../AspNetCore.Objects/Views/{path}/{Model}View.cs"))
            results = new Dictionary<String, GennyScaffoldingResult>
            {
                { $"../AspNetCore.Objects/Models/{path}/{Model}.cs", Scaffold("Objects/Model") },
                { $"../AspNetCore.Objects/Views/{path}/{Model}View.cs", Scaffold("Objects/View") }
            };

        if (results.Any(result => result.Value.Errors.Any()))
        {
            Dictionary<String, GennyScaffoldingResult> errors = new(results.Where(x => x.Value.Errors.Any()));

            Write(errors);

            Logger.WriteLine("");
            Logger.WriteLine("Scaffolding failed! Rolling back...", ConsoleColor.Red);
        }
        else
        {
            Logger.WriteLine("");

            if (Force)
                Write(results);
            else
                TryWrite(results);

            if (results.Count > 2)
            {
                AddArea();
                AddSiteMap();
                AddPermissions();
                AddViewImports();
                AddObjectFactory();
                AddConfigurationTests();
                AddTestingContextDrops();

                AddResource("Page", "Headers", Model!, Model.Humanize());
                AddResource("Page", "Headers", Model.Pluralize(), Model.Pluralize().Humanize());

                AddResource("Page", "Titles", $"{Area}/{Controller}/Create".Trim('/'), $"{Model.Humanize()} creation");
                AddResource("Page", "Titles", $"{Area}/{Controller}/Delete".Trim('/'), $"{Model.Humanize()} deletion");
                AddResource("Page", "Titles", $"{Area}/{Controller}/Details".Trim('/'), $"{Model.Humanize()} details");
                AddResource("Page", "Titles", $"{Area}/{Controller}/Index".Trim('/'), Model.Pluralize().Humanize());
                AddResource("Page", "Titles", $"{Area}/{Controller}/Edit".Trim('/'), $"{Model.Humanize()} edit");

                if (Area != null)
                    AddResource("Shared", "Areas", Area, Area.Humanize());

                AddResource("Shared", "Controllers", $"{Area}/{Controller}".Trim('/'), Model.Pluralize().Humanize());

                if (Area != null)
                    AddResource("SiteMap", "Titles", Area, Area.Humanize());

                AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Create".Trim('/'), "Create");
                AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Delete".Trim('/'), "Delete");
                AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Details".Trim('/'), "Details");
                AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Index".Trim('/'), Model.Pluralize().Humanize());
                AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Edit".Trim('/'), "Edit");

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolded successfully!", ConsoleColor.Green);
            }
            else
            {
                Logger.WriteLine("");
                Logger.WriteLine("Scaffolded successfully! Write in model and view properties and rerun the scaffolding.", ConsoleColor.Green);
            }
        }
    }

    public override void ShowHelp()
    {
        Logger.WriteLine("Parameters:");
        Logger.WriteLine("    0 - Scaffolded model.");
        Logger.WriteLine("    1 - Scaffolded controller.");
        Logger.WriteLine("    2 - Scaffolded area (optional).");
    }

    private void AddArea()
    {
        if (Area == null)
            return;

        Logger.Write("../AspNetCore.Controllers/Area.cs - ");

        String content = File.ReadAllText("../AspNetCore.Controllers/Area.cs");
        String[] areas = Regex.Matches(content, @"    (?<area>\w+),?").Select(match => match.Groups["area"].Value).ToArray();
        String[] newAreas = areas.Append(Area).Distinct().OrderBy(name => name).ToArray();

        if (areas.Length == newAreas.Length)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            content = Regex.Replace(content,
                @"{\r?\n( +\w+,?\r?\n)+}",
                $"{{\n    {String.Join(",\n    ", newAreas)}\n}}");

            File.WriteAllText("../AspNetCore.Controllers/Area.cs", content);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddSiteMap()
    {
        Logger.Write("../AspNetCore.Web/mvc.sitemap - ");

        XElement sitemap = XElement.Parse(File.ReadAllText("mvc.sitemap"));
        Boolean isDefined = sitemap
            .Descendants("siteMapNode")
            .Any(node =>
                node.Attribute("action")?.Value == "Index" &&
                node.Parent?.Attribute("area")?.Value == Area &&
                node.Attribute("controller")?.Value == Controller);

        if (isDefined)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            XElement? parent = sitemap
                .Descendants("siteMapNode")
                .FirstOrDefault(node =>
                    node.Attribute("action") == null &&
                    node.Attribute("controller") == null &&
                    node.Attribute("area")?.Value == Area);

            if (parent == null)
            {
                if (Area == null)
                {
                    parent = sitemap.Descendants("siteMapNode").First();
                }
                else
                {
                    parent = XElement.Parse($@"<siteMapNode menu=""true"" icon=""far fa-folder"" area=""{Area}"" />");
                    sitemap.Descendants("siteMapNode").First().Add(parent);
                }
            }

            parent.Add(XElement.Parse(
                $@"<siteMapNode menu=""true"" icon=""far fa-folder"" controller=""{Controller}"" action=""Index"">
                    <siteMapNode icon=""far fa-file"" action=""Create"" />
                    <siteMapNode icon=""fa fa-info"" action=""Details"" />
                    <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                    <siteMapNode icon=""fa fa-times"" action=""Delete"" />
                </siteMapNode>"
            ));

            File.WriteAllText("mvc.sitemap", $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n{sitemap.ToString().Replace("  ", "    ")}\n");

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddPermissions()
    {
        Logger.Write("../AspNetCore.Data/Migrations/Configuration.cs - ");

        String content = File.ReadAllText("../AspNetCore.Data/Migrations/Configuration.cs");
        String[] permissions = Regex.Matches(content, "new\\(\\) {[^}]+}").Select(match => $"            {match.Value}").ToArray();
        String[] newPermissions = permissions
            .Append($@"            new() {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Create"" }}")
            .Append($@"            new() {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Delete"" }}")
            .Append($@"            new() {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Edit"" }}")
            .Append($@"            new() {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Index"" }}")
            .Distinct()
            .OrderBy(permission => permission)
            .ToArray();

        if (permissions.Length == newPermissions.Length)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            content = Regex.Replace(content, @"( +new\(\) {[^}]+},?\r?\n+)+", $"{String.Join(",\n", newPermissions)}\n");

            File.WriteAllText("../AspNetCore.Data/Migrations/Configuration.cs", content);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddViewImports()
    {
        Logger.Write("../AspNetCore.Web/Views/_ViewImports.cshtml - ");

        String content = File.ReadAllText("../AspNetCore.Web/Views/_ViewImports.cshtml");
        String[] imports = Regex.Matches(content, "@using (.+);").Select(match => match.Value).ToArray();
        String[] newImports = imports
            .Append(Area == null ? "@using AspNetCore.Controllers;" : $"@using AspNetCore.Controllers.{Area};")
            .Distinct()
            .OrderBy(definition => definition.TrimEnd(';'))
            .ToArray();

        if (imports.Length == newImports.Length)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            content = Regex.Replace(content, @"(@using (.+);\r?\n)+", $"{String.Join("\n", newImports)}\n");

            File.WriteAllText("../AspNetCore.Web/Views/_ViewImports.cshtml", content);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddObjectFactory()
    {
        Logger.Write("../../test/AspNetCore.Tests/Helpers/ObjectsFactory.cs - ");

        String content = File.ReadAllText("../../test/AspNetCore.Tests/Helpers/ObjectsFactory.cs");
        ModuleModel model = new(Model!, Controller!, Area);
        String newContent = content;

        if (!content.Contains($" Create{Model}("))
        {
            String fakeModel = FakeObjectCreation(model.Model, model.Properties);

            newContent = Regex.Replace(newContent, "(?=\r?\n})", fakeModel);
        }

        foreach (String view in model.ViewProperties.Keys)
        {
            if (content.Contains($" Create{Model}{view}View("))
                continue;

            if (view != "" && model.Views[""] == model.Views[view])
                continue;

            String fakeView = FakeObjectCreation($"{Model}{view}View", model.AllViewProperties[view]);

            newContent = Regex.Replace(newContent, "(?=\r?\n})", fakeView);
        }

        if (newContent == content)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            MatchCollection matches = Regex.Matches(newContent, @"    public static (?<class>\w+) .*(.*\r?\n)+?        }");
            String[] methods = matches.OrderBy(match => match.Groups["class"].Value).Select(match => match.Value).ToArray();

            newContent = Regex.Replace(newContent,
                @"    public static class ObjectsFactory.*\r?\n    {\r?\n(.*\r?\n)+?    }",
                $"    public static class ObjectsFactory\n    {{\n{String.Join("\n\n", methods)}\n    }}");

            File.WriteAllText("../../test/AspNetCore.Tests/Helpers/ObjectsFactory.cs", newContent);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddConfigurationTests()
    {
        Logger.Write("../../test/AspNetCore.Tests/Unit/Data/Migrations/ConfigurationTests.cs - ");

        String content = File.ReadAllText("../../test/AspNetCore.Tests/Unit/Data/Migrations/ConfigurationTests.cs");
        String[] tests = Regex.Matches(content, @"\[InlineData\(.*, ""\w+"", ""\w+""\)\]").Select(match => $"    {match.Value}").ToArray();
        String[] newTests = tests
            .Append($@"    [InlineData(""{Area}"", ""{Controller}"", ""Create"")]")
            .Append($@"    [InlineData(""{Area}"", ""{Controller}"", ""Delete"")]")
            .Append($@"    [InlineData(""{Area}"", ""{Controller}"", ""Edit"")]")
            .Append($@"    [InlineData(""{Area}"", ""{Controller}"", ""Index"")]")
            .Distinct()
            .OrderBy(test => test)
            .ToArray();

        if (tests.Length == newTests.Length)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            content = Regex.Replace(content, @"( +\[InlineData\(.*, ""\w+"", ""\w+""\)\]\r?\n)+", $"{String.Join("\n", newTests)}\n");

            File.WriteAllText("../../test/AspNetCore.Tests/Unit/Data/Migrations/ConfigurationTests.cs", content);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddTestingContextDrops()
    {
        Logger.Write("../../test/AspNetCore.Tests/Helpers/TestingContext.cs - ");

        String content = File.ReadAllText("../../test/AspNetCore.Tests/Helpers/TestingContext.cs");
        String[] drops = Regex.Matches(content, @"context.RemoveRange\(context.Set<(.+)>\(\)\);").Select(match => $"        {match.Value}").ToArray();
        String[] newDrops = drops
            .Append($"        context.RemoveRange(context.Set<{Model}>());")
            .Distinct()
            .OrderByDescending(drop => drop.Length)
            .ThenByDescending(drop => drop)
            .ToArray();

        if (drops.Length == newDrops.Length)
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            content = Regex.Replace(content, @"( +context.RemoveRange\(context.Set<(.+)>\(\)\);\r?\n)+", $"{String.Join("\n", newDrops)}\n");

            File.WriteAllText("../../test/AspNetCore.Tests/Helpers/TestingContext.cs", content);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }
    private void AddResource(String resource, String group, String key, String value)
    {
        Logger.Write($"../AspNetCore.Web/Resources/Shared/{resource}.json - ");

        String page = File.ReadAllText($"Resources/Shared/{resource}.json");
        Dictionary<String, SortedDictionary<String, String>> resources = JsonSerializer.Deserialize<Dictionary<String, SortedDictionary<String, String>>>(page)!;

        if (resources[group].ContainsKey(key))
        {
            Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
        }
        else
        {
            resources[group][key] = value;

            String text = Regex.Replace(JsonSerializer.Serialize(resources, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }), "(^ +)", "$1$1", RegexOptions.Multiline);

            File.WriteAllText($"Resources/Shared/{resource}.json", $"{text}\n");

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
    }

    private GennyScaffoldingResult Scaffold(String path, Object? model = null)
    {
        return Scaffolder.Scaffold($"Templates/Module/{path}", model ?? new ModuleModel(Model!, Controller!, Area));
    }
    private String FakeObjectCreation(String name, PropertyInfo[] properties)
    {
        String creation = $"\n\n    public static {name} Create{name}(Int64 id)\n";
        creation += "    {\n";
        creation += "        return new()\n";
        creation += "        {\n";

        creation += String.Join(",\n", properties
            .Where(property => property.Name != nameof(AModel.CreationDate))
            .OrderBy(property => property.Name.Length)
            .Select(property =>
            {
                Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                String set = $"            {property.Name} = ";

                if (property.PropertyType == typeof(String))
                    return $"{set}$\"{property.Name}{{id}}\"";

                if (type == typeof(Boolean))
                    return $"{set}true";

                if (type == typeof(DateTime))
                    return $"{set}DateTime.Now.AddDays(id)";

                if (type.IsEnum)
                    return $"{set}{type.Name}.{Enum.GetNames(type)[0]}";

                return $"{set}id";
            })) +
            "\n";

        creation += "        };\n";
        creation += "    }";

        return creation;
    }
}
