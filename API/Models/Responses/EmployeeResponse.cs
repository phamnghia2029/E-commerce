namespace API.Models.Responses;

public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public int DepartmentId { get; set; }
    public DepartmentResponse? Department { get; set; }
    public string? Title { get; set; }
    public string? TitleOfCourtesy { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Address { get; set; }
    public bool? Gender { get; set; }

}