using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;

namespace AspNetCore.Validators;

public class AValidatorTests : IDisposable
{
    private AValidatorProxy validator;
    private IUnitOfWork unitOfWork;

    public AValidatorTests()
    {
        unitOfWork = Substitute.For<IUnitOfWork>();
        validator = new AValidatorProxy(unitOfWork);
    }
    public void Dispose()
    {
        unitOfWork.Dispose();
        validator.Dispose();
    }

    [Fact]
    public void AValidator_CreatesEmptyModelState()
    {
        Assert.Empty(validator.ModelState);
    }

    [Fact]
    public void AValidator_CreatesEmptyAlerts()
    {
        Assert.Empty(validator.Alerts);
    }

    [Fact]
    public void IsSpecified_Null_ReturnsFalse()
    {
        RoleView view = new();

        Boolean isSpecified = validator.BaseIsSpecified(view, role => role.Title);
        String message = Validation.For("Required", Resource.ForProperty<RoleView, String?>(role => role.Title));

        Assert.False(isSpecified);
        Assert.Empty(validator.Alerts);
        Assert.Single(validator.ModelState);
        Assert.Equal(message, validator.ModelState[nameof(RoleView.Title)]!.Errors.Single().ErrorMessage);
    }

    [Fact]
    public void IsSpecified_NullValue_ReturnsFalse()
    {
        AccountEditView view = new();

        Boolean isSpecified = validator.BaseIsSpecified(view, account => account.RoleId);
        String message = Validation.For("Required", Resource.ForProperty<AccountEditView, Int64?>(account => account.RoleId));

        Assert.False(isSpecified);
        Assert.Empty(validator.Alerts);
        Assert.Single(validator.ModelState);
        Assert.Equal(message, validator.ModelState[nameof(AccountEditView.RoleId)]!.Errors.Single().ErrorMessage);
    }

    [Fact]
    public void IsSpecified_Valid()
    {
        Assert.True(validator.BaseIsSpecified(ObjectsFactory.CreateRoleView(0), role => role.Id));
        Assert.Empty(validator.ModelState);
        Assert.Empty(validator.Alerts);
    }

    [Fact]
    public void Dispose_UnitOfWork()
    {
        validator.Dispose();

        unitOfWork.Received().Dispose();
    }

    [Fact]
    public void Dispose_MultipleTimes()
    {
        validator.Dispose();
        validator.Dispose();
    }
}
