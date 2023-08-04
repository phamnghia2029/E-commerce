namespace API.Models.Responses;

public class AccountResponse
{
    public int AccountId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public int? Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpireAt { get; set; }
    public string? AccessToken { get; set; }
    
    public CustomerResponse? Customer { get; set; }

}