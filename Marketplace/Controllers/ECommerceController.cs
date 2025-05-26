using Marketplace.Adapters;
using Marketplace.Decorators;
using Marketplace.Factories;
using Marketplace.Models;
using Marketplace.Services;
using Marketplace.Services.Payment;
using Marketplace.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ECommerceController(IOrderService orderService) : ControllerBase
{
    
    static ECommerceController()
    {
        _userCarts[TestUser.Id] = new Cart();
        _userCartHistory[TestUser.Id] = new List<CartMemento>();
        Logger.Instance.Log($"Тестовый пользователь с id={TestUser.Id} инициализирован.");
    }
    
    private static Dictionary<int, Order> _orders = new();
    private static Dictionary<string, Cart> _userCarts = new();
    private static Dictionary<string, List<CartMemento>> _userCartHistory = new();
    private static List<Product> _availableProducts = new()
    {
        new Product { Id = 1, Name = "Ноутбук", Price = 45000 },
        new Product { Id = 2, Name = "Смартфон", Price = 25000 },
        new Product { Id = 3, Name = "Наушники", Price = 5000 },
        new Product { Id = 4, Name = "Клавиатура", Price = 3000 },
        new Product { Id = 5, Name = "Мышь", Price = 1500 },
        new Product { Id = 6, Name = "Монитор", Price = 12000 },
        new Product { Id = 7, Name = "Игровая консоль", Price = 35000 },
        new Product { Id = 8, Name = "Внешний жесткий диск", Price = 4000 },
        new Product { Id = 9, Name = "Планшет", Price = 18000 },
        new Product { Id = 10, Name = "Смарт-часы", Price = 7000 },
        new Product { Id = 11, Name = "Веб-камера", Price = 2500 },
        new Product { Id = 12, Name = "Колонки", Price = 6000 },
        new Product { Id = 13, Name = "Игровая мышь", Price = 4500 },
        new Product { Id = 14, Name = "Коврик для мыши", Price = 800 },
        new Product { Id = 15, Name = "USB-хаб", Price = 1200 },
        new Product { Id = 16, Name = "Сетевой фильтр", Price = 1500 },
        new Product { Id = 17, Name = "Беспроводные наушники", Price = 9000 },
        new Product { Id = 18, Name = "Принтер", Price = 8000 },
        new Product { Id = 19, Name = "Роутер", Price = 3000 },
        new Product { Id = 20, Name = "Игровая клавиатура", Price = 6000 }
        
    };
    private static readonly User TestUser = new User { Id = "0" };
    
    [HttpGet("products")]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        Logger.Instance.Log("All products requested.");
        return Ok(_availableProducts);
    }
    
    [HttpGet("products/filter")]
    public ActionResult<IEnumerable<Product>> FilterProducts([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
    {
        var iterator = new ProductIterator(_availableProducts);
        
        var filteredProducts = iterator.Filter(p => 
            (!minPrice.HasValue || p.Price >= minPrice.Value) && 
            (!maxPrice.HasValue || p.Price <= maxPrice.Value));
            
        return Ok(filteredProducts);
    }
    
    [HttpPost("cart/{userId}/add/{productId}")]
    public ActionResult AddToCart(string userId, int productId, [FromBody] Product customProduct = null)
    {
        var product = _availableProducts.Find(p => p.Id == productId);
        if (product == null)
            return NotFound("Продукт не найден");
            
        if (!_userCarts.ContainsKey(userId))
            _userCarts[userId] = new Cart();
            
        var cart = _userCarts[userId];
        
        // Сохраняем состояние корзины перед изменением (Memento Pattern)
        if (!_userCartHistory.ContainsKey(userId))
            _userCartHistory[userId] = new List<CartMemento>();
            
        _userCartHistory[userId].Add(cart.Save());

        var productToAdd = customProduct ?? product;
        
        cart.AddItem(productToAdd);
        Logger.Instance.Log($"User {userId} added product {productToAdd.Name} in cart");
        
        return Ok(cart.GetItems());
    }
    
    [HttpPost("cart/{userId}/remove/{productId}")]
    public ActionResult RemoveFromCart(string userId, int productId)
    {
        if (!_userCarts.ContainsKey(userId))
            return NotFound("Cart not found");
            
        var cart = _userCarts[userId];
        var product = cart.GetItems().Find(p => p.Id == productId);
        
        if (product == null)
            return NotFound("Product not found in cart");
            
        // Сохраняем состояние корзины перед изменением (Memento Pattern)
        if (!_userCartHistory.ContainsKey(userId))
            _userCartHistory[userId] = [];
            
        _userCartHistory[userId].Add(cart.Save());
        
        cart.RemoveItem(product);
        Logger.Instance.Log($"Пользователь {userId} удалил продукт {product.Name} из корзины");
        
        return Ok(cart.GetItems());
    }
    
    [HttpPost("cart/{userId}/undo")]
    public ActionResult UndoCartChange(string userId)
    {
        if (!_userCarts.ContainsKey(userId) || !_userCartHistory.ContainsKey(userId) || !_userCartHistory[userId].Any())
            return BadRequest("Нет истории изменений корзины");
            
        var cart = _userCarts[userId];
        var mementoList = _userCartHistory[userId];
        
        var lastMemento = mementoList.Last();
        mementoList.RemoveAt(mementoList.Count - 1);
        
        cart.Restore(lastMemento);
        Logger.Instance.Log($"Пользователь {userId} отменил последнее изменение корзины");
        
        return Ok(cart.GetItems());
    }
    
    [HttpGet("cart/{userId}")]
    public ActionResult GetCart(string userId)
    {
        if (!_userCarts.ContainsKey(userId))
            return NotFound("Корзина не найдена");
            
        return Ok(_userCarts[userId].GetItems());
    }
    
    [HttpPost("product/{productId}/customize")]
    public ActionResult<IProduct> CustomizeProduct(
        int productId, 
        [FromQuery] bool giftWrap = false, 
        [FromQuery] bool expressDelivery = false)
    {
        var product = _availableProducts.Find(p => p.Id == productId);
        if (product == null)
            return NotFound("Продукт не найден");
            
        // Decorator Pattern для кастомизации продукта
        IProduct baseProduct = new BaseProduct(product);
        
        if (giftWrap)
            baseProduct = new GiftWrapDecorator(baseProduct);
            
        if (expressDelivery)
            baseProduct = new ExpressDeliveryDecorator(baseProduct);
            
        var result = new 
        {
            Description = baseProduct.GetDescription(),
            Price = baseProduct.GetPrice()
        };
        
        return Ok(result);
    }
    
    [HttpPost("order/{userId}")]
    public ActionResult<Order> PlaceOrder(string userId, [FromQuery] string paymentMethod = "apple")
    {
        if (!_userCarts.ContainsKey(userId) || _userCarts[userId].GetItems().Count == 0)
            return BadRequest("Корзина пуста");
            
        var cart = _userCarts[userId];
        var items = cart.GetItems();
        
        // Создание нового заказа
        var order = new Order
        {
            Id = _orders.Count + 1,
            Products = items,
            TotalAmount = items.Sum(p => p.Price),
            Status = "Created"
        };
        
        var placeOrderCommand = new PlaceOrderCommand(order);

        orderService.PlaceOrder(placeOrderCommand);
        
        // Strategy Pattern, Abstract Factory для оплаты
        IPaymentFactory paymentFactory;
        switch (paymentMethod.ToLower())
        {
            case "apple":
                paymentFactory = new ApplePaymentFactory();
                break;
            case "google":
                paymentFactory = new GooglePaymentFactory();
                break;
            case "bonus":
                paymentFactory = new BonusPaymentFactory();
                break;
            default:
                return BadRequest("Неподдерживаемый метод оплаты");
        }
        
        var paymentStrategy = paymentFactory.CreatePaymentStrategy();
        var paymentResult = paymentStrategy.Pay(order.TotalAmount);
        
        // Добавляем заказ в список и очищаем корзину пользователя
        _orders[order.Id] = order;
        _userCarts[userId] = new Cart();
        _userCartHistory[userId]?.Clear();
        
        Logger.Instance.Log($"Пользователь {userId} оформил заказ №{order.Id}");
        
        return Ok(new { Order = order, Payment = paymentResult });
    }

    [HttpPost("order/{orderId}/cancel")]
    public ActionResult CancelOrder(int orderId)
    {
        if (!_orders.ContainsKey(orderId))
            return NotFound("Заказ не найден");
            
        var order = _orders[orderId];
        
        // Command Pattern для отмены заказа
        var cancelOrderCommand = new CancelOrderCommand(order);
        orderService.CancelOrder(cancelOrderCommand);
        
        return Ok(order);
    }
    
    [HttpPost("order/{orderId}/deliver")]
    public ActionResult DeliverOrder(int orderId, [FromBody] DeliveryRequest request)
    {
        if (!_orders.TryGetValue(orderId, out var value))
            return NotFound("Заказ не найден");
            
        var order = value;
        
        if (order.Status != "Confirmed")
            return BadRequest("Заказ не подтвержден");
            
        // Adapter Pattern для службы доставки
        IDeliveryService deliveryService = new ExternalCourierAdapter();
        
        var orderDescription = string.Join(", ", order.Products.Select(p => p.Name));
        deliveryService.Deliver(request.Address, orderDescription);
        
        order.Status = "Delivered";
        Logger.Instance.Log($"Заказ №{orderId} доставлен по адресу: {request.Address}");
        
        return Ok(order);
    }
    
    [HttpGet("order/{orderId}")]
    public ActionResult<Order> GetOrder(int orderId)
    {
        if (!_orders.ContainsKey(orderId))
            return NotFound("Заказ не найден");
            
        return Ok(_orders[orderId]);
    }
    
    [HttpGet("orders")]
    public ActionResult<IEnumerable<Order>> GetAllOrders()
    {
        return Ok(_orders.Values);
    }
    
    [HttpPost("cart/{userId}/clear")]
    public ActionResult ClearCart(string userId)
    {
        if (!_userCarts.ContainsKey(userId))
        {
            _userCarts[userId] = new Cart();
        }

        var cart = _userCarts[userId];
        if (!_userCartHistory.ContainsKey(userId))
        {
            _userCartHistory[userId] = new List<CartMemento>();
        }

        _userCartHistory[userId].Add(cart.Save()); // Сохраняем состояние перед очисткой (для undo)
        cart.Clear(); // Очищаем корзину
        Logger.Instance.Log($"User {userId} cleared their cart");

        return Ok();
    }
}

public class DeliveryRequest
{
    public string Address { get; set; }
}

public class User
{
    public string Id { get; set; }
}
