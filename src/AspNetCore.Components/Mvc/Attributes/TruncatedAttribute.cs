using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Components.Mvc;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class TruncatedAttribute : ModelBinderAttribute, IModelBinder
{
    public TruncatedAttribute()
    {
        BinderType = GetType();
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ILoggerFactory logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();

        await new SimpleTypeModelBinder(typeof(DateTime?), logger).BindModelAsync(bindingContext);

        if (bindingContext.Result.IsModelSet)
            bindingContext.Result = ModelBindingResult.Success((bindingContext.Result.Model as DateTime?)?.Date);
    }
}
