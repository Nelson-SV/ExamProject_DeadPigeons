using Common.EmailTemplates;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Service.AdminService;

using System.Net.Mail;
using System.Net;

public class EmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly string _sendGridApiKey;
    private readonly bool _isProduction;

    public EmailService(IOptions<EmailSettings> emailSettings, IOptions<AppOptions> appOptions)
    {
        _emailSettings = emailSettings.Value;
        _sendGridApiKey = appOptions.Value.SendGridApiKey;
        _isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    }

    // Send email asynchronously either via SMTP (MailCatcher) or SendGrid (Production)
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        if (_isProduction && !string.IsNullOrEmpty(_sendGridApiKey))
        {
            await SendEmailUsingSendGrid(to, subject, body, isHtml);
        }
        else
        {
            await SendEmailUsingSmtpClient(to, subject, body, isHtml);
        }
    }

    // Method to send email via SendGrid
    private async Task SendEmailUsingSendGrid(string to, string subject, string body, bool isHtml)
    {
        var client = new SendGridClient(_sendGridApiKey);
        var from = new EmailAddress(_emailSettings.SmtpSenderEmail, _emailSettings.SmtpSenderName);
        var toEmail = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

        var response = await client.SendEmailAsync(msg);
        // You can log or handle the response here if needed
    }

    // Method to send email via MailCatcher (SMTP)
    private async Task SendEmailUsingSmtpClient(string to, string subject, string body, bool isHtml)
    {
        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.SmtpSenderEmail, ""),
            EnableSsl = _emailSettings.SmtpEnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SmtpSenderEmail, _emailSettings.SmtpSenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        mailMessage.To.Add(to);
        await smtpClient.SendMailAsync(mailMessage);
    }

    // Loads email template for both environments (MailCatcher or SendGrid)
    public string LoadEmailTemplate(string templateName)
    {
        var rootDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
        var templateDirectory = Path.Combine(rootDirectory, "Common", "EmailTemplates");
        var path = Path.Combine(templateDirectory, templateName);
        
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Email template '{templateName}' not found at: {path}");
        }

        return File.ReadAllText(path);
    }
}
