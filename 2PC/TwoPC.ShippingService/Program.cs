using Microsoft.AspNetCore.Server.Kestrel.Core;
using TwoPc.PaymentService.Data;
using TwoPC.ShippingService.Repositories.Shipping;
using TwoPC.ShippingService.Repositories.Transaction;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5049, opt =>
    {
        opt.UseHttps();
        opt.Protocols = HttpProtocols.Http1AndHttp2;
    });
});
builder.Services.AddGrpc();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();

var app = builder.Build();

app.MapGrpcService<TwoPC.ShippingService.Services.ShippingService>();

app.Run();