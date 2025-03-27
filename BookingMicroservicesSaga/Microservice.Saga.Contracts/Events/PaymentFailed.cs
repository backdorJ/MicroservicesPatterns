namespace Contracts.Events;

public class PaymentFailed
{
    public Guid HotelId { get; set; }
    public Guid UserId { get; set; }
    public Guid BookingId { get; set; }
    public string ErrorMessage { get; set; }
}