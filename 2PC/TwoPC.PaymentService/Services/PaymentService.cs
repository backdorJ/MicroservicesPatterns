using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Npgsql;
using PaymentService;
using TwoPc.PaymentService.Data;
using TwoPC.PaymentService.Repositories.Payment;
using TwoPC.PaymentService.Repositories.Transaction;

namespace TwoPC.PaymentService.Services;

public class PaymentService : global::PaymentService.PaymentService.PaymentServiceBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly NpgsqlConnection _connection;

    public PaymentService(
        INpgsqlConnectionFactory connectionFactory,
        ITransactionRepository transactionRepository,
        IPaymentRepository paymentRepository)
    {
        _transactionRepository = transactionRepository;
        _paymentRepository = paymentRepository;
        _connection = connectionFactory.CreateConnection();
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

            var isSuccess = await _paymentRepository.CreatePaymentAsync(
                new PaymentRequest(
                    request.UserId,
                    (decimal)request.Price,
                    request.ProductId),
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