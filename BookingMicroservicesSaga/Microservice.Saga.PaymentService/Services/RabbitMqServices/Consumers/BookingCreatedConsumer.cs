using Contracts.Events;
using MassTransit;
using PaymentService.Data;
using PaymentService.Data.Entities;

namespace PaymentService.Services.RabbitMqServices.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreated>
{
    private readonly ILogger<BookingCreatedConsumer> _logger;
    private readonly IBus _bus;
    private readonly AppDbContext _dbContext;

    public BookingCreatedConsumer(ILogger<BookingCreatedConsumer> logger, IBus bus, AppDbContext dbContext)
    {
        _logger = logger;
        _bus = bus;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BookingCreated> context)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(context.CancellationToken);
        var message = context.Message;
        try
        {
            _logger.LogInformation($"Processing message: {message}");
           
            // Логика снятия деняг
            var payment = new Payment
            {
                UserId = message.UserId,
                HotelId = message.HotelId,
                IsSuccess = true,
                BookingId = message.BookingId,
            };
            
            await _dbContext.Payments.AddAsync(payment, context.CancellationToken);
            await _dbContext.SaveChangesAsync(context.CancellationToken);
            
            var hotelServiceRequest = new PaymentCreated
            {
                PaymentCreatedId = payment.Id,
                BookingId = message.BookingId,
                UserId = message.UserId,
                HotelId = message.HotelId,
                Email = message.Email,
            };

            await _bus.Publish(hotelServiceRequest, context.CancellationToken);
            
            await transaction.CommitAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(context.CancellationToken);
            
            _logger.LogError($"Payment created failed. {e.Message}");
            
            var paymentFailed = new PaymentFailed
            {
                HotelId = message.HotelId,
                UserId = message.UserId,
                BookingId = message.BookingId,
                ErrorMessage = e.Message,
            };
            
            // send to booking 
            await _bus.Publish(paymentFailed, context.CancellationToken);
        }
    }
}