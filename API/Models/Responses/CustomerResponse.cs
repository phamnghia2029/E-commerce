namespace API.Models.Responses;

public class CustomerResponse
{
    public string CustomerId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? IsActive { get; set; }  
}