using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Npgsql;
using OrderService;
using TwoPc.OrderService.Data;
using TwoPC.OrderService.Repositories.Order;
using TwoPC.OrderService.Repositories.Transaction;

namespace TwoPC.OrderService.Services;

public class OrderService : global::OrderService.OrderService.OrderServiceBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly NpgsqlConnection _connection;

    public OrderService(INpgsqlConnectionFactory connectionFactory, ITransactionRepository transactionRepository, IOrderRepository orderRepository)
    {
        _transactionRepository = transactionRepository;
        _orderRepository = orderRepository;

        _connection = connectionFactory.CreateConnection();
    }

    public override async Task<CommitResponse> Commit(CommitRequest request, ServerCallContext context)
    {
        return await _transactionRepository.TryCommitAsync(request.TransactionId)
            ? new CommitResponse
            {
                IsSuccess = true
            }
            : new CommitResponse()
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

            var isSuccess = await _orderRepository.CreateOrderAsync(
                new OrderRequest(
                    request.ProductId,
                    request.UserId,
                    (decimal)request.Price),
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