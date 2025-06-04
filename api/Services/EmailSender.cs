using System.Net;
using System.Net.Mail;
using api.Config;
using Microsoft.Extensions.Options;

namespace api.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IOptions<EmailConfiguration> emailConfig,
            ILogger<EmailSender> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
                try
                {
                    // Validate input parameters
                    if (string.IsNullOrWhiteSpace(toEmail))
                        throw new ArgumentException("Recipient email address is required.", nameof(toEmail));
                    if (string.IsNullOrWhiteSpace(subject))
                        throw new ArgumentException("Email subject is required.", nameof(subject));
                    if (string.IsNullOrWhiteSpace(htmlMessage))
                        throw new ArgumentException("Email message body is required.", nameof(htmlMessage));

                    // Create the email message
                    using var message = new MailMessage
                    {
                        From = new MailAddress(_emailConfig.From, _emailConfig.DisplayName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true,
                        Priority = MailPriority.High
                    };

                    message.To.Add(new MailAddress(toEmail));

                    // Set up the SMTP client
                    using var smtpClient = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port)
                    {
                        Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password),
                        EnableSsl = _emailConfig.UseSsl,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Timeout = 30000 // 30 seconds
                    };

                    _logger.LogInformation("Attempting to send email to {Email} with subject: {Subject}", toEmail, subject);

                    // Send the email
                    await smtpClient.SendMailAsync(message);

                    _logger.LogInformation("✅ Email successfully sent to {Email}", toEmail);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    _logger.LogError(ex, "❌ SMTP failed for one or more recipients: {FailedRecipients}", string.Join(", ", ex.InnerExceptions.Select(e => e.FailedRecipient)));
                    throw new EmailException("Failed to send email to one or more recipients.", ex);
                }
                catch (SmtpException ex)
                {
                    _logger.LogError(ex, "❌ SMTP error occurred while sending email to {Email}. Status: {StatusCode}", toEmail, ex.StatusCode);
                    throw new EmailException("SMTP error occurred while sending the email.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Unexpected error occurred while sending email to {Email}", toEmail);
                    throw new EmailException("Unexpected error occurred while sending the email.", ex);
                }
            }
}

    public class EmailException : Exception
        {
            public EmailException(string message, Exception innerException)
                : base(message, innerException) { }
        }
}