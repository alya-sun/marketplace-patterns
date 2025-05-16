using Marketplace.Models;

namespace Marketplace.Services;

// iterator
public class ProductIterator(List<Product> products) : IEnumerable<Product>
{
    private int _position = 0;

    public bool HasNext() => _position < products.Count;

    public Product Next() => products[_position++];

    public void Reset() => _position = 0;

    public Product? Current => _position > 0 && _position <= products.Count
        ? products[_position - 1]
        : null;

    public IEnumerable<Product> Filter(Func<Product, bool> predicate)
    {
        return products.Where(p => predicate(p));
    }

    public IEnumerator<Product> GetEnumerator() => products.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}