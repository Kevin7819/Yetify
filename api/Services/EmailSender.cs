using System.Net;
using System.Net.Mail;
using api.Config;
using Microsoft.Extensions.Options;

namespace api.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfig;

    public EmailSender(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }
    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_emailConfig.From, "Yetify");
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = htmlMessage;
        message.IsBodyHtml = true;

        using var smtpClient = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port);
        smtpClient.Credentials = new NetworkCredential(
            _emailConfig.Username, 
            _emailConfig.Password);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        try
        {
            await smtpClient.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"Error SMTP: {ex.StatusCode} - {ex.Message}");
            throw;
        }
    }
}
