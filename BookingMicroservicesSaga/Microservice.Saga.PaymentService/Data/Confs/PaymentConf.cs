using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Data.Entities;

namespace PaymentService.Data.Confs;

public class PaymentConf : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(p => p.HotelId).IsRequired();
        builder.Property(p => p.BookingId).IsRequired();
        builder.Property(p => p.IsSuccess);
    }
}