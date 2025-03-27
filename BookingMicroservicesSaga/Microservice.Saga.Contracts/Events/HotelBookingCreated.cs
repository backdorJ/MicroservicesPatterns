namespace Contracts.Events;

public class HotelBookingCreated
{
    public Guid PaymentCreatedId { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public string Email { get; set; }
}