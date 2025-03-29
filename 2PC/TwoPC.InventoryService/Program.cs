using Microsoft.AspNetCore.Server.Kestrel.Core;
using TwoPc.InventoryService.Data;
using TwoPC.InventoryService.Repositories.ProductRepository;
using TwoPC.InventoryService.Repositories.Transaction;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(conf =>
{
    conf.ListenAnyIP(5239, c =>
    {
        c.UseHttps();
        c.Protocols = HttpProtocols.Http1AndHttp2;
    });
});
builder.Services.AddGrpc();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

app.MapGrpcService<TwoPc.InventoryService.Services.InventoryService>();
app.Run();