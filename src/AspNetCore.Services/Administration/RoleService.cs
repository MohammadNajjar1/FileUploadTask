using AspNetCore.Components.Tree;
using AspNetCore.Data;
using AspNetCore.Objects;
using AspNetCore.Resources;

namespace AspNetCore.Services;

public class RoleService : AService
{
    public RoleService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public IQueryable<RoleView> GetViews()
    {
        return UnitOfWork
            .Select<Role>()
            .To<RoleView>()
            .OrderByDescending(role => role.Id);
    }
    public RoleView? GetView(Int64 id)
    {
        return UnitOfWork.GetAs<Role, RoleView>(id) is RoleView role ? Seed(role) : null;
    }

    public RoleView Seed(RoleView view)
    {
        MvcTreeNode root = new(Resource.ForString("All"));
        view.Permissions.Nodes.Add(root);

        foreach (IGrouping<String, PermissionView> area in GetAllPermissions().GroupBy(permission => permission.Area))
        {
            List<MvcTreeNode> nodes = new();

            foreach (IGrouping<String, PermissionView> controller in area.GroupBy(permission => permission.Controller))
            {
                MvcTreeNode node = new(controller.Key);
                node.Children.AddRange(controller.Select(permission => new MvcTreeNode(permission.Id, permission.Action)));

                nodes.Add(node);
            }

            if (area.Key.Length == 0)
                root.Children.AddRange(nodes);
            else
                root.Children.Add(new MvcTreeNode(area.Key) { Children = nodes });
        }

        return view;
    }

    public void Create(RoleView view)
    {
        Role role = UnitOfWork.To<Role>(view);

        UnitOfWork.Insert(role);
        UnitOfWork.Commit();
    }
    public void Edit(RoleView view)
    {
        List<Int64> permissions = view.Permissions.SelectedIds.ToList();
        Role role = UnitOfWork.Get<Role>(view.Id)!;
        role.Title = view.Title;

        foreach (RolePermission rolePermission in role.Permissions.ToArray())
            if (!permissions.Remove(rolePermission.PermissionId))
                UnitOfWork.Delete(rolePermission);

        foreach (Int64 permissionId in permissions)
            UnitOfWork.Insert(new RolePermission { RoleId = role.Id, PermissionId = permissionId });

        UnitOfWork.Update(role);
        UnitOfWork.Commit();
    }
    public void Delete(Int64 id)
    {
        Role role = UnitOfWork.Get<Role>(id)!;
        role.Accounts.ForEach(account => account.RoleId = null);

        UnitOfWork.DeleteRange(role.Permissions);
        UnitOfWork.Delete(role);
        UnitOfWork.Commit();
    }

    private IEnumerable<PermissionView> GetAllPermissions()
    {
        return UnitOfWork
            .Select<Permission>()
            .ToArray()
            .Select(permission => new PermissionView
            {
                Id = permission.Id,
                Area = Resource.ForArea(permission.Area),
                Action = Resource.ForAction(permission.Action),
                Controller = Resource.ForController($"{permission.Area}/{permission.Controller}".Trim('/'))
            })
            .OrderBy(permission => permission.Area.Length == 0 ? permission.Controller : permission.Area)
            .ThenBy(permission => permission.Controller)
            .ThenBy(permission => permission.Action);
    }
}
