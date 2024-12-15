using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Components.Security;

[AllowUnauthorized]
[ExcludeFromCodeCoverage]
public class AllowUnauthorizedController : AuthorizeController
{
    [HttpGet]
    public ViewResult AuthorizedAction()
    {
        return View();
    }
}
