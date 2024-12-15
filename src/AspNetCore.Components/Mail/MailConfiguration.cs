namespace AspNetCore.Components.Mail;

public class MailConfiguration
{
    public String? Host { get; set; }
    public Int32 Port { get; set; }

    public String? Sender { get; set; }
    public String? Password { get; set; }

    public Boolean EnableSsl { get; set; }
}
