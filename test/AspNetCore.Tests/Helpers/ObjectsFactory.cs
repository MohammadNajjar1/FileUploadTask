using AspNetCore.Controllers;
using AspNetCore.Controllers.Administration;
using AspNetCore.Objects;

namespace AspNetCore;

public static class ObjectsFactory
{
    public static Account CreateAccount(Int64 id)
    {
        return new()
        {
            Username = $"Username{id}",
            Passhash = $"Password{id}Hashed",
            Email = $"{id}@tests.com",
            IsLocked = false,
            RecoveryToken = Guid.NewGuid().ToString(),
            RecoveryTokenExpiration = DateTime.Now.AddMinutes(-5),
            Role = CreateRole(id)
        };
    }

    public static AccountCreateView CreateAccountCreateView(Int64 id)
    {
        return new()
        {
            Id = id,
            Username = $"Username{id}",
            Password = $"Password{id}",
            Email = $"{id}@tests.com"
        };
    }

    public static AccountEditView CreateAccountEditView(Int64 id)
    {
        return new()
        {
            Id = id,
            Username = $"Username{id}",
            Email = $"{id}@tests.com",
            IsLocked = true
        };
    }

    public static AccountLoginView CreateAccountLoginView(Int64 id)
    {
        return new()
        {
            Id = id,
            Username = $"Username{id}",
            Password = $"Password{id}",
            ReturnUrl = $"ReturnUrl{id}"
        };
    }

    public static Permission CreatePermission(Int64 id)
    {
        return new()
        {
            Area = $"Area{id}",
            Action = $"Action{id}",
            Controller = $"Controller{id}"
        };
    }

    public static ProfileDeleteView CreateProfileDeleteView(Int64 id)
    {
        return new()
        {
            Id = id,
            Password = $"Password{id}"
        };
    }

    public static ProfileEditView CreateProfileEditView(Int64 id)
    {
        return new()
        {
            Id = id,
            Email = $"{id}@tests.com",
            Username = $"Username{id}",
            Password = $"Password{id}",
            NewPassword = $"NewPassword{id}"
        };
    }

    public static Role CreateRole(Int64 id)
    {
        return new()
        {
            Title = $"Title{id}",
            Accounts = new List<Account>(),
            Permissions = new List<RolePermission>
            {
                new() { Permission = new Permission { Area = "", Controller = nameof(Auth), Action = nameof(Auth.Recover) } },
                new() { Permission = new Permission { Area = "", Controller = nameof(Profile), Action = nameof(Profile.Delete) } },
                new() { Permission = new Permission { Area = nameof(Area.Administration), Controller = nameof(Roles), Action = nameof(Roles.Create) } },
                new() { Permission = new Permission { Area = nameof(Area.Administration), Controller = nameof(Roles), Action = nameof(Roles.Delete) } },
                new() { Permission = new Permission { Area = nameof(Area.Administration), Controller = nameof(Accounts), Action = nameof(Accounts.Edit) } }
            }
        };
    }

    public static RolePermission CreateRolePermission(Int64 id)
    {
        return new()
        {
            RoleId = id,
            Role = CreateRole(id),
            PermissionId = id,
            Permission = CreatePermission(id)
        };
    }

    public static RoleView CreateRoleView(Int64 id)
    {
        return new()
        {
            Id = id,
            Title = $"Title{id}"
        };
    }
}
