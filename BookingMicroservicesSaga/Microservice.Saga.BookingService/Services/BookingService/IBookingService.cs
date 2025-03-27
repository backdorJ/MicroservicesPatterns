using BookingService.Models.CreateBooking;

namespace BookingService.Services.BookingService;

public interface IBookingService
{
    public Task<bool> CreateBooking(CreateBookingRequest request, CancellationToken cancellationToken);
}