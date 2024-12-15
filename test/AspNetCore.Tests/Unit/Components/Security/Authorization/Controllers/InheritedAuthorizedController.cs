using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Components.Security;

[ExcludeFromCodeCoverage]
public class InheritedAuthorizedController : AuthorizeController
{
    [HttpGet]
    public ViewResult InheritanceAction()
    {
        return View();
    }
}
