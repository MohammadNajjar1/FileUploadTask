using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Components.Mvc;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NotTrimmedAttribute : ModelBinderAttribute, IModelBinder
{
    public NotTrimmedAttribute()
    {
        BinderType = GetType();
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ILoggerFactory logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();

        await new SimpleTypeModelBinder(typeof(String), logger).BindModelAsync(bindingContext);
    }
}
