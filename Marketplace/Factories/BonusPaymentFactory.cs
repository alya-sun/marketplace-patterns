using Marketplace.Services.Payment;

namespace Marketplace.Factories;

public class BonusPaymentFactory : IPaymentFactory
{
    public IPaymentStrategy CreatePaymentStrategy() => new BonusPointsPayment();
}