namespace PaymentService.Data.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public bool IsSuccess { get; set; }
    public Guid BookingId { get; set; }
}