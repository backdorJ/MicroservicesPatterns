namespace Microservice.NotificationService.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        _logger.LogInformation($"Sending email: {email}");
    }
}