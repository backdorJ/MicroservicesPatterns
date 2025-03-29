using Npgsql;

namespace TwoPc.InventoryService.Data;

public interface INpgsqlConnectionFactory
{
    NpgsqlConnection CreateConnection();
}