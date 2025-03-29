using Npgsql;

namespace TwoPC.PaymentService.Repositories.Payment;

public class PaymentRepository : IPaymentRepository
{
    public async Task<bool> CreatePaymentAsync(
        PaymentRequest request,
        NpgsqlTransaction transaction,
        NpgsqlConnection connection)
    {
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO payments(user_id, price, product_id) values(@userId, @price, @productId)";
            command.Parameters.AddWithValue("@userId", request.UserId);
            command.Parameters.AddWithValue("@price", request.Price);
            command.Parameters.AddWithValue("@productId", request.ProductId);
        
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception )
        {
            return false;
        }
    }
}