using System.Reflection;
using MassTransit;
using Microservice.NotificationService.Data;
using Microservice.NotificationService.Services.EmailService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddTransient<Migrator>();
builder.Services.AddScoped<IEmailService, EmailService>();

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
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration["DefaultConnection"]));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var migrator = serviceProvider.GetRequiredService<Migrator>();
await migrator.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();