using System.Net;
using System.Net.Mail;
using Contracts;
using Microsoft.Extensions.Configuration;




namespace Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly ILoggerManager _logger;
    private readonly IConfiguration _configuration;
    

    public EmailSenderService(IConfiguration configuration, ILoggerManager logger)
    {
        _configuration = configuration;
       _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:From"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInfo($"Email was sent successfully to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogInfo($"Failure sending mail to {email}: {ex.Message}");

        }
    }
}
