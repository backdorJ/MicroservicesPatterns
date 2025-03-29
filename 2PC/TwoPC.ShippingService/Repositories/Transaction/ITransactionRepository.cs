using Npgsql;

namespace TwoPC.ShippingService.Repositories.Transaction;

public interface ITransactionRepository
{
    public Task<bool> TryPrepareTransactionAsync(string transactionId, NpgsqlConnection connection);

    public Task<bool> TryCommitAsync(string transactionId);
    
    public Task<bool> TryRollbackAsync(string transactionId);
}