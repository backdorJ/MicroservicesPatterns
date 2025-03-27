using HotelService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelService.Data.Confs;

public class HotelBookingConfiguration : IEntityTypeConfiguration<HotelBooking>
{
    public void Configure(EntityTypeBuilder<HotelBooking> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.HotelId).IsRequired();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.PaymentId).IsRequired();
        builder.Property(p => p.BookingId).IsRequired();
    }
}