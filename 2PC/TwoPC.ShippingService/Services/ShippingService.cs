using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Npgsql;
using ShippingService;
using TwoPc.PaymentService.Data;
using TwoPC.ShippingService.Repositories.Shipping;
using TwoPC.ShippingService.Repositories.Transaction;

namespace TwoPC.ShippingService.Services;

public class ShippingService : global::ShippingService.ShippingService.ShippingServiceBase
{
    private readonly IShippingRepository _shippingRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly NpgsqlConnection _connection;

    public ShippingService(
        INpgsqlConnectionFactory connectionFactory,
        IShippingRepository shippingRepository,
        ITransactionRepository transactionRepository)
    {
        _connection = connectionFactory.CreateConnection();
        _shippingRepository = shippingRepository;
        _transactionRepository = transactionRepository;
    }
    
    public override async Task<CommitResponse> Commit(CommitRequest request, ServerCallContext context)
    {
        return await _transactionRepository.TryCommitAsync(request.TransactionId) 
            ? new CommitResponse
            {
                IsSuccess = true
            }
            : new CommitResponse
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

            var isSuccess = await _shippingRepository.CreateShippingAsync(
                new CreateShippingRequest(
                    request.ProductId,
                    request.UserId),
                transaction, _connection);
            
            if (!isSuccess)
            {
                return new PrepareResponse
                {
                    IsCommitReady = false
                };
            }

            var isPrepared = await _transactionRepository
                .TryPrepareTransactionAsync(request.TransactionId, _connection);
            
            await _connection.CloseAsync();
            
            return new PrepareResponse
            {
                IsCommitReady = isPrepared
            };
        }
        catch
        {
            await _connection.CloseAsync();
            return new PrepareResponse { IsCommitReady = false };
        }
    }

    public override async Task<Empty> Rollback(RollbackRequest request, ServerCallContext context)
    {
        await _transactionRepository.TryRollbackAsync(request.TransactionId);
        return new Empty();
    }
}