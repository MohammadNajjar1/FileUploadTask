using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Components.Security;
using AspNetCore.Resources;
using NonFactors.Mvc.Grid;
using System.Text.Encodings.Web;

namespace AspNetCore.Components.Extensions;

public class MvcGridExtensionsTests
{
    private IGridColumnsOf<AllTypesView> columns;
    private IHtmlGrid<AllTypesView> html;
    private ViewContext context;

    public MvcGridExtensionsTests()
    {
        Grid<AllTypesView> grid = new(Array.Empty<AllTypesView>());
        IHtmlHelper helper = HttpFactory.CreateHtmlHelper();
        html = new HtmlGrid<AllTypesView>(helper, grid);
        columns = new GridColumns<AllTypesView>(grid);
        context = html.Grid.ViewContext!;
    }

    [Fact]
    public void AddAction_Unauthorized_Empty()
    {
        IGridColumn<AllTypesView, IHtmlContent> actual = columns.AddAction("Edit", "fa fa-pencil-alt");

        Assert.Empty(actual.ValueFor(new GridRow<AllTypesView>(new AllTypesView(), 0)).ToString());
        Assert.Empty(columns);
    }

    [Fact]
    public void AddAction_Authorized_Renders()
    {
        StringWriter writer = new();
        context.RouteData.Values["area"] = "Testing";
        context.RouteData.Values["controller"] = "Tests";
        IUrlHelper url = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(context);
        IAuthorization authorization = html.Grid.ViewContext!.HttpContext.RequestServices.GetRequiredService<IAuthorization>();

        url.Action(Arg.Any<UrlActionContext>()).Returns("/test");
        authorization.IsGrantedFor(Arg.Any<Int64>(), "Testing/Tests/Details").Returns(true);

        IGridColumn<AllTypesView, IHtmlContent> column = columns.AddAction("Details", "fa fa-info");
        column.ValueFor(new GridRow<AllTypesView>(new AllTypesView(), 0)).WriteTo(writer, HtmlEncoder.Default);

        String expected = $"<a class=\"fa fa-info\" href=\"/test\" title=\"{Resource.ForAction("Details")}\"></a>";
        String actual = writer.ToString();

        Assert.Equal("action-cell details", column.CssClasses);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddAction_NoId_Throws()
    {
        IAuthorization authorization = html.Grid.ViewContext!.HttpContext.RequestServices.GetRequiredService<IAuthorization>();
        IGridColumnsOf<Object> gridColumns = new GridColumns<Object>(new Grid<Object>(Array.Empty<Object>()));
        authorization.IsGrantedFor(Arg.Any<Int64>(), Arg.Any<String>()).Returns(true);
        gridColumns.Grid.ViewContext = html.Grid.ViewContext;

        IGridColumn<Object, IHtmlContent> column = gridColumns.AddAction("Delete", "fa fa-times");

        String actual = Assert.Throws<Exception>(() => column.ValueFor(new GridRow<Object>(new Object(), 0))).Message;
        String expected = "Object type does not have an id.";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddDate_Column()
    {
        Expression<Func<AllTypesView, DateTime>> expression = model => model.DateTimeField;

        IGridColumn<AllTypesView, DateTime> actual = columns.AddDate(expression);

        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal("date-time-field", actual.Name);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Equal("{0:d}", actual.Format);
        Assert.Single(columns);
    }

    [Fact]
    public void AddDate_Nullable_Column()
    {
        Expression<Func<AllTypesView, DateTime?>> expression = model => model.NullableDateTimeField;

        IGridColumn<AllTypesView, DateTime?> actual = columns.AddDate(expression);

        Assert.Equal("nullable-date-time-field", actual.Name);
        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Equal("{0:d}", actual.Format);
        Assert.Single(columns);
    }

    [Fact]
    public void AddBoolean_Column()
    {
        Expression<Func<AllTypesView, Boolean>> expression = model => model.BooleanField;

        IGridColumn<AllTypesView, Boolean> actual = columns.AddBoolean(expression);

        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal(expression, actual.Expression);
        Assert.Equal("boolean-field", actual.Name);
        Assert.Empty(actual.Title.ToString());
        Assert.Single(columns);
    }

    [Fact]
    public void AddBoolean_True()
    {
        GridRow<AllTypesView> row = new(new AllTypesView { BooleanField = true }, 0);
        IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);

        Assert.Equal(Resource.ForString("Yes"), column.ValueFor(row).ToString());
    }

    [Fact]
    public void AddBoolean_False()
    {
        GridRow<AllTypesView> row = new(new AllTypesView { BooleanField = false }, 0);
        IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);

        Assert.Equal(Resource.ForString("No"), column.ValueFor(row).ToString());
    }

    [Fact]
    public void AddBoolean_Nullable_Column()
    {
        Expression<Func<AllTypesView, Boolean?>> expression = model => model.NullableBooleanField;

        IGridColumn<AllTypesView, Boolean?> actual = columns.AddBoolean(expression);

        Assert.Equal("nullable-boolean-field", actual.Name);
        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Single(columns);
    }

    [Fact]
    public void AddBoolean_Nullable()
    {
        GridRow<AllTypesView> row = new(new AllTypesView { NullableBooleanField = null }, 0);
        IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

        Assert.Empty(column.ValueFor(row).ToString());
    }

    [Fact]
    public void AddBoolean_Nullable_True()
    {
        GridRow<AllTypesView> row = new(new AllTypesView { NullableBooleanField = true }, 0);
        IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

        Assert.Equal(Resource.ForString("Yes"), column.ValueFor(row).ToString());
    }

    [Fact]
    public void AddBoolean_Nullable_False()
    {
        GridRow<AllTypesView> row = new(new AllTypesView { NullableBooleanField = false }, 0);
        IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

        Assert.Equal(Resource.ForString("No"), column.ValueFor(row).ToString());
    }

    [Fact]
    public void AddDateTime_Column()
    {
        Expression<Func<AllTypesView, DateTime>> expression = model => model.DateTimeField;

        IGridColumn<AllTypesView, DateTime> actual = columns.AddDateTime(expression);

        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal("date-time-field", actual.Name);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Equal("{0:g}", actual.Format);
        Assert.Single(columns);
    }

    [Fact]
    public void AddDateTime_Nullable_Column()
    {
        Expression<Func<AllTypesView, DateTime?>> expression = model => model.NullableDateTimeField;

        IGridColumn<AllTypesView, DateTime?> actual = columns.AddDateTime(expression);

        Assert.Equal("nullable-date-time-field", actual.Name);
        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Equal("{0:g}", actual.Format);
        Assert.Single(columns);
    }

    [Fact]
    public void AddProperty_Column()
    {
        Expression<Func<AllTypesView, AllTypesView>> expression = model => model;

        IGridColumn<AllTypesView, AllTypesView> actual = columns.AddProperty(expression);

        Assert.Equal("text-start", actual.CssClasses);
        Assert.Equal(expression, actual.Expression);
        Assert.Empty(actual.Title.ToString());
        Assert.Empty(actual.Name);
        Assert.Single(columns);
    }

    [Fact]
    public void AddProperty_SetsColumnName()
    {
        Expression<Func<AllTypesView, SByte>> expression = model => model.NullableSByteField!.Value;

        Assert.Equal("nullable-s-byte-field-value", columns.AddProperty(expression).Name);
    }

    [Fact]
    public void AddProperty_SetsCssClassForEnum()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.EnumField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForSByte()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.SByteField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForByte()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.ByteField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForInt16()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.Int16Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForUInt16()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.UInt16Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForInt32()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.Int32Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForUInt32()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.UInt32Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForInt64()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.Int64Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForUInt64()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.UInt64Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForSingle()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.SingleField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForDouble()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.DoubleField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForDecimal()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.DecimalField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForBoolean()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.BooleanField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForDateTime()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.DateTimeField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableEnum()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.NullableEnumField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableSByte()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableSByteField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableByte()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableByteField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableInt16()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableInt16Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableUInt16()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableUInt16Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableInt32()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableInt32Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableUInt32()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableUInt32Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableInt64()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableInt64Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableUInt64()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableUInt64Field).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableSingle()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableSingleField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableDouble()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableDoubleField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableDecimal()
    {
        Assert.Equal("text-end", columns.AddProperty(model => model.NullableDecimalField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableBoolean()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.NullableBooleanField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForNullableDateTime()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.NullableDateTimeField).CssClasses);
    }

    [Fact]
    public void AddProperty_SetsCssClassForOtherTypes()
    {
        Assert.Equal("text-start", columns.AddProperty(model => model.StringField).CssClasses);
    }

    [Theory]
    [InlineData("", "table-hover")]
    [InlineData(" ", "table-hover")]
    [InlineData(null, "table-hover")]
    [InlineData("test", "test table-hover")]
    [InlineData(" test", "test table-hover")]
    [InlineData("test ", "test  table-hover")]
    [InlineData(" test ", "test  table-hover")]
    public void ApplyDefaults_Values(String? css, String classes)
    {
        IGridColumn column = html.Grid.Columns.Add(model => model.ByteField);
        html.Grid.Attributes["class"] = css;
        column.Filter.IsEnabled = null;
        column.Sort.IsEnabled = null;

        IGrid actual = html.ApplyDefaults().Grid;

        Assert.Equal(Resource.ForString("NoDataFound"), actual.EmptyText);
        Assert.Equal(GridFilterMode.Header, actual.FilterMode);
        Assert.Equal(classes, html.Grid.Attributes["class"]);
        Assert.True(column.Filter.IsEnabled);
        Assert.True(column.Sort.IsEnabled);
        Assert.NotEmpty(actual.Columns);
    }
}
