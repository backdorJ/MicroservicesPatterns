using Microsoft.EntityFrameworkCore;

namespace BookingService.Data;

public class Migrator
{
    private readonly ILogger<Migrator> _logger;
    private readonly AppDbContext _context;

    public Migrator(AppDbContext context, ILogger<Migrator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Migrating database started with context ...");

            await _context.Database.MigrateAsync();
            
            _logger.LogInformation("Migrating database ended with context ...");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}