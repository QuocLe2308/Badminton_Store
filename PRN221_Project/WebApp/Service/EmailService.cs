using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;

        if (string.IsNullOrEmpty(_smtpSettings.Host))
            throw new ArgumentNullException(nameof(_smtpSettings.Host));
        if (string.IsNullOrEmpty(_smtpSettings.Username))
            throw new ArgumentNullException(nameof(_smtpSettings.Username));
        if (string.IsNullOrEmpty(_smtpSettings.Password))
            throw new ArgumentNullException(nameof(_smtpSettings.Password));
        if (_smtpSettings.Port <= 0)
            throw new ArgumentOutOfRangeException(nameof(_smtpSettings.Port));
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl,
                UseDefaultCredentials = _smtpSettings.UseDefaultCredentials
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error sending email: {ex.Message}");
        }
    }
}
