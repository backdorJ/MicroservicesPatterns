namespace Contracts.Events;

public class HotelCreateFailed
{
    public string ErrorMessage { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public Guid BookingId { get; set; }
}