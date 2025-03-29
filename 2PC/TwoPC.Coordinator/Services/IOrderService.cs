using TwoPC.Coordinator.Requests;

namespace TwoPc.Coordinator.Services;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderRequest request);
}