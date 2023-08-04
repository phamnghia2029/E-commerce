using API.Models.Domain;

namespace API.Models.Requests;

public class PurchaseRequest
{
    public DateTime? OrderDate { get; set; } = DateTime.Now;
    public Cart Cart { get; set; }
    public string CustomerId { get; set; }
    public string Email { get; set; }

}