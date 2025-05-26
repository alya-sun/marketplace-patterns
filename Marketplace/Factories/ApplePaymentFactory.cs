using Marketplace.Services.Payment;

namespace Marketplace.Factories;

public class ApplePaymentFactory : IPaymentFactory
{
    public IPaymentStrategy CreatePaymentStrategy() => new ApplePayPayment();
}