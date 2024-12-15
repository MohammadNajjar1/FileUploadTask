using AspNetCore.Data;
using NonFactors.Mvc.Lookup;

namespace AspNetCore.Controllers;

[Collection("Database access")]
public class LookupTests : IDisposable
{
    private Lookup controller;
    private IUnitOfWork unitOfWork;

    public LookupTests()
    {
        unitOfWork = new UnitOfWork(TestingContext.Create(), TestingContext.Mapper);
        controller = new Lookup(unitOfWork);
    }
    public void Dispose()
    {
        controller.Dispose();
        unitOfWork.Dispose();
    }

    [Fact]
    public void Role_Lookup()
    {
        LookupData actual = Assert.IsType<LookupData>(controller.Role(new LookupFilter()).Value);

        Assert.NotEmpty(actual.Columns);
    }

    [Fact]
    public void Dispose_UnitOfWork()
    {
        IUnitOfWork dbUnit = Substitute.For<IUnitOfWork>();

        new Lookup(dbUnit).Dispose();

        dbUnit.Received().Dispose();
    }

    [Fact]
    public void Dispose_MultipleTimes()
    {
        IUnitOfWork dbUnit = Substitute.For<IUnitOfWork>();

        new Lookup(dbUnit).Dispose();

        controller.Dispose();
        controller.Dispose();
    }
}
