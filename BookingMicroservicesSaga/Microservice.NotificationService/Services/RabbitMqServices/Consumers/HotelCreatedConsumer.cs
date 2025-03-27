using Contracts.Events;
using MassTransit;
using Microservice.NotificationService.Data;
using Microservice.NotificationService.Data.Entities;
using Microservice.NotificationService.Services.EmailService;

namespace Microservice.NotificationService.Services.RabbitMqServices.Consumers;

public class HotelCreatedConsumer : IConsumer<HotelBookingCreated>
{
    private readonly ILogger<HotelCreatedConsumer> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IEmailService _emailService;

    public HotelCreatedConsumer(ILogger<HotelCreatedConsumer> logger, AppDbContext dbContext, IEmailService emailService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<HotelBookingCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Started notify message bookingId: {message.BookingId}");
        
        var notification = new Notification
        {
            PaymentId = message.PaymentCreatedId,
            UserId = message.UserId,
            BookingId = message.BookingId,
            HotelId = message.HotelId,
            NotificationStatus = NotificationStatus.Start,
            Email = message.Email,
        };

        try
        {
            // send email
            await _emailService.SendEmailAsync(message.Email, "", "");
            notification.NotificationStatus = NotificationStatus.Success;
        }
        catch (Exception e)
        {
            notification.NotificationStatus = NotificationStatus.Failure;

            // in background worker send failure messages

            _logger.LogError(e, "Failed to send notification");
        }
        finally
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}