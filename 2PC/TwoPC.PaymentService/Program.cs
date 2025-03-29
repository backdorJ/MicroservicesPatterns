
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TwoPc.PaymentService.Data;
using TwoPC.PaymentService.Repositories.Payment;
using TwoPC.PaymentService.Repositories.Transaction;
using TwoPC.PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(conf =>
{
    conf.ListenAnyIP(5240, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddGrpc();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();

var app = builder.Build();

app.MapGrpcService<TwoPC.PaymentService.Services.PaymentService>();

app.Run();