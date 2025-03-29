using Npgsql;

namespace TwoPc.OrderService.Data;

public interface INpgsqlConnectionFactory
{
    NpgsqlConnection CreateConnection();
}