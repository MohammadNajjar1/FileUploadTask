namespace AspNetCore.Components.Notifications;

public class AlertsTests
{
    private Alerts alerts;

    public AlertsTests()
    {
        alerts = new Alerts();
    }

    [Fact]
    public void Merge_DoesNotMergeItself()
    {
        alerts.Add(new Alert());
        IEnumerable<Alert> original = alerts.ToArray();

        alerts.Merge(alerts);

        Assert.Equal(alerts, original);
    }

    [Fact]
    public void Merge_Alerts()
    {
        Alerts part = new();
        part.AddError("SecondError");
        alerts.AddError("FirstError");

        IEnumerable<Alert> expected = alerts.Union(part);
        IEnumerable<Alert> actual = alerts;
        alerts.Merge(part);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddInfo_Message()
    {
        alerts.AddInfo("Message", 1);

        Alert actual = alerts.Single();

        Assert.Equal(AlertType.Info, actual.Type);
        Assert.Equal("Message", actual.Message);
        Assert.Equal(1, actual.Timeout);
        Assert.Null(actual.Id);
    }

    [Fact]
    public void AddError_Message()
    {
        alerts.AddError("Message", 1);

        Alert actual = alerts.Single();

        Assert.Equal(AlertType.Danger, actual.Type);
        Assert.Equal("Message", actual.Message);
        Assert.Equal(1, actual.Timeout);
        Assert.Null(actual.Id);
    }

    [Fact]
    public void AddSuccess_Message()
    {
        alerts.AddSuccess("Message", 1);

        Alert actual = alerts.Single();

        Assert.Equal(AlertType.Success, actual.Type);
        Assert.Equal("Message", actual.Message);
        Assert.Equal(1, actual.Timeout);
        Assert.Null(actual.Id);
    }

    [Fact]
    public void AddWarning_Message()
    {
        alerts.AddWarning("Message", 1);

        Alert actual = alerts.Single();

        Assert.Equal(AlertType.Warning, actual.Type);
        Assert.Equal("Message", actual.Message);
        Assert.Equal(1, actual.Timeout);
        Assert.Null(actual.Id);
    }
}
