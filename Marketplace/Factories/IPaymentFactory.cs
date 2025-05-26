using Marketplace.Services.Payment;

namespace Marketplace.Factories;

public interface IPaymentFactory
{
    IPaymentStrategy CreatePaymentStrategy();
}