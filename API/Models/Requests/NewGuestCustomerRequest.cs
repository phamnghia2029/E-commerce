namespace API.Models.Requests;

public class NewGuestCustomerRequest
{
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
}