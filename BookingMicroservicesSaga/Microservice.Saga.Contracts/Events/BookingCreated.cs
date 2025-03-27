namespace Contracts.Events;

public class BookingCreated
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public decimal Price { get; set; }
    public Guid HotelId { get; set; }
    public string Email { get; set; }
}