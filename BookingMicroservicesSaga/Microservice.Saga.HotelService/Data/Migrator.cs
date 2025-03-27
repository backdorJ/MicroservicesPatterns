using Microsoft.EntityFrameworkCore;

namespace HotelService.Data;

public class Migrator
{
    private readonly ILogger<Migrator> _logger;
    private readonly AppDbContext _dbContext;

    public Migrator(AppDbContext dbContext, ILogger<Migrator> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Migrating hotel booking database...");
            await _dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}