using Marketplace.Services.Payment;

namespace Marketplace.Factories;

public class GooglePaymentFactory : IPaymentFactory
{   
    public IPaymentStrategy CreatePaymentStrategy() => new GooglePayPayment();
}