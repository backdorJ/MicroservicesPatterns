using Npgsql;

namespace TwoPC.ShippingService.Repositories.Shipping;

public class ShippingRepository : IShippingRepository
{
    public async Task<bool> CreateShippingAsync(CreateShippingRequest request, NpgsqlTransaction transaction, NpgsqlConnection connection)
    {
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "insert into shippings(product_id, user_id) values(@product_id, @user_id);";
            command.Parameters.AddWithValue("product_id", request.ProductId);
            command.Parameters.AddWithValue("user_id", request.UserId);
        
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}