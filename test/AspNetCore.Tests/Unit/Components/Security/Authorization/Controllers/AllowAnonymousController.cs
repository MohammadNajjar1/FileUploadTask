using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Components.Security;

[AllowAnonymous]
[ExcludeFromCodeCoverage]
public class AllowAnonymousController : AuthorizeController
{
    [HttpGet]
    public ViewResult SimpleAction()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    [AllowAnonymous]
    [AllowUnauthorized]
    public ViewResult AuthorizedAction()
    {
        return View();
    }
}
