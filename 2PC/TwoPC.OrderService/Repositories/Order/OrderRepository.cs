using Npgsql;
using TwoPc.OrderService.Data;

namespace TwoPC.OrderService.Repositories.Order;

public class OrderRepository : IOrderRepository
{
    public async Task<bool> CreateOrderAsync(OrderRequest request, NpgsqlTransaction transaction, NpgsqlConnection connection)
    {
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO orders (user_id, amount, product_id) VALUES (@user_id, @amount, @product_id)";
            command.Parameters.AddWithValue("@user_id", request.UserId);
            command.Parameters.AddWithValue("@amount", request.Price);
            command.Parameters.AddWithValue("@product_id", request.ProductId);
        
            await command.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}