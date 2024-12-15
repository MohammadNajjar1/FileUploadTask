using Microsoft.AspNetCore.Mvc.ModelBinding;
using AspNetCore.Components.Notifications;

namespace AspNetCore.Validators;

public interface IValidator : IDisposable
{
    Alerts Alerts { get; set; }
    ModelStateDictionary ModelState { get; set; }
}
