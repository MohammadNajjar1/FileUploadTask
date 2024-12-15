namespace AspNetCore.Objects;

public class PermissionView : AView<Permission>
{
    [StringLength(64)]
    public String Area { get; set; }

    [StringLength(64)]
    public String Controller { get; set; }

    [StringLength(64)]
    public String Action { get; set; }
}
