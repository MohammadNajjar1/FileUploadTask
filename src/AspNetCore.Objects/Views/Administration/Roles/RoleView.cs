using AutoMapper;
using AspNetCore.Components.Tree;
using NonFactors.Mvc.Lookup;

namespace AspNetCore.Objects;

public class RoleView : AView<Role>
{
    [LookupColumn]
    [StringLength(128)]
    public String Title { get; set; }

    public MvcTree Permissions { get; set; }

    public RoleView()
    {
        Permissions = new MvcTree();
    }

    internal override void Map(Profile profile)
    {
        profile.CreateMap<Role, RoleView>().ForMember(role => role.Permissions, member => member.MapFrom(role =>
            new MvcTree { SelectedIds = new HashSet<Int64>(role.Permissions.Select(role => role.PermissionId)) }));
        profile.CreateMap<RoleView, Role>().ForMember(role => role.Permissions, member => member.MapFrom(role =>
            role.Permissions.SelectedIds.Select(permission => new RolePermission { PermissionId = permission }).ToList()));
    }
}
