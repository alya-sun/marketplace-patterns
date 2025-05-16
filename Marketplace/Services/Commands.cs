using Marketplace.Models;
using Marketplace.Utils;

namespace Marketplace.Services;

// command
public interface ICommand
{
    void Execute();
}

public class PlaceOrderCommand(Order order) : ICommand
{
    public Order Order { get; set; } = order;
    public void Execute()
    {
        Logger.Instance.Log($"Оформление заказа #{order.Id} на сумму {order.TotalAmount}");
        order.Status = "Confirmed";
    }
}

public class CancelOrderCommand(Order order) : ICommand
{
    public Order Order { get; set; } = order;
    public void Execute()
    {
        Logger.Instance.Log($"Отмена заказа #{order.Id}");
        order.Status = "Cancelled";
    }
}