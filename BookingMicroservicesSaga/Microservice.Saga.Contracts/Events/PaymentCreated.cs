namespace Contracts.Events;

public class PaymentCreated
{
    public Guid PaymentCreatedId { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public string Email { get; set; }
}