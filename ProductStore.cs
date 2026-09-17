using System.Collections.Concurrent;

namespace ProductApi;

public class ProductStore
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId;

    public ProductStore()
    {
        Add(new ProductRequest("Keyboard", "Mechanical keyboard", 89.90m));
        Add(new ProductRequest("Monitor", "27-inch 4K monitor", 349.99m));
    }

    public IEnumerable<Product> GetAll() => _products.Values.OrderBy(p => p.Id);

    public Product? GetById(int id) => _products.GetValueOrDefault(id);

    public Product Add(ProductRequest request)
    {
        var id = Interlocked.Increment(ref _nextId);
        var product = new Product(id, request.Name, request.Description, request.Price);
        _products[id] = product;
        return product;
    }

    public Product? Update(int id, ProductRequest request)
    {
        if (!_products.ContainsKey(id))
        {
            return null;
        }

        var updated = new Product(id, request.Name, request.Description, request.Price);
        _products[id] = updated;
        return updated;
    }

    public bool Delete(int id) => _products.TryRemove(id, out _);
}
