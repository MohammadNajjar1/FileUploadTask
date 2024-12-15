namespace AspNetCore.Objects;

public class AccountEditView : AView<Account>
{
    [StringLength(32)]
    public String Username { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public String Email { get; set; }

    public Boolean IsLocked { get; set; }

    public Int64? RoleId { get; set; }
}
