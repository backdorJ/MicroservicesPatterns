namespace HotelService.Data.Entities;

public class HotelBooking
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }
}