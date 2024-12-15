namespace AspNetCore.Objects;

public class CustomerView : AView<Customer>
{
    public String CustomerName { get; set; }

    public String Email { get; set; }
}
