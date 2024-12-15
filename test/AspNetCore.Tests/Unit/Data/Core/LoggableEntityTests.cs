using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AspNetCore.Objects;

namespace AspNetCore.Data;

[Collection("Database access")]
public class LoggableEntityTests : IDisposable
{
    private Role model;
    private DbContext context;
    private EntityEntry<AModel> entry;

    public LoggableEntityTests()
    {
        using (context = TestingContext.Create())
        {
            context.Drop().Add(model = ObjectsFactory.CreateRole(0));
            context.SaveChanges();
        }

        context = TestingContext.Create();
        entry = context.Entry<AModel>(model);
    }
    public void Dispose()
    {
        context.Dispose();
    }

    [Fact]
    public void LoggableEntity_SetsAction()
    {
        entry.State = EntityState.Deleted;

        Assert.Equal(nameof(EntityState.Deleted), new LoggableEntity(entry).Action);
    }

    [Fact]
    public void LoggableEntity_SetsName()
    {
        Assert.Equal(nameof(Role), new LoggableEntity(entry).Name);
    }

    [Fact]
    public void LoggableEntity_Proxy_SetsName()
    {
        model = context.Set<Role>().Single();
        entry = context.ChangeTracker.Entries<AModel>().Single();

        Assert.IsAssignableFrom<IProxyTargetAccessor>(model);
        Assert.Equal(nameof(Role), new LoggableEntity(entry).Name);
    }

    [Fact]
    public void LoggableEntity_SetsId()
    {
        Assert.Equal(model.Id, new LoggableEntity(entry).Id());
    }

    [Fact]
    public void LoggableEntity_Added_SetsIsModified()
    {
        entry.State = EntityState.Added;

        Assert.True(new LoggableEntity(entry).IsModified);
    }

    [Fact]
    public void LoggableEntity_Modified_SetsIsModified()
    {
        model.Title += "Test";
        entry.State = EntityState.Modified;

        Assert.True(new LoggableEntity(entry).IsModified);
    }

    [Fact]
    public void LoggableEntity_NotModified_SetsIsModified()
    {
        entry.State = EntityState.Modified;

        Assert.False(new LoggableEntity(entry).IsModified);
    }

    [Fact]
    public void LoggableEntity_Deleted_SetsIsModified()
    {
        entry.State = EntityState.Deleted;

        Assert.True(new LoggableEntity(entry).IsModified);
    }

    [Fact]
    public void ToString_Added_Changes()
    {
        entry.State = EntityState.Added;

        String expected = $"CreationDate: \"{model.CreationDate}\"\nTitle: \"{model.Title}\"\n";
        String actual = new LoggableEntity(entry).ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_Modified_Changes()
    {
        model.Title += "Test";
        entry.State = EntityState.Modified;

        String expected = $"Title: \"{model.Title[..^4]}\" => \"{model.Title}\"\n";
        String actual = new LoggableEntity(entry).ToString();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_Deleted_Changes()
    {
        entry.State = EntityState.Deleted;

        String expected = $"CreationDate: \"{model.CreationDate}\"\nTitle: \"{model.Title}\"\n";
        String actual = new LoggableEntity(entry).ToString();

        Assert.Equal(expected, actual);
    }
}
