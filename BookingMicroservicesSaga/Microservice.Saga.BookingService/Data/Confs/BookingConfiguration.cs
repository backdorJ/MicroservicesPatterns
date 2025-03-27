using BookingService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Data.Confs;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HotelId);
        builder.Property(p => p.Price);
        builder.Property(p => p.UserId);
        builder.Property(p => p.CreateAt);
        builder.Property(p => p.Status);
    }
}