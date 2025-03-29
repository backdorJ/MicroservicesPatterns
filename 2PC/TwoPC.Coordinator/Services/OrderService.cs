using InventoryService;
using OrderService;
using TwoPC.Coordinator.Requests;
using PrepareRequest = InventoryService.PrepareRequest;

namespace TwoPc.Coordinator.Services;

public class OrderService : IOrderService
{
    private readonly InventoryService.InventoryService.InventoryServiceClient _inventoryService;
    private readonly global::OrderService.OrderService.OrderServiceClient _orderService;
    private readonly PaymentService.PaymentService.PaymentServiceClient _paymentService;
    private readonly ShippingService.ShippingService.ShippingServiceClient _shippingService;

    public OrderService(
        InventoryService.InventoryService.InventoryServiceClient inventoryService,
        global::OrderService.OrderService.OrderServiceClient orderService,
        PaymentService.PaymentService.PaymentServiceClient paymentService,
        ShippingService.ShippingService.ShippingServiceClient shippingService)
    {
        _inventoryService = inventoryService;
        _orderService = orderService;
        _paymentService = paymentService;
        _shippingService = shippingService;
    }

    public async Task CreateOrder(CreateOrderRequest request)
    {
        var uniqueId = Guid.NewGuid().ToString();
        var inventoryTransactionId = $"inventory_{uniqueId}";
        var orderTransactionId = $"order_{uniqueId}";
        var paymentTransactionId = $"payment_{uniqueId}";
        var shippingTransactionId = $"shipping_{uniqueId}";
        
        var userId = Random.Shared.Next(int.MaxValue);

        // prepare inventory service
        var inventoryServiceIsReadyCommit = await _inventoryService.PrepareAsync(new PrepareRequest
        {
            ProductId = request.ProductId,
            UserId = userId,
            TransactionId = inventoryTransactionId,
        });

        // prepare order service
        var orderServiceIsReadyCommit = await _orderService.PrepareAsync(new global::OrderService.PrepareRequest
        {
            ProductId = request.ProductId,
            TransactionId = orderTransactionId,
            UserId = userId,
            Price = (double)request.Price,
        });
        
        // prepare payment service
        var paymentServiceIsReadyCommit = await _paymentService.PrepareAsync(new PaymentService.PrepareRequest
        {
            ProductId = request.ProductId,
            TransactionId = paymentTransactionId,
            UserId = userId,
            Price = (double)request.Price
        });

        // prepare shipping service
        var shippingServiceIsReadyCommit = await _shippingService.PrepareAsync(new ShippingService.PrepareRequest
        {
            ProductId = request.ProductId,
            TransactionId = shippingTransactionId,
            UserId = userId
        });

        if (inventoryServiceIsReadyCommit.IsReadyCommit &&
            orderServiceIsReadyCommit.IsCommitReady &&
            paymentServiceIsReadyCommit.IsCommitReady &&
            shippingServiceIsReadyCommit.IsCommitReady)
        {
            //commit inventory service
            await _inventoryService.CommitAsync(new TransactionRequest
            {
                TransactionId = inventoryTransactionId
            });

            //commit order service
            await _orderService.CommitAsync(new CommitRequest
            {
                TransactionId = orderTransactionId
            });
            
            //commit payment service
            await _paymentService.CommitAsync(new PaymentService.CommitRequest
            {
                TransactionId = paymentTransactionId
            });

            await _shippingService.CommitAsync(new ShippingService.CommitRequest()
            {
                TransactionId = shippingTransactionId
            });
            
            return;
        }

        await _inventoryService.RollbackAsync(new TransactionRequest
        {
            TransactionId = inventoryTransactionId
        });

        await _orderService.RollbackAsync(new RollbackRequest
        {
            TransactionId = orderTransactionId
        });

        await _paymentService.RollbackAsync(new PaymentService.RollbackRequest
        {
            TransactionId = paymentTransactionId
        });

        await _shippingService.RollbackAsync(new ShippingService.RollbackRequest
        {
            TransactionId = shippingTransactionId
        });
    }
}