using Microsoft.EntityFrameworkCore;
using AspNetCore.Components.Mvc;
using AspNetCore.Controllers;
using AspNetCore.Data.Migrations;
using AspNetCore.Objects;

namespace AspNetCore.Resources;

[Collection("Database access")]
public class ResourcesTests
{
    [Fact]
    public void Resources_HasAllPageTitles()
    {
        XElement sitemap = XDocument.Load("../../../../src/AspNetCore.Web/mvc.sitemap").Element("siteMap")!;

        foreach (SiteMapNode node in Flatten(sitemap.Elements()).Where(node => node.Action != null))
        {
            String path = $"{node.Area}/{node.Controller}/{node.Action}".Trim('/');

            Assert.True(Resource.ForPage(path) != "", $"'{path}' page does not have a title.");
        }
    }

    [Fact]
    public void Resources_HasAllSiteMapTitles()
    {
        XElement sitemap = XDocument.Load("../../../../src/AspNetCore.Web/mvc.sitemap").Element("siteMap")!;

        foreach (String path in Flatten(sitemap.Elements()).Select(node => $"{node.Area}/{node.Controller}/{node.Action}".Trim('/')))
            Assert.True(Resource.ForSiteMap(path) != "", $"'{path}' page does not have a sitemap title.");
    }

    [Fact]
    public void Resources_HasAllPermissionAreaTitles()
    {
        using DbContext context = TestingContext.Create();
        using Configuration configuration = new(context, TestingContext.Mapper);

        configuration.Seed();

        foreach (String area in context.Set<Permission>().Select(permission => permission.Area).Where(area => area != "").Distinct())
            Assert.True(Resource.ForArea(area) != "", $"'{area}' area does not have a title.");
    }

    [Fact]
    public void Resources_HasAllPermissionControllerTitles()
    {
        using DbContext context = TestingContext.Create();
        using Configuration configuration = new(context, TestingContext.Mapper);

        configuration.Seed();

        foreach (String path in context.Set<Permission>().Select(permission => $"{permission.Area}/{permission.Controller}".Trim('/')).Distinct())
            Assert.True(Resource.ForController(path) != "", $"'{path}' permission does not have a title.");
    }

    [Fact]
    public void Resources_HasAllPermissionActionTitles()
    {
        using DbContext context = TestingContext.Create();
        using Configuration configuration = new(context, TestingContext.Mapper);

        configuration.Seed();

        foreach (String name in context.Set<Permission>().Select(permission => permission.Action).Distinct())
            Assert.True(Resource.ForAction(name) != "", $"'{name}' action does not have a title.");
    }

    [Fact]
    public void Resources_HasAllLookupTitles()
    {
        BindingFlags actions = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        foreach (String handler in typeof(Lookup).GetMethods(actions).Select(action => action.Name))
            Assert.True(Resource.ForLookup(handler) != "", $"'{handler}' lookup does not have a title.");
    }

    private List<SiteMapNode> Flatten(IEnumerable<XElement> elements, SiteMapNode? parent = null)
    {
        List<SiteMapNode> list = new();

        foreach (XElement element in elements)
        {
            SiteMapNode node = new()
            {
                Action = (String?)element.Attribute("action"),
                Area = (String?)element.Attribute("area") ?? parent?.Area,
                Controller = (String?)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null)
            };

            list.Add(node);
            list.AddRange(Flatten(element.Elements(), node));
        }

        return list;
    }
}
