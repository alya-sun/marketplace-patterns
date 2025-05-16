using Marketplace.Utils;

namespace Marketplace.Services.Payment;

public interface IPaymentStrategy
{
    public string Pay(decimal amount);
}

public class ApplePayPayment : IPaymentStrategy
{
    public string Pay(decimal amount)
    {
        Logger.Instance.Log($"Оплата через ApplePay на сумму {amount}.");
        return $"Оплата через ApplePay на сумму {amount}.";
    }
}

public class GooglePayPayment : IPaymentStrategy
{
    public string Pay(decimal amount)
    {
        Logger.Instance.Log($"Оплата через GooglePay на сумму {amount}.");
        return $"Оплата через GooglePay на сумму {amount}.";
    }
}

public class BonusPointsPayment : IPaymentStrategy
{
    public string Pay(decimal amount)
    {
        Logger.Instance.Log($"Оплата бонусными баллами на сумму {amount}.");
        return $"Оплата бонусными баллами на сумму {amount}.";
    }
}