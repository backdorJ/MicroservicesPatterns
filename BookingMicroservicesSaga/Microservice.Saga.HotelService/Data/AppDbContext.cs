using System.Reflection;
using HotelService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<HotelBooking> HotelBookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}