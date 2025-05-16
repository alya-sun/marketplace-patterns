using Marketplace.Models;

namespace Marketplace.Decorators;

public interface IProduct
{
    string GetDescription();
    decimal GetPrice();
}

public class BaseProduct(Product product) : IProduct
{
    public string GetDescription() => product.Name;
    public decimal GetPrice() => product.Price;
}

public class GiftWrapDecorator(IProduct product) : IProduct
{
    public string GetDescription() => product.GetDescription() + " + подарочная упаковка";
    public decimal GetPrice() => product.GetPrice() + 50;
}

public class ExpressDeliveryDecorator(IProduct product) : IProduct
{
    public string GetDescription() => product.GetDescription() + " + ускоренная доставка";
    public decimal GetPrice() => product.GetPrice() + 150;
}