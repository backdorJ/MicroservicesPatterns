using Npgsql;
using TwoPc.OrderService.Data;

namespace TwoPC.OrderService.Repositories.Transaction;

public class TransactionRepository : ITransactionRepository
{
    private readonly NpgsqlConnection _connection;
    
    public TransactionRepository(INpgsqlConnectionFactory npgsqlConnectionFactory)
    {
        _connection = npgsqlConnectionFactory.CreateConnection();
    }
    
    public async Task<bool> TryPrepareTransactionAsync(string transactionId, NpgsqlConnection connection)
    {
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"PREPARE TRANSACTION '{transactionId}'";
            command.ExecuteNonQuery();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> TryCommitAsync(string transactionId)
    {
        try
        {
            await _connection.OpenAsync();
            await using var command = _connection.CreateCommand();
            command.CommandText = $"COMMIT PREPARED '{transactionId}'";
            command.ExecuteNonQuery();
            
            await _connection.CloseAsync();
            return true;
        }
        catch (Exception)
        {
            await _connection.CloseAsync();
            return false;
        }
    }

    public async Task<bool> TryRollbackAsync(string transactionId)
    {
        try
        {
            await _connection.OpenAsync();
            await using var command = _connection.CreateCommand();
            command.CommandText = $"ROLLBACK PREPARED '{transactionId}'";
            command.ExecuteNonQuery();

            await _connection.CloseAsync();
            return true;
        }
        catch (Exception)
        {
            await _connection.CloseAsync();
            return false;
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}