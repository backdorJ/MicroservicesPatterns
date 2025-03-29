namespace TwoPC.Coordinator.Requests;

public class CreateOrderRequest
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
}