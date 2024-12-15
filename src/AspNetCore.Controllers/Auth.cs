using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Components.Mail;
using AspNetCore.Objects;
using AspNetCore.Resources;
using AspNetCore.Services;
using AspNetCore.Validators;

namespace AspNetCore.Controllers;

[AllowAnonymous]
public class Auth : ValidatedController<AccountValidator, AccountService>
{
    private IMailClient MailClient { get; }

    public Auth(AccountValidator validator, AccountService service, IMailClient mailClient)
        : base(validator, service)
    {
        MailClient = mailClient;
    }

    [HttpGet]
    public ActionResult Recover()
    {
        if (Service.IsLoggedIn(User))
            return RedirectToAction(nameof(Home.Index), nameof(Home));

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Recover(AccountRecoveryView account)
    {
        if (Service.IsLoggedIn(User))
            return RedirectToAction(nameof(Home.Index), nameof(Home));

        if (!Validator.CanRecover(account))
            return View(account);

        if (Service.Recover(account) is String token)
        {
            String? url = Url.Action(nameof(Reset), nameof(Auth), new { token }, Request.Scheme);

            await MailClient.SendAsync(account.Email,
                Message.For<AccountView>("RecoveryEmailSubject"),
                Message.For<AccountView>("RecoveryEmailBody", url));
        }

        Alerts.AddInfo(Message.For<AccountView>("RecoveryInformation"));

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public ActionResult Reset(String? token)
    {
        if (Service.IsLoggedIn(User))
            return RedirectToAction(nameof(Home.Index), nameof(Home));

        if (!Validator.CanReset(new AccountResetView { Token = token }))
            return RedirectToAction(nameof(Recover));

        return View();
    }

    [HttpPost]
    public ActionResult Reset(AccountResetView account)
    {
        if (Service.IsLoggedIn(User))
            return RedirectToAction(nameof(Home.Index), nameof(Home));

        if (!Validator.CanReset(account))
            return RedirectToAction(nameof(Recover));

        Service.Reset(account);

        Alerts.AddSuccess(Message.For<AccountView>("SuccessfulReset"), 4000);

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public ActionResult Login(String? returnUrl)
    {
        if (Service.IsLoggedIn(User))
            return RedirectToLocal(returnUrl);

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginView account)
    {
        if (Service.IsLoggedIn(User))
            return RedirectToLocal(account.ReturnUrl);

        if (!Validator.CanLogin(account))
            return View(account);

        await Service.Login(HttpContext, account.Username);

        return RedirectToLocal(account.ReturnUrl);
    }

    [HttpGet]
    public async Task<RedirectToActionResult> Logout()
    {
        await Service.Logout(HttpContext);

        Response.Headers["Clear-Site-Data"] = @"""cookies"", ""storage"", ""executionContexts""";

        return RedirectToAction(nameof(Login));
    }
}
