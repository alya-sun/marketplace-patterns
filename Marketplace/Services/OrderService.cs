namespace Marketplace.Services;

public interface IOrderService
{
    public void PlaceOrder(PlaceOrderCommand command);
    public void CancelOrder(CancelOrderCommand command);
}

// Facade
public class OrderService : IOrderService
{
    public void PlaceOrder(PlaceOrderCommand command)
    {
        // Chain of Responsibility для обработки заказа
        var validator = new ValidateOrderHandler();
        var discounter = new ApplyDiscountHandler();
        var logger = new LogOrderHandler();
        
        validator.SetNext(discounter);
        discounter.SetNext(logger);
        
        validator.Handle(command.Order);
            
        // Command Pattern для оформления заказа
        command.Execute();
    }

    public void CancelOrder(CancelOrderCommand command)
    {
        var validator = new CheckOrderStatusHandler();
        var logger = new LogOrderHandler();
        validator.SetNext(logger);
        validator.Handle(command.Order);
        
        command.Execute();
        
    }
}