using AspNetCore.Components.Mvc;

namespace AspNetCore.Objects;

public class ProfileDeleteView : AView
{
    [NotTrimmed]
    [StringLength(32)]
    public String Password { get; set; }
}
