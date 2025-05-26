// using Marketplace.Services.Payment;
//
// namespace Marketplace.Factories;
//
// public static class PaymentFactory // иерархия продукта - абстрактная
// {
//     public static IPaymentStrategy Create(string method)
//     {
//         return method.ToLower() switch
//         {
//             "apple" => new ApplePayPayment(),
//             "google" => new GooglePayPayment(),
//             "bonus" => new BonusPointsPayment(),
//             _ => throw new ArgumentException("Unknown payment method")
//         };
//     }
// }