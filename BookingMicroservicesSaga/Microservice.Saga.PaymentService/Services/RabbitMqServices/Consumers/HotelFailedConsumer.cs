using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;

namespace PaymentService.Services.RabbitMqServices.Consumers;

public class HotelFailedConsumer : IConsumer<HotelCreateFailed>
{
    private readonly ILogger<HotelFailedConsumer> _logger;
    private readonly AppDbContext _context;
    private readonly IBus _bus;

    public HotelFailedConsumer(ILogger<HotelFailedConsumer> logger, AppDbContext context, IBus bus)
    {
        _logger = logger;
        _context = context;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<HotelCreateFailed> context)
    {
        var message = context.Message;
        
        var payment = await _context.Payments
            .FirstOrDefaultAsync(
                x => x.UserId == message.UserId,
                cancellationToken: context.CancellationToken);

        if (payment is null)
        {
            _logger.LogCritical("Failed to find payment for HotelCreateFailed message");
            throw new ApplicationException("Failed to find payment for HotelCreateFailed message");
        }
        
        //send to notification or logic back money
        
        // send to booking service
        
        var paymentFailed = new PaymentFailed
        {
            HotelId = payment.HotelId,
            UserId = payment.UserId,
            BookingId = payment.BookingId,
            ErrorMessage = $"Failed from Microservice.Saga.HotelService: {message.ErrorMessage}"
        };
        
        await _bus.Publish(paymentFailed, context.CancellationToken);
    }
}