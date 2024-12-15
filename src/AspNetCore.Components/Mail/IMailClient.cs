namespace AspNetCore.Components.Mail;

public interface IMailClient
{
    Task SendAsync(String email, String subject, String body);
}
