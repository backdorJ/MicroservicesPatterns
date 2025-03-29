using Npgsql;

namespace TwoPC.InventoryService.Repositories.ProductRepository;

public interface IProductRepository
{
    public Task<bool> AvailableProduct(int productId,  NpgsqlTransaction transaction, NpgsqlConnection connection);
    
    public Task<bool> ReserveProduct(int productId,  NpgsqlTransaction transaction, NpgsqlConnection connection);
}