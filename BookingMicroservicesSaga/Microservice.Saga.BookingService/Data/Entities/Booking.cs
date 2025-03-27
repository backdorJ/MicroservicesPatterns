namespace BookingService.Data.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public decimal Price { get; set; }
    public Guid UserId { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreateAt { get; set; }
}

public enum BookingStatus
{
    Start = 1,
    Success = 2,
    Failure = 3,
}