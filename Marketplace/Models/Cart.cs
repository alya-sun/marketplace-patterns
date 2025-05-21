namespace Marketplace.Models;

public class Cart
{
    private List<Product> _items = [];

    public void AddItem(Product product) => _items.Add(product);
    public void RemoveItem(Product product) => _items.Remove(product);
    public List<Product> GetItems() => [.._items];

    public CartMemento Save() => new(_items);
    public void Restore(CartMemento memento) => _items = [..memento.SavedItems];
    public void Clear() => _items.Clear();
}