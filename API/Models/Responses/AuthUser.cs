namespace API.Models.Responses;

public class AuthUser
{
    public int Role { get; set; } = 1;
    public string? Id { get; set; }
    public string? Password { get; set; }
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime TokenExpireAt { get; set; }

    public bool IsAdmin => Role == 1;

}