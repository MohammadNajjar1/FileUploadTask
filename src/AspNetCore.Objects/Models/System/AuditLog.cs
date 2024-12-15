namespace AspNetCore.Objects;

public class AuditLog : AModel
{
    public Int64? AccountId { get; set; }

    [StringLength(16)]
    public String Action { get; set; }

    [StringLength(64)]
    public String EntityName { get; set; }

    public Int64 EntityId { get; set; }

    public String Changes { get; set; }
}
