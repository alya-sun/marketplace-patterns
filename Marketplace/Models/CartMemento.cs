namespace Marketplace.Models;

// memento
public class CartMemento(List<Product> items)
{
    public List<Product> SavedItems { get; } = [..items];
}