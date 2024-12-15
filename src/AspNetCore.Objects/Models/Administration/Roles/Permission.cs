namespace AspNetCore.Objects;

public class Permission : AModel
{
    [StringLength(64)]
    public String Area { get; set; }

    [StringLength(64)]
    public String Controller { get; set; }

    [StringLength(64)]
    public String Action { get; set; }
}
