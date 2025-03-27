using BookingService.Data;
using BookingService.Data.Entities;
using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookingService.RabbitMqServices.Consumers;

public class PaymentFailedConsumer : IConsumer<PaymentFailed>
{
    private readonly ILogger<PaymentFailedConsumer> _logger;
    private readonly AppDbContext _dbContext;

    public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        var payload = context.Message;
        
        _logger.LogInformation($"Payment failed for hotel: {payload?.HotelId}");
        
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(x => x.Id == payload!.BookingId, context.CancellationToken);

        if (booking is null)
        {
            _logger.LogError($"Booking with id: {payload?.BookingId} not found");
            throw new Exception($"Booking with id: {payload?.BookingId} not found");
        }

        booking.Status = BookingStatus.Failure;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}