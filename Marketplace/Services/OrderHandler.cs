using Marketplace.Models;
using Marketplace.Utils;

namespace Marketplace.Services;

public abstract class OrderHandler
{
    protected OrderHandler? Next;

    public void SetNext(OrderHandler next)
    {
        Next = next;
    }

    public abstract void Handle(Order order);
}

public class ValidateOrderHandler : OrderHandler
{
    public override void Handle(Order order)
    {
        Logger.Instance.Log("Валидация заказа...");
        if (order.Products.Count == 0)
            throw new Exception("Пустой заказ!");

        Next?.Handle(order);
    }
}

public class ApplyDiscountHandler : OrderHandler
{
    public override void Handle(Order order)
    {
        Logger.Instance.Log("Применение скидок...");
        if (order.TotalAmount > 1000)
        {
            order.TotalAmount *= 0.9m;
            Logger.Instance.Log("Скидка 10% применена.");
        }

        Next?.Handle(order);
    }
}


public class CheckOrderStatusHandler : OrderHandler
{
    public override void Handle(Order order)
    {
        Logger.Instance.Log("Проверка статуса заказа...");

        if (order.Status == "Cancelled" || order.Status == "Completed")
            throw new InvalidOperationException("Нельзя отменить завершённый или уже отменённый заказ");

        Next?.Handle(order);
    }
}


public class LogOrderHandler : OrderHandler
{
    public override void Handle(Order order)
    {
        Logger.Instance.Log($"Итоговая сумма заказа: {order.TotalAmount}");
    }
}