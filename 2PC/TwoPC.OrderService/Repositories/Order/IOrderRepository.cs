using Npgsql;

namespace TwoPC.OrderService.Repositories.Order;

public interface IOrderRepository
{
    public Task<bool> CreateOrderAsync(OrderRequest request, NpgsqlTransaction transaction, NpgsqlConnection connection);
}

public record OrderRequest(int ProductId, int UserId, decimal Price);