using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AspNetCore.Components.Notifications;
using AspNetCore.Resources;

namespace AspNetCore;

public static class AssertExtensions
{
    public static T Returns<T>(this ActionResult result)
    {
        return Assert.IsAssignableFrom<T>(Assert.IsType<ViewResult>(result).Model);
    }
    public static void Returns<T, M>(this T _, Func<M, ActionResult> action, M model)
        where T : Controller
        where M : class
    {
        Assert.Same(Assert.IsType<ViewResult>(action(model)).Model, model);
    }
    public static async Task Returns<T, M>(this T _, Func<M, Task<ActionResult>> action, M model)
        where T : Controller
        where M : class
    {
        Assert.Same(Assert.IsType<ViewResult>(await action(model)).Model, model);
    }

    public static void IsSingle(this ModelStateDictionary state, String key, String errorMessage)
    {
        Assert.Single(state);
        Assert.Equal(errorMessage, state[key]!.Errors.Single().ErrorMessage);
    }
    public static void IsSingle<T>(this ModelStateDictionary state, String property, String key)
    {
        state.IsSingle(property, Validation.For<T>(key));
    }
    public static void IsSingle<T>(this Alerts alerts, AlertType type, String key, Int32 timeout = 0, String? id = null)
    {
        alerts.IsSingle(type, type == AlertType.Danger ? Validation.For<T>(key) : Message.For<T>(key), timeout, id);
    }
    public static void IsSingle(this Alerts alerts, AlertType type, String message, Int32 timeout = 0, String? id = null)
    {
        Alert actual = alerts.Single();

        Assert.Equal(message, actual.Message);
        Assert.Equal(timeout, actual.Timeout);
        Assert.Equal(type, actual.Type);
        Assert.Equal(id, actual.Id);
    }
}
