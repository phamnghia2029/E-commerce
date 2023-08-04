namespace API.Models.Responses;

public class ProductResponse
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = null!;
    public virtual CategoryResponse? Category { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
    public DateTime? CreatedAt { get; set; }
    
}