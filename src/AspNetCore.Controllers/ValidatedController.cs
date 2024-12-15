using Microsoft.AspNetCore.Mvc.Filters;
using AspNetCore.Services;
using AspNetCore.Validators;

namespace AspNetCore.Controllers;

public abstract class ValidatedController<TValidator, TService> : ServicedController<TService>
    where TValidator : IValidator
    where TService : IService
{
    protected TValidator Validator { get; }

    protected ValidatedController(TValidator validator, TService service)
        : base(service)
    {
        Validator = validator;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        Validator.ModelState = ModelState;
        Validator.Alerts = Alerts;
    }

    protected override void Dispose(Boolean disposing)
    {
        Validator.Dispose();

        base.Dispose(disposing);
    }
}
