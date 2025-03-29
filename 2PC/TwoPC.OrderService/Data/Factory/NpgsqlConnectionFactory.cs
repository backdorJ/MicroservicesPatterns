﻿using Npgsql;

namespace TwoPc.OrderService.Data;

public class NpgsqlConnectionFactory : INpgsqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection CreateConnection() => new(_configuration["ConnectionString"]);
}