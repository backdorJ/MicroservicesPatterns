namespace BookingService.Models.CreateBooking;

public class CreateBookingRequest
{
    public decimal Amount { get; set; }
    public Guid HotelId { get; set; }
    public string Email { get; set; }
}