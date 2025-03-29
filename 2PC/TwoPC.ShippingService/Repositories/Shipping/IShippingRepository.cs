using Npgsql;

namespace TwoPC.ShippingService.Repositories.Shipping;

public interface IShippingRepository
{
    public Task<bool> CreateShippingAsync(
        CreateShippingRequest request,
        NpgsqlTransaction transaction,
        NpgsqlConnection connection);
}

public record CreateShippingRequest(int ProductId, int UserId);