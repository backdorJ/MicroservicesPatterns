using Npgsql;

namespace TwoPC.PaymentService.Repositories.Payment;

public interface IPaymentRepository
{
    public Task<bool> CreatePaymentAsync(PaymentRequest request, NpgsqlTransaction transaction, NpgsqlConnection connection);
}

public record PaymentRequest(int UserId, decimal Price, int ProductId);