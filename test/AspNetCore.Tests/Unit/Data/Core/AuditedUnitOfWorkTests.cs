using Microsoft.EntityFrameworkCore;
using AspNetCore.Objects;

namespace AspNetCore.Data;

[Collection("Database access")]
public class AuditedUnitOfWorkTests : IDisposable
{
    private Permission model;
    private DbContext context;
    private AuditedUnitOfWork unitOfWork;

    public AuditedUnitOfWorkTests()
    {
        context = TestingContext.Create();
        model = ObjectsFactory.CreatePermission(0);
        unitOfWork = new AuditedUnitOfWork(context, TestingContext.Mapper, 1);

        context.Drop().Add(model);
        context.SaveChanges();
    }
    public void Dispose()
    {
        unitOfWork.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Commit_AddedAudit()
    {
        context.Dispose();
        unitOfWork.Dispose();
        context = TestingContext.Create();
        unitOfWork = new AuditedUnitOfWork(context, TestingContext.Mapper, 1);
        unitOfWork.Insert(ObjectsFactory.CreatePermission(1));

        LoggableEntity expected = new(context.ChangeTracker.Entries<AModel>().Single());

        unitOfWork.Commit();

        AuditLog actual = unitOfWork.Select<AuditLog>().Single();

        Assert.Equal(expected.ToString(), actual.Changes);
        Assert.Equal(expected.Name, actual.EntityName);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Id(), actual.EntityId);
        Assert.Equal(1, actual.AccountId);
    }

    [Fact]
    public void Commit_ModifiedAudit()
    {
        model.Action += "Test";

        unitOfWork.Update(model);

        LoggableEntity expected = new(context.ChangeTracker.Entries<AModel>().Single());

        unitOfWork.Commit();

        AuditLog actual = unitOfWork.Select<AuditLog>().Single();

        Assert.Equal(expected.ToString(), actual.Changes);
        Assert.Equal(expected.Name, actual.EntityName);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Id(), actual.EntityId);
        Assert.Equal(1, actual.AccountId);
    }

    [Fact]
    public void Commit_NoChanges_DoesNotAudit()
    {
        unitOfWork.Update(model);
        unitOfWork.Commit();

        Assert.Empty(unitOfWork.Select<AuditLog>());
    }

    [Fact]
    public void Commit_DeletedAudit()
    {
        unitOfWork.Delete(model);

        LoggableEntity expected = new(context.ChangeTracker.Entries<AModel>().Single());

        unitOfWork.Commit();

        AuditLog actual = unitOfWork.Select<AuditLog>().Single();

        Assert.Equal(expected.ToString(), actual.Changes);
        Assert.Equal(expected.Name, actual.EntityName);
        Assert.Equal(expected.Action, actual.Action);
        Assert.Equal(expected.Id(), actual.EntityId);
        Assert.Equal(1, actual.AccountId);
    }

    [Fact]
    public void Commit_UnsupportedState_DoesNotAudit()
    {
        IEnumerable<EntityState> unsupportedStates = Enum
            .GetValues(typeof(EntityState))
            .Cast<EntityState>()
            .Where(state => state is not EntityState.Added and not EntityState.Modified and not EntityState.Deleted);

        foreach (EntityState state in unsupportedStates)
        {
            context.Add(model).State = state;

            unitOfWork.Commit();

            Assert.Empty(unitOfWork.Select<AuditLog>());
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Commit_DoesNotChangeTrackingBehaviour(Boolean detectChanges)
    {
        context.ChangeTracker.AutoDetectChangesEnabled = detectChanges;

        unitOfWork.Insert(ObjectsFactory.CreatePermission(1));
        unitOfWork.Commit();

        Assert.Equal(detectChanges, context.ChangeTracker.AutoDetectChangesEnabled);
    }
}
