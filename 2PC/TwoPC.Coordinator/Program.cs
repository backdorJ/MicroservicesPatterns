
using TwoPc.Coordinator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

// Add services to the container.
builder.Services.AddGrpcClient<InventoryService.InventoryService.InventoryServiceClient>(opt =>
{
    opt.Address = new Uri("https://localhost:5239");
});
builder.Services.AddGrpcClient<OrderService.OrderService.OrderServiceClient>(opt =>
{
    opt.Address = new Uri("https://localhost:5030");
});
builder.Services.AddGrpcClient<PaymentService.PaymentService.PaymentServiceClient>(opt =>
{
    opt.Address = new Uri("https://localhost:5240");
});
builder.Services.AddGrpcClient<ShippingService.ShippingService.ShippingServiceClient>(opt =>
{
    opt.Address = new Uri("https://localhost:5049");
});

builder.Services.AddScoped<IOrderService, TwoPc.Coordinator.Services.OrderService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.MapControllers();

app.Run();