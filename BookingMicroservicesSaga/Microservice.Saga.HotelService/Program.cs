using System.Reflection;
using HotelService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Migrator>();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration["HotelServiceDbConnection"]));

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
var services = scope.ServiceProvider;
var migrator = services.GetRequiredService<Migrator>();
await migrator.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();