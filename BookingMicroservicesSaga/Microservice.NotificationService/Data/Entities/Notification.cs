namespace Microservice.NotificationService.Data.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public Guid UserId { get; set; }
    public Guid BookingId { get; set; }
    public Guid HotelId { get; set; }

    public string Email { get; set; }
    public NotificationStatus NotificationStatus { get; set; }
}

public enum NotificationStatus
{
    Start = 1,
    Success = 2,
    Failure = 3,
}