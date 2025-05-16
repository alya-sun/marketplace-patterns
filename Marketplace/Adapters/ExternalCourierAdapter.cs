using Marketplace.Utils;

namespace Marketplace.Adapters;

public class ThirdPartyCourier
{
    public void Send(string address, string item)
    {
        Console.WriteLine($"[ThirdPartyCourier] Отправка '{item}' по адресу: {address}");
    }
}

public interface IDeliveryService
{
    void Deliver(string address, string item);
}

public class ExternalCourierAdapter : IDeliveryService
{
    private readonly ThirdPartyCourier _courier = new();

    public void Deliver(string address, string item)
    {
        Logger.Instance.Log("Через адаптер вызываем внешнюю доставку...");
        _courier.Send(address, item);
    }
}