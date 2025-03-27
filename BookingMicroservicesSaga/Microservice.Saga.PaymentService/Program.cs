using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<Migrator>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["DefaultConnection"]));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetExecutingAssembly());
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], configurator =>
        {
            configurator.Username(builder.Configuration["RabbitMq:Username"]!);
            configurator.Password(builder.Configuration["RabbitMq:Password"]!);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var migrator = serviceProvider.GetRequiredService<Migrator>();
await migrator.MigrateAsync(CancellationToken.None);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();