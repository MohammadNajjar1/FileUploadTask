using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using AspNetCore.Components.Security;
using AspNetCore.Data;
using AspNetCore.Objects;
using System.Security.Claims;
using System.Security.Principal;

namespace AspNetCore.Services;

public class AccountService : AService
{
    private IHasher Hasher { get; }

    public AccountService(IUnitOfWork unitOfWork, IHasher hasher)
        : base(unitOfWork)
    {
        Hasher = hasher;
    }

    public TView? Get<TView>(Int64 id) where TView : AView
    {
        return UnitOfWork.GetAs<Account, TView>(id);
    }
    public IQueryable<AccountView> GetViews()
    {
        return UnitOfWork
            .Select<Account>()
            .To<AccountView>()
            .OrderByDescending(account => account.Id);
    }

    public Boolean IsLoggedIn(IPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }
    public Boolean IsActive(Int64 id)
    {
        return UnitOfWork.Select<Account>().Any(account => account.Id == id && !account.IsLocked);
    }

    public String? Recover(AccountRecoveryView view)
    {
        Account? account = UnitOfWork.Select<Account>().SingleOrDefault(model => model.Email == view.Email);

        if (account == null)
            return null;

        account.RecoveryTokenExpiration = DateTime.Now.AddMinutes(30);
        account.RecoveryToken = Guid.NewGuid().ToString();

        UnitOfWork.Update(account);
        UnitOfWork.Commit();

        return account.RecoveryToken;
    }
    public void Reset(AccountResetView view)
    {
        Account account = UnitOfWork.Select<Account>().Single(model => model.RecoveryToken == view.Token);
        account.Passhash = Hasher.HashPassword(view.NewPassword);
        account.RecoveryTokenExpiration = null;
        account.RecoveryToken = null;

        UnitOfWork.Update(account);
        UnitOfWork.Commit();
    }

    public void Create(AccountCreateView view)
    {
        Account account = UnitOfWork.To<Account>(view);
        account.Passhash = Hasher.HashPassword(view.Password);
        account.Email = view.Email.ToLower();

        UnitOfWork.Insert(account);
        UnitOfWork.Commit();
    }
    public void Edit(AccountEditView view)
    {
        Account account = UnitOfWork.Get<Account>(view.Id)!;
        account.IsLocked = view.IsLocked;
        account.RoleId = view.RoleId;

        UnitOfWork.Update(account);
        UnitOfWork.Commit();
    }
    public void Edit(ProfileEditView view)
    {
        Account account = UnitOfWork.Get<Account>(view.Id)!;
        account.Email = view.Email.ToLower();
        account.Username = view.Username;

        if (!String.IsNullOrWhiteSpace(view.NewPassword))
            account.Passhash = Hasher.HashPassword(view.NewPassword);

        UnitOfWork.Update(account);
        UnitOfWork.Commit();
    }
    public void Delete(Int64 id)
    {
        UnitOfWork.Delete<Account>(id);
        UnitOfWork.Commit();
    }

    public async Task Login(HttpContext context, String username)
    {
        Account account = UnitOfWork.Select<Account>().Single(model => model.Username == username);

        await context.SignInAsync("Cookies", new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Email, account.Email)
        }, "Password")));
    }
    public async Task Logout(HttpContext context)
    {
        await context.SignOutAsync("Cookies");
    }
}
