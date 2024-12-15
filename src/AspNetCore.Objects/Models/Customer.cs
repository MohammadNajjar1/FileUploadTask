using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Objects
{
    [Index(nameof(Email), IsUnique = true)]
    public class Customer : AModel
    {
        public String CustomerName { get; set; }
        public String Email { get; set; }
    }
}
