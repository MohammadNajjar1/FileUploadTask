using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Components.Security;

[Authorize]
[ExcludeFromCodeCoverage]
public class AuthorizeController : Controller
{
    [HttpGet]
    public ViewResult Action()
    {
        return View();
    }

    [HttpPost]
    public ViewResult Action(Object obj)
    {
        return View(obj);
    }

    [HttpGet]
    [AllowAnonymous]
    public ViewResult AllowAnonymousAction()
    {
        return View();
    }

    [HttpGet]
    [AllowUnauthorized]
    public ViewResult AllowUnauthorizedAction()
    {
        return View();
    }
}
