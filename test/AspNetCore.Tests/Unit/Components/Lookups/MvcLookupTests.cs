using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;

namespace AspNetCore.Components.Lookups;

public class MvcLookupTests : IDisposable
{
    private IUnitOfWork unitOfWork;
    private MvcLookup<Role, RoleView> lookup;

    public MvcLookupTests()
    {
        unitOfWork = Substitute.For<IUnitOfWork>();
        lookup = new MvcLookup<Role, RoleView>(unitOfWork);
    }
    public void Dispose()
    {
        unitOfWork.Dispose();
    }

    [Fact]
    public void GetColumnHeader_ReturnsPropertyTitle()
    {
        String actual = lookup.GetColumnHeader(typeof(RoleView).GetProperty(nameof(RoleView.Title))!);
        String expected = Resource.ForProperty(typeof(RoleView), nameof(RoleView.Title));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetColumnHeader_ReturnsRelationPropertyTitle()
    {
        PropertyInfo property = typeof(AllTypesView).GetProperty(nameof(AllTypesView.Child))!;

        Assert.Empty(lookup.GetColumnHeader(property));
    }

    [Theory]
    [InlineData(nameof(AllTypesView.EnumField), "text-start")]
    [InlineData(nameof(AllTypesView.SByteField), "text-end")]
    [InlineData(nameof(AllTypesView.ByteField), "text-end")]
    [InlineData(nameof(AllTypesView.Int16Field), "text-end")]
    [InlineData(nameof(AllTypesView.UInt16Field), "text-end")]
    [InlineData(nameof(AllTypesView.Int32Field), "text-end")]
    [InlineData(nameof(AllTypesView.UInt32Field), "text-end")]
    [InlineData(nameof(AllTypesView.Int64Field), "text-end")]
    [InlineData(nameof(AllTypesView.UInt64Field), "text-end")]
    [InlineData(nameof(AllTypesView.SingleField), "text-end")]
    [InlineData(nameof(AllTypesView.DoubleField), "text-end")]
    [InlineData(nameof(AllTypesView.DecimalField), "text-end")]
    [InlineData(nameof(AllTypesView.BooleanField), "text-start")]
    [InlineData(nameof(AllTypesView.DateTimeField), "text-start")]

    [InlineData(nameof(AllTypesView.NullableEnumField), "text-start")]
    [InlineData(nameof(AllTypesView.NullableSByteField), "text-end")]
    [InlineData(nameof(AllTypesView.NullableByteField), "text-end")]
    [InlineData(nameof(AllTypesView.NullableInt16Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableUInt16Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableInt32Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableUInt32Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableInt64Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableUInt64Field), "text-end")]
    [InlineData(nameof(AllTypesView.NullableSingleField), "text-end")]
    [InlineData(nameof(AllTypesView.NullableDoubleField), "text-end")]
    [InlineData(nameof(AllTypesView.NullableDecimalField), "text-end")]
    [InlineData(nameof(AllTypesView.NullableBooleanField), "text-start")]
    [InlineData(nameof(AllTypesView.NullableDateTimeField), "text-start")]

    [InlineData(nameof(AllTypesView.StringField), "text-start")]
    [InlineData(nameof(AllTypesView.Child), "text-start")]
    public void GetColumnCssClass_ReturnsCssClassForPropertyType(String name, String classes)
    {
        PropertyInfo property = typeof(AllTypesView).GetProperty(name)!;

        Assert.Equal(classes, lookup.GetColumnCssClass(property));
    }

    [Fact]
    public void GetModels_FromUnitOfWork()
    {
        unitOfWork.Select<Role>().To<RoleView>().Returns(Array.Empty<RoleView>().AsQueryable());

        Object actual = new MvcLookup<Role, RoleView>(unitOfWork).GetModels();
        Object expected = unitOfWork.Select<Role>().To<RoleView>();

        Assert.Same(expected, actual);
    }
}
