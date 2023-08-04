namespace API.Models.Responses;

public class OrderDetailResponse
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual ProductResponse Product { get; set; } = null!;
    public short Quantity { get; set; }
    public float Discount { get; set; }
}