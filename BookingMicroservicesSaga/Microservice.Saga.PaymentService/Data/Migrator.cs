using Microsoft.EntityFrameworkCore;

namespace PaymentService.Data;

public class Migrator
{
    private readonly ILogger<Migrator> _logger;
    private readonly AppDbContext _dbContext;

    public Migrator(AppDbContext dbContext, ILogger<Migrator> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Migrating database ...");
            await _dbContext.Database.MigrateAsync(cancellationToken: stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}