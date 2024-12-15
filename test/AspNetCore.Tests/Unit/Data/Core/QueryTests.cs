using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Objects;

namespace AspNetCore.Data;

[Collection("Database access")]
public class QueryTests : IDisposable
{
    private DbContext context;
    private Query<Role> select;

    public QueryTests()
    {
        context = TestingContext.Create();
        select = new Query<Role>(context.Set<Role>(), TestingContext.Mapper.ConfigurationProvider);

        context.Drop().Add(ObjectsFactory.CreateRole(0));
        context.SaveChanges();
    }
    public void Dispose()
    {
        context.Dispose();
    }

    [Fact]
    public void ElementType_IsModelType()
    {
        Assert.Same(typeof(Role), select.ElementType);
    }

    [Fact]
    public void Expression_IsSetsExpression()
    {
        DbSet<Role> set = Substitute.For<DbSet<Role>, IQueryable>();
        DbContext testingContext = Substitute.For<DbContext>();
        ((IQueryable)set).Expression.Returns(Expression.Empty());
        testingContext.Set<Role>().Returns(set);

        select = new Query<Role>(testingContext.Set<Role>(), TestingContext.Mapper.ConfigurationProvider);

        Assert.Equal(((IQueryable)set).Expression, select.Expression);
    }

    [Fact]
    public void Provider_IsSetsProvider()
    {
        Assert.Equal(((IQueryable)context.Set<Role>()).Provider, select.Provider);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Where_Filters(Boolean predicate)
    {
        Assert.Equal(context.Set<Role>().Where(_ => predicate), select.Where(_ => predicate));
    }

    [Fact]
    public void To_ProjectsSet()
    {
        IEnumerable<Int64> expected = context.Set<Role>().ProjectTo<RoleView>(TestingContext.Mapper.ConfigurationProvider).Select(view => view.Id).ToArray();
        IEnumerable<Int64> actual = select.To<RoleView>().Select(view => view.Id).ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEnumerator_ReturnsSetEnumerator()
    {
        Assert.Equal(context.Set<Role>(), select.ToArray());
    }

    [Fact]
    public void GetEnumerator_ReturnsSameEnumerator()
    {
        Assert.Equal(context.Set<Role>(), select);
    }
}
