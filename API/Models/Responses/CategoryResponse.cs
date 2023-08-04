namespace API.Models.Responses;

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
}