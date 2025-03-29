using Npgsql;

namespace TwoPc.PaymentService.Data;

public interface INpgsqlConnectionFactory
{
    NpgsqlConnection CreateConnection();
}