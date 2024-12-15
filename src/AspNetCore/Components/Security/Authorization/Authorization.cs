using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore.Data;
using AspNetCore.Objects;

namespace AspNetCore.Components.Security;

public class Authorization : IAuthorization
{
    private Dictionary<String, String> Required { get; }
    private Dictionary<String, MethodInfo> Actions { get; }
    private Dictionary<Int64, HashSet<String>> Permissions { get; set; }

    public Authorization(Assembly controllers, IServiceProvider services)
    {
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        Actions = new Dictionary<String, MethodInfo>(StringComparer.OrdinalIgnoreCase);
        Required = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        Permissions = new Dictionary<Int64, HashSet<String>>();

        foreach (Type controller in controllers.GetTypes().Where(IsController))
            foreach (MethodInfo method in controller.GetMethods(flags).Where(IsAction))
                Actions[ActionFor(method)] = method;

        foreach (String action in Actions.Keys)
            if (RequiredPermissionFor(action) is String permission)
                Required[action] = permission;

        Refresh(services);
    }

    public Boolean IsGrantedFor(Int64 accountId, String permission)
    {
        if (Actions.ContainsKey(permission))
        {
            if (Required.TryGetValue(permission, out String? requiredPermission))
                permission = requiredPermission;
            else
                return true;
        }

        return Permissions.ContainsKey(accountId) && Permissions[accountId].Contains(permission);
    }

    public void Refresh(IServiceProvider services)
    {
        IUnitOfWork unitOfWork = services.GetRequiredService<IUnitOfWork>();

        Permissions = unitOfWork
            .Select<Account>()
            .Where(account =>
                !account.IsLocked &&
                account.RoleId != null)
            .Select(account => new
            {
                Id = account.Id,
                Permissions = account
                    .Role!
                    .Permissions
                    .Select(role => role.Permission)
                    .Select(permission => (permission.Area + "/" + permission.Controller + "/" + permission.Action).Trim('/'))
            })
            .ToDictionary(
                account => account.Id,
                account => new HashSet<String>(account.Permissions, StringComparer.OrdinalIgnoreCase));
    }

    private Boolean RequiresAuthorization(String action)
    {
        Boolean? isRequired = null;
        MethodInfo method = Actions[action];
        Type? controller = method.DeclaringType;

        if (method.IsDefined(typeof(AuthorizeAttribute), false))
            isRequired = true;

        if (method.IsDefined(typeof(AllowAnonymousAttribute), false))
            return false;

        if (method.IsDefined(typeof(AllowUnauthorizedAttribute), false))
            isRequired ??= false;

        while (controller != null)
        {
            if (controller.IsDefined(typeof(AuthorizeAttribute), false))
                isRequired ??= true;

            if (controller.IsDefined(typeof(AllowAnonymousAttribute), false))
                return false;

            if (controller.IsDefined(typeof(AllowUnauthorizedAttribute), false))
                isRequired ??= false;

            controller = controller.BaseType;
        }

        return isRequired == true;
    }
    private String? RequiredPermissionFor(String action)
    {
        String[] path = action.Split('/');
        path = path.Length < 3 ? new[] { "" }.Concat(path).ToArray() : path;
        AuthorizeAsAttribute? auth = Actions[action].GetCustomAttribute<AuthorizeAsAttribute>(false);
        String asAction = $"{auth?.Area ?? path[0]}/{auth?.Controller ?? path[1]}/{auth?.Action ?? path[2]}".Trim('/');

        if (action != asAction)
            return RequiredPermissionFor(asAction);

        return RequiresAuthorization(action) ? action : null;
    }
    private String ActionFor(MemberInfo method)
    {
        String action = method.GetCustomAttribute<ActionNameAttribute>(false)?.Name ?? method.Name;
        String? area = method.DeclaringType?.GetCustomAttribute<AreaAttribute>(false)?.RouteValue;
        String? controller = method.DeclaringType?.Name;

        return $"{area}/{controller}/{action}".Trim('/');
    }
    private Boolean IsAction(MethodInfo method)
    {
        return !method.IsSpecialName && !method.IsDefined(typeof(NonActionAttribute));
    }
    private Boolean IsController(Type type)
    {
        return !type.IsDefined(typeof(NonControllerAttribute)) &&
            typeof(Controller).IsAssignableFrom(type) &&
            !type.ContainsGenericParameters &&
            !type.IsAbstract &&
            type.IsPublic;
    }
}
