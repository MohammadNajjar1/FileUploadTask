using AspNetCore.Components.Mvc;

namespace AspNetCore.Objects;

public class AccountResetView : AView
{
    [StringLength(36)]
    public String? Token { get; set; }

    [NotTrimmed]
    [StringLength(32)]
    public String NewPassword { get; set; }
}
