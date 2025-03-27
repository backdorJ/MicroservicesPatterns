using System.Text.Json;
using Contracts.Events;
using HotelService.Data;
using HotelService.Data.Entities;
using MassTransit;

namespace HotelService.Services.RabbitMqServices.Consumers;

public class PaymentCreatedConsumer : IConsumer<PaymentCreated>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentCreatedConsumer> _logger;
    private readonly AppDbContext _dbContext;

    public PaymentCreatedConsumer(IBus bus, ILogger<PaymentCreatedConsumer> logger, AppDbContext dbContext)
    {
        _bus = bus;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<PaymentCreated> context)
    {
        var message = context.Message;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(context.CancellationToken);
        try
        {
            if (message == null)
            {
                _logger.LogError("PaymentCreated could not be deserialised.");
                throw new Exception("PaymentCreated could not be deserialised.");
            }

            var hotelBooking = new HotelBooking
            {
                UserId = message.UserId,
                PaymentId = message.PaymentCreatedId,
                BookingId = message.BookingId,
                HotelId = message.HotelId,
            };

            var hotelBookingEvent = new HotelBookingCreated
            {
                UserId = message.UserId,
                BookingId = message.BookingId,
                HotelId = message.HotelId,
                Email = message.Email,
            };

            await _dbContext.HotelBookings.AddAsync(hotelBooking, context.CancellationToken);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            // send to notify service + booking service
            
            await _bus.Publish(hotelBookingEvent, context.CancellationToken);

            await transaction.CommitAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(context.CancellationToken);

            //send to payment failure
            await _bus.Publish(new HotelCreateFailed()
            {
                ErrorMessage = $"Booking hotel failed. InnerException: {e.InnerException}",
                UserId = message?.UserId ?? Guid.Empty,
                HotelId = message?.HotelId ?? Guid.Empty,
                BookingId = message?.BookingId ?? Guid.Empty
            }, context.CancellationToken);
        }
    }
}