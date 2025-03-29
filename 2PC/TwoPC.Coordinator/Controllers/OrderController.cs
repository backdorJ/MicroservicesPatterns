using Microsoft.AspNetCore.Mvc;
using TwoPC.Coordinator.Requests;
using TwoPc.Coordinator.Services;

namespace TwoPc.Coordinator.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task CreateOrder(CreateOrderRequest request) => await _orderService.CreateOrder(request);
}