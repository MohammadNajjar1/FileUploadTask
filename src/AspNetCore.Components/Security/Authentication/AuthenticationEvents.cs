using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Components.Security;

public class AuthenticationEvents : CookieAuthenticationEvents
{
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        LinkGenerator link = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        context.RedirectUri = link.GetUriByAction(context.HttpContext, "Login", "Auth", new { area = "", returnUrl = $"{context.Request.PathBase}{context.Request.Path}" })!;

        return base.RedirectToLogin(context);
    }
}
