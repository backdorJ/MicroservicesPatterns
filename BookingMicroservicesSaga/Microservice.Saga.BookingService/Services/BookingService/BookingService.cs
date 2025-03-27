using BookingService.Data;
using BookingService.Data.Entities;
using BookingService.Models.CreateBooking;
using Contracts.Events;
using MassTransit;

namespace BookingService.Services.BookingService;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly IBus _bus;

    public BookingService(ILogger<BookingService> logger, AppDbContext appDbContext, IBus bus)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _bus = bus;
    }

    public async Task<bool> CreateBooking(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            
            var booking = new Booking
            {
                HotelId = request.HotelId,
                Price = request.Amount,
                UserId = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow
            };
            
            await _appDbContext.Bookings.AddAsync(booking, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            var bookingCreated = new BookingCreated
            {
                BookingId = booking.Id,
                UserId = booking.UserId,
                Price = booking.Price,
                HotelId = booking.HotelId,
                Email = request.Email,
            };
            
            // send to paymentService
            await _bus.Publish(bookingCreated, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation($"Booking created: {booking.Id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Booking failed: {ex.Message}");
            return false;
        }
    }
}