using Npgsql;
using TwoPc.InventoryService.Data;

namespace TwoPC.InventoryService.Repositories.ProductRepository;

public class ProductRepository : IProductRepository
{
    public async Task<bool> AvailableProduct(int productId, NpgsqlTransaction transaction, NpgsqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT EXISTS(SELECT 1 FROM products WHERE id = {productId} and product_count >= 1)";
        return command.ExecuteScalar() is true;
    }

    public async Task<bool> ReserveProduct(int productId,  NpgsqlTransaction transaction, NpgsqlConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"UPDATE products SET product_count = product_count - 1 WHERE id = {productId}";
        await command.ExecuteNonQueryAsync();

        return true;
    }
}