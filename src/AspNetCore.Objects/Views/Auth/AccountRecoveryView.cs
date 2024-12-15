namespace AspNetCore.Objects;

public class AccountRecoveryView : AView
{
    [EmailAddress]
    [StringLength(256)]
    public String Email { get; set; }
}
