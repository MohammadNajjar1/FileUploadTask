using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Components.Security.Area;

[Authorize]
[Area("Area")]
[ExcludeFromCodeCoverage]
public class AuthorizedController : Controller
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
    public ViewResult AuthorizedGetAction()
    {
        return View();
    }

    [HttpPost]
    public ViewResult AuthorizedGetAction(Object obj)
    {
        return View(obj);
    }

    [HttpPost]
    public ViewResult AuthorizedPostAction()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    [ActionName("AuthorizedNamedGetAction")]
    public ViewResult GetActionWithName()
    {
        return View();
    }

    [HttpPost]
    [ActionName("AuthorizedNamedGetAction")]
    public ViewResult GetActionWithName(Object obj)
    {
        return View(obj);
    }

    [HttpPost]
    [ActionName("AuthorizedNamedPostAction")]
    public ViewResult PostActionWithName()
    {
        return View();
    }

    [HttpGet]
    [AuthorizeAs(nameof(Action))]
    public ViewResult AuthorizedAsAction()
    {
        return View();
    }

    [HttpGet]
    [AuthorizeAs(nameof(AuthorizedAsSelf))]
    public ViewResult AuthorizedAsSelf()
    {
        return View();
    }

    [HttpGet]
    [AuthorizeAs(nameof(InheritedAuthorizedController.InheritanceAction), Controller = nameof(InheritedAuthorizedController), Area = "")]
    public ViewResult AuthorizedAsOtherAction()
    {
        return View();
    }
}
