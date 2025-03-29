using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using InventoryService;
using Npgsql;
using TwoPc.InventoryService.Data;
using TwoPC.InventoryService.Repositories.ProductRepository;
using TwoPC.InventoryService.Repositories.Transaction;

namespace TwoPc.InventoryService.Services;

public class InventoryService : global::InventoryService.InventoryService.InventoryServiceBase
{
    private readonly ConcurrentDictionary<string, NpgsqlTransaction> _transactions = new ConcurrentDictionary<string, NpgsqlTransaction>();
    
    private readonly IProductRepository _productRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly NpgsqlConnection _connection;

    public InventoryService(IProductRepository productRepository, ITransactionRepository transactionRepository, INpgsqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _productRepository = productRepository;
        _transactionRepository = transactionRepository;
    }

    public override async Task<TransactionResponse> Commit(TransactionRequest request, ServerCallContext context)
    {
        return await _transactionRepository.TryCommitAsync(request.TransactionId)
            ? new TransactionResponse
            {
                IsSuccess = true
            }
            : new TransactionResponse
            {
                IsSuccess = false
            };
    }

    public override async Task<PrepareResponse> Prepare(PrepareRequest request, ServerCallContext context)
    {
        try
        {
            await _connection.OpenAsync();
            var transaction = await _connection.BeginTransactionAsync();

            var isExist = await _productRepository.AvailableProduct(request.ProductId, transaction, _connection);
            if (!isExist)
            {
                return new PrepareResponse { IsReadyCommit = false };
            }

            var isReserved = await _productRepository.ReserveProduct(request.ProductId, transaction, _connection);
            if (!isReserved)
            {
                return new PrepareResponse { IsReadyCommit = false };
            }

            var isPrepared = await _transactionRepository.TryPrepareTransactionAsync(request.TransactionId, _connection);
            
            await _connection.CloseAsync();
            
            return new PrepareResponse { IsReadyCommit = isPrepared };
        }
        catch
        {
            await _connection.CloseAsync();
            return new PrepareResponse { IsReadyCommit = false };
        }
    }

    public override async Task<Empty> Rollback(TransactionRequest request, ServerCallContext context)
    {
        await _transactionRepository.TryRollbackAsync(request.TransactionId);
        return new Empty();
    }
}