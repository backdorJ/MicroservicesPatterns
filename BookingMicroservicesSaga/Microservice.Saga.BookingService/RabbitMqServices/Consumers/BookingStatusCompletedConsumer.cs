using BookingService.Data;
using BookingService.Data.Entities;
using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookingService.RabbitMqServices.Consumers;

/// <summary>
/// Последний консьюмер который проставляет статус успешности бронирования
/// </summary>
public class BookingStatusCompletedConsumer : IConsumer<HotelBookingCreated>
{
    private readonly ILogger<BookingStatusCompletedConsumer> _logger;
    private readonly AppDbContext _dbContext;

    public BookingStatusCompletedConsumer(AppDbContext dbContext, ILogger<BookingStatusCompletedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HotelBookingCreated> context)
    {
        _logger.LogInformation("Receive message about hotelService booking");
        var message = context.Message;
        
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(x => x.Id == message.BookingId, context.CancellationToken);

        if (booking == null)
        {
            _logger.LogCritical($"Booking {message.BookingId} is not found");
            return;
        }
        
        booking.Status = BookingStatus.Success;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }
}