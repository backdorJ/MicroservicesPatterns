using Microservice.NotificationService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.NotificationService.Data.Confs;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(p => p.PaymentId);
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.BookingId).IsRequired();
        builder.Property(p => p.HotelId).IsRequired();
        builder.Property(p => p.NotificationStatus).IsRequired();
        builder.Property(p => p.Email).IsRequired();
    }
}