using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using AspNetCore.Objects;

namespace AspNetCore.Data;

[Collection("Database access")]
public class UnitOfWorkTests : IDisposable
{
    private Permission model;
    private DbContext context;
    private UnitOfWork unitOfWork;

    public UnitOfWorkTests()
    {
        context = TestingContext.Create();
        model = ObjectsFactory.CreatePermission(0);
        unitOfWork = new UnitOfWork(context, TestingContext.Mapper);

        context.Drop();
    }
    public void Dispose()
    {
        unitOfWork.Dispose();
        context.Dispose();
    }

    [Fact]
    public void GetAs_Null_ReturnsDestinationDefault()
    {
        Assert.Null(unitOfWork.GetAs<Permission, PermissionView>(null));
    }

    [Fact]
    public void GetAs_ReturnsModelAsDestinationModelById()
    {
        context.Add(model);
        context.SaveChanges();

        PermissionView expected = TestingContext.Mapper.Map<PermissionView>(model);
        PermissionView actual = unitOfWork.GetAs<Permission, PermissionView>(model.Id)!;

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Controller, actual.Controller);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Area, actual.Area);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Get_Null_ReturnsNull()
    {
        Assert.Null(unitOfWork.Get<Permission>(null));
    }

    [Fact]
    public void Get_ModelById()
    {
        context.Add(model);
        context.SaveChanges();

        Permission expected = context.Set<Permission>().Single();
        Permission actual = unitOfWork.Get<Permission>(model.Id)!;

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Controller, actual.Controller);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Area, actual.Area);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Get_NotFound_ReturnsNull()
    {
        Assert.Null(unitOfWork.Get<Permission>(0));
    }

    [Fact]
    public void To_ConvertsSourceToDestination()
    {
        PermissionView expected = TestingContext.Mapper.Map<PermissionView>(model);
        PermissionView actual = unitOfWork.To<PermissionView>(model);

        Assert.Equal(expected.CreationDate, actual.CreationDate);
        Assert.Equal(expected.Controller, actual.Controller);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Area, actual.Area);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Select_FromSet()
    {
        context.Add(model);
        context.SaveChanges();

        Assert.Equal(context.Set<Permission>(), unitOfWork.Select<Permission>());
    }

    [Fact]
    public void InsertRange_AddsModelsToDbSet()
    {
        IEnumerable<Permission> permissions = new[] { ObjectsFactory.CreatePermission(2), ObjectsFactory.CreatePermission(3) };
        DbContext testingContext = Substitute.For<DbContext>();

        unitOfWork.Dispose();

        unitOfWork = new UnitOfWork(testingContext, TestingContext.Mapper);
        unitOfWork.InsertRange(permissions);

        foreach (Permission permission in permissions)
            testingContext.Received().Add(permission);
    }

    [Fact]
    public void Insert_AddsModelToDbSet()
    {
        unitOfWork.Insert(model);

        Assert.Equal(EntityState.Added, context.Entry(model).State);
        Assert.Same(model, context.ChangeTracker.Entries<Permission>().Single().Entity);
    }

    [Theory]
    [InlineData(EntityState.Added, EntityState.Added)]
    [InlineData(EntityState.Deleted, EntityState.Deleted)]
    [InlineData(EntityState.Detached, EntityState.Modified)]
    [InlineData(EntityState.Modified, EntityState.Modified)]
    [InlineData(EntityState.Unchanged, EntityState.Unchanged)]
    public void Update_Entry(EntityState initialState, EntityState state)
    {
        EntityEntry<Permission> entry = context.Entry(model);
        entry.State = initialState;

        unitOfWork.Update(model);

        Assert.Equal(state, entry.State);
        Assert.False(entry.Property(prop => prop.CreationDate).IsModified);
    }

    [Fact]
    public void DeleteRange_Models()
    {
        IEnumerable<Permission> models = new[] { ObjectsFactory.CreatePermission(2), ObjectsFactory.CreatePermission(3) };

        context.AddRange(models);
        context.SaveChanges();

        unitOfWork.DeleteRange(models);
        unitOfWork.Commit();

        Assert.Empty(context.Set<Permission>());
    }

    [Fact]
    public void Delete_Model()
    {
        context.Add(model);
        context.SaveChanges();

        unitOfWork.Delete(model);
        unitOfWork.Commit();

        Assert.Empty(context.Set<Permission>());
    }

    [Fact]
    public void Delete_ModelById()
    {
        context.Add(model);
        context.SaveChanges();

        unitOfWork.Delete<Permission>(model.Id);
        unitOfWork.Commit();

        Assert.Empty(context.Set<Permission>());
    }

    [Fact]
    public void Commit_SavesChanges()
    {
        using DbContext testingContext = Substitute.For<DbContext>();
        using UnitOfWork testingUnitOfWork = new(testingContext, TestingContext.Mapper);

        testingUnitOfWork.Commit();

        testingContext.Received().SaveChanges();
    }

    [Fact]
    public void Dispose_Context()
    {
        DbContext testingContext = Substitute.For<DbContext>();

        new UnitOfWork(testingContext, TestingContext.Mapper).Dispose();

        testingContext.Received().Dispose();
    }

    [Fact]
    public void Dispose_MultipleTimes()
    {
        unitOfWork.Dispose();
        unitOfWork.Dispose();
    }
}
