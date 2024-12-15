using AspNetCore.Components.Security;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;

namespace AspNetCore.Validators;

public class AccountValidator : AValidator
{
    private IHasher Hasher { get; }

    public AccountValidator(IUnitOfWork unitOfWork, IHasher hasher)
        : base(unitOfWork)
    {
        Hasher = hasher;
    }

    public Boolean CanRecover(AccountRecoveryView _)
    {
        return ModelState.IsValid;
    }
    public Boolean CanReset(AccountResetView view)
    {
        Boolean isValid = IsValidResetToken(view.Token);
        isValid &= ModelState.IsValid;

        return isValid;
    }
    public Boolean CanLogin(AccountLoginView view)
    {
        Boolean isValid = IsAuthenticated(view.Username, view.Password);
        isValid = isValid && IsActive(view.Username);
        isValid &= ModelState.IsValid;

        return isValid;
    }

    public Boolean CanCreate(AccountCreateView view)
    {
        Boolean isValid = IsUniqueUsername(0, view.Username);
        isValid &= IsUniqueEmail(0, view.Email);
        isValid &= ModelState.IsValid;

        return isValid;
    }
    public Boolean CanEdit(AccountEditView view)
    {
        Boolean isValid = IsUniqueUsername(view.Id, view.Username);
        isValid &= IsUniqueEmail(view.Id, view.Email);
        isValid &= ModelState.IsValid;

        return isValid;
    }

    public Boolean CanEdit(ProfileEditView view)
    {
        Boolean isValid = IsUniqueUsername(view.Id, view.Username);
        isValid &= IsCorrectPassword(view.Id, view.Password);
        isValid &= IsUniqueEmail(view.Id, view.Email);
        isValid &= ModelState.IsValid;

        return isValid;
    }
    public Boolean CanDelete(ProfileDeleteView view)
    {
        Boolean isValid = IsCorrectPassword(view.Id, view.Password);
        isValid &= ModelState.IsValid;

        return isValid;
    }

    private Boolean IsAuthenticated(String username, String password)
    {
        String? passhash = UnitOfWork
            .Select<Account>()
            .Where(account => account.Username == username)
            .Select(account => account.Passhash)
            .SingleOrDefault();

        Boolean isCorrect = Hasher.VerifyPassword(password, passhash);

        if (!isCorrect)
            Alerts.AddError(Validation.For<AccountView>("IncorrectAuthentication"));

        return isCorrect;
    }
    private Boolean IsCorrectPassword(Int64 id, String password)
    {
        String? passhash = UnitOfWork
            .Select<Account>()
            .Where(account => account.Id == id)
            .Select(account => account.Passhash)
            .SingleOrDefault();

        Boolean isCorrect = Hasher.VerifyPassword(password, passhash);

        if (!isCorrect)
            ModelState.AddModelError(nameof(ProfileEditView.Password), Validation.For<AccountView>("IncorrectPassword"));

        return isCorrect;
    }
    private Boolean IsUniqueUsername(Int64 id, String username)
    {
        Boolean isUnique = !UnitOfWork
            .Select<Account>()
            .Any(account =>
                account.Id != id &&
                account.Username == username);

        if (!isUnique)
            ModelState.AddModelError(nameof(AccountView.Username), Validation.For<AccountView>("UniqueUsername"));

        return isUnique;
    }
    private Boolean IsUniqueEmail(Int64 id, String email)
    {
        Boolean isUnique = !UnitOfWork
            .Select<Account>()
            .Any(account =>
                account.Id != id &&
                account.Email == email);

        if (!isUnique)
            ModelState.AddModelError(nameof(AccountView.Email), Validation.For<AccountView>("UniqueEmail"));

        return isUnique;
    }

    private Boolean IsValidResetToken(String? token)
    {
        Boolean isValid = UnitOfWork
            .Select<Account>()
            .Any(account =>
                account.RecoveryToken == token &&
                account.RecoveryTokenExpiration > DateTime.Now);

        if (!isValid)
            Alerts.AddError(Validation.For<AccountView>("ExpiredToken"));

        return isValid;
    }
    private Boolean IsActive(String username)
    {
        Boolean isActive = UnitOfWork
            .Select<Account>()
            .Any(account =>
                !account.IsLocked &&
                account.Username == username);

        if (!isActive)
            Alerts.AddError(Validation.For<AccountView>("LockedAccount"));

        return isActive;
    }
}
