
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TwoPc.OrderService.Data;
using TwoPC.OrderService.Repositories.Order;
using TwoPC.OrderService.Repositories.Transaction;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(conf =>
{
    conf.ListenAnyIP(5030, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddGrpc();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

app.MapGrpcService<TwoPC.OrderService.Services.OrderService>();

app.Run();