using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AspNetCore.Components.Mail;

[ExcludeFromCodeCoverage]
public class SmtpMailClient : IMailClient
{
    private MailConfiguration Config { get; }

    public SmtpMailClient(IOptions<MailConfiguration> config)
    {
        Config = config.Value;
    }

    public async Task SendAsync(String email, String subject, String body)
    {
        using SmtpClient client = new(Config.Host, Config.Port);
        using MailMessage mail = new(Config.Sender!, email, subject, body);

        client.Credentials = new NetworkCredential(Config.Sender, Config.Password);
        client.EnableSsl = Config.EnableSsl;

        mail.SubjectEncoding = Encoding.UTF8;
        mail.BodyEncoding = Encoding.UTF8;
        mail.IsBodyHtml = true;

        await client.SendMailAsync(mail);
    }
}
