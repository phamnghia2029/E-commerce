using API.Models.Responses;

namespace API.Models.Domain;

public class Cart
{
    public Dictionary<int, CartItem> itemMap { get; set; }
    public List<CartItem> items { get; set; }
    public decimal Total { get; set; }

    public Cart(Dictionary<int, CartItem> itemMap, List<CartItem> items)
    {
        this.itemMap = itemMap;
        this.items = items;
    }

    public Cart()
    {
        this.items = new() { };
        this.itemMap = new() { };
    }

    private void CalculateTotal()
    {
        Total = this.items.Sum(x => (decimal)(x.count * x.Product.UnitPrice));
    }

    public void Add(ProductResponse product, int count = 1)
    {
        if (itemMap.ContainsKey(product.ProductId))
        {
            CartItem item;
            itemMap.TryGetValue(product.ProductId, out item);
            item.increase(count);

            items = items.Select(x => x.Product.ProductId == item.Product.ProductId ? item : x).ToList();
            if (item.count <= 0)
            {
                Remove(product);
            }
            CalculateTotal();
            return;
        }

        CartItem newItem = new CartItem(product, count);
        itemMap.Add(product.ProductId, newItem);
        items.Add(newItem);
        CalculateTotal();

    }

    public void Remove(ProductResponse product)
    {
        Remove(product.ProductId);
    }

    public void Remove(int productId)
    {
        itemMap.Remove(productId);
        items.RemoveAll(item => item.Product.ProductId == productId);
        CalculateTotal();

    }

    public CartItem? Get(int productId)
    {
        CartItem? item;
        itemMap.TryGetValue(productId, out item);
        return item;
    }
    public List<CartItem> GetAll()
    {
        return items;
    }

    public void ClearAll()
    {
        this.itemMap.Clear();
        this.items.Clear();
        CalculateTotal();

    }
}

public class CartItem
{
    public ProductResponse Product { get; set; }
    public int count { get; set; }

    public CartItem(ProductResponse product, int count)
    {
        Product = product;
        this.count = count;
    }

    public void increase(int increment)
    {
        count += increment;
    }
}